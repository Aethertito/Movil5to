using aigis.Models;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace aigis.DAL
{
    public class UsuarioDAL
    {
        private readonly string _connectionString;

        public UsuarioDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> CreateUserAsync(RegisterRequest request)
        {
            string query = @"
                INSERT INTO usuarios (_id, nombre, correo, contrasena, rol, direccion, telefono, memActiva)
                VALUES (@_id, @Nombre, @Correo, @Contrasena, @Rol, @Direccion, @Telefono, @MemActiva)";

            string hashedPassword = HashPassword(request.Contrasena);
            string userId = Guid.NewGuid().ToString();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@_id", userId);
                        cmd.Parameters.AddWithValue("@Nombre", request.Nombre);
                        cmd.Parameters.AddWithValue("@Correo", request.Correo);
                        cmd.Parameters.AddWithValue("@Contrasena", hashedPassword);
                        cmd.Parameters.AddWithValue("@Rol", "user"); // Asigna el rol por defecto
                        cmd.Parameters.AddWithValue("@Direccion", request.Direccion);
                        cmd.Parameters.AddWithValue("@Telefono", request.Telefono);
                        cmd.Parameters.AddWithValue("@MemActiva", false); // Por defecto, la membresía no está activa

                        conn.Open();
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear usuario: {ex.Message}");
                return false; // Retorna false en caso de error
            }
        }

        public async Task<Usuario> GetUserByEmailAndPasswordAsync(string correo, string contrasena)
        {
            Usuario usuario = null;

            string query = "SELECT * FROM usuarios WHERE correo = @Correo AND contrasena = @Contrasena";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Correo", correo);
                        cmd.Parameters.AddWithValue("@Contrasena", contrasena);

                        conn.Open();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                usuario = new Usuario
                                {
                                    _id = reader["_id"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Correo = reader["correo"].ToString(),
                                    Contrasena = reader["contrasena"].ToString(),
                                    Rol = reader["rol"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario: {ex.Message}");
            }

            return usuario;
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            string hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
