using Microsoft.AspNetCore.Mvc;
using aigis.DataAccess;
using aigis.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace aigis.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Support()
        {
            return View(new AyudaUsuarios());
        }

        [HttpPost]
        public IActionResult Support(AyudaUsuarios model)
        {
            if (ModelState.IsValid)
            {
                // Obtener el correo y el ID del usuario autenticado
                string userEmail = User.Identity.Name; // Supone que el nombre de usuario es el correo
                string userId = ObtenerUsuarioId(userEmail); // M�todo para obtener el ID del usuario desde la base de datos

                if (userId == null)
                {
                    // Manejar el caso donde el ID del usuario no se encuentra
                    ModelState.AddModelError(string.Empty, "No se pudo encontrar el usuario.");
                    return View(model);
                }

                // Asignar valores adicionales
                model.Correo = userEmail;
                model.UsuarioId = userId;
                model.Fecha = DateTime.Now;

                // Generar un ID �nico para la ayuda
                model.Id = Guid.NewGuid().ToString();

                // Guardar el modelo en la base de datos
                GuardarAyudaEnBaseDeDatos(model);

                // Redirigir o mostrar un mensaje de �xito
                return RedirectToAction("Support", new { success = true });
            }

            // Si el modelo no es v�lido, vuelve a mostrar la vista con los errores
            return View(model);
        }

        private string ObtenerUsuarioId(string email)
        {
            string userId = null;
            string query = "SELECT _Id FROM Usuarios WHERE Correo = @Correo";

            try
            {
                using (SqlConnection conn = new SqlConnection(SqlServerConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Correo", email);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userId = reader["Id"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el ID del usuario: {ex.Message}");
            }

            return userId;
        }

        private void GuardarAyudaEnBaseDeDatos(AyudaUsuarios ayuda)
        {
            string query = @"
                INSERT INTO AyudaUsuarios (Id, Correo, Titulo, Problema, UsuarioId, Fecha)
                VALUES (@Id, @Correo, @Titulo, @Problema, @UsuarioId, @Fecha)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", ayuda.Id),
                new SqlParameter("@Correo", ayuda.Correo),
                new SqlParameter("@Titulo", ayuda.Titulo),
                new SqlParameter("@Problema", ayuda.Problema),
                new SqlParameter("@UsuarioId", ayuda.UsuarioId),
                new SqlParameter("@Fecha", ayuda.Fecha)
            };

            try
            {
                SqlServerConnection.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar la ayuda en la base de datos: {ex.Message}");
                // Podr�as agregar l�gica adicional para manejar el error, como registrar el error en una tabla de logs
            }
        }
    }
}
