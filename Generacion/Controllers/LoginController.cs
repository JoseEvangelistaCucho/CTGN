using Generacion.Application.DataBase.cache;
using Generacion.Application.Usuario.Session;
using Generacion.Application.Usuario.Session.SessionStatus;
using Generacion.Application.ValidationSession.Login;
using Generacion.Infraestructura;
using Generacion.Models;
using Generacion.Models.Session;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.ComponentModel;
using System.Reflection;

namespace Generacion.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IValidateUser _validateUser; 
        private readonly CacheDatos _cache;
        public LoginController(IValidateUser validateUser, CacheDatos cache, IConfiguration configuration)
        {
            _cache = cache;
            _validateUser = validateUser;
            _configuration = configuration;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            string valorPruebas = _configuration["Pruebas"];
            bool esTrue = bool.TryParse(valorPruebas, out bool resultadoBooleano);
            ViewBag.Pruebas= esTrue;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ValidarSession(UsuarioSession usuario)
        {
            string mensajesError = usuario.ValidarPropiedadesNulasOVacias();

            if (mensajesError.Any())
            {
                return Json(new { Success = false, message = mensajesError });
            }
            else
            {
                var respuesta = await _validateUser.ValidateSessionUser(usuario);
                if (respuesta.IdRespuesta == 0 && respuesta.Detalle != null)
                {
                    respuesta.Detalle.IdSitio = usuario.Sitio;
                    HttpContext.Session.SetString("usuarioDetail", JsonConvert.SerializeObject(respuesta.Detalle));

                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7), // Expirará en 7 días
                        HttpOnly = true // La cookie solo será accesible desde el servidor
                    };

                    Response.Cookies.Append("usuarioDetail", JsonConvert.SerializeObject(respuesta.Detalle), cookieOptions);

                    return Json(new { Success = true, RedirectUrl = Url.Action("Index", "Home") });
                }
                else
                {
                    return Json(new { Success = false, message = respuesta.Mensaje});
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> CerrarSession()
        {
            HttpContext.Session.Clear();
            // Deshabilitar la caché
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            HttpContext.Session.Remove("usuarioDetail");
            HttpContext.Session.Remove("DetalleUsuario");
            _cache.BorrarDatosCache("DatoCacheOperario");
            return Json(new { Success = true, RedirectUrl = Url.Action("Index", "Login") });
        }

    }
}
