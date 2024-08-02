using aigis.Models;
using aigis.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace aigis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioDAL _usuarioDal;

        public AuthController(IConfiguration configuration)
        {
            _usuarioDal = new UsuarioDAL(configuration);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                bool result = await _usuarioDal.CreateUserAsync(request);
                if (result)
                {
                    return Ok(new { message = "Usuario registrado con éxito" });
                }
                return BadRequest(new { message = "Error al registrar el usuario" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var usuario = await _usuarioDal.GetUserByEmailAndPasswordAsync(request.Correo, request.Contrasena);
                if (usuario == null)
                {
                    return Unauthorized(new { message = "Correo o contraseña incorrectos." });
                }

                var response = new
                {
                    Id = usuario._id,
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    Rol = usuario.Rol
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
