using Generacion.Application.Common;
using Generacion.Application.Usuario;
using Generacion.Application.Usuario.Query;
using Generacion.Infraestructura;
using Generacion.Models;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Generacion.Controllers
{

    public class HistorialController : ApiControllerBase
    {
        private readonly IUsuario _usuario;
        private readonly ConsultarUsuario _consultarUsuario;
        public HistorialController( IUsuario usuario , ConsultarUsuario consultarUsuario)
        {
            _usuario = usuario;
            _consultarUsuario = consultarUsuario;
        }
        public async Task<IActionResult> Index([FromQuery] int idVista)
        {
            DateTime fecha = DateTime.Now;

            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario detalleOperario = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);
            Respuesta<Dictionary<string, List<HistorialUsuario>>> historialOperarios = new Respuesta<Dictionary<string, List<HistorialUsuario>>>();
            if (Constante.idVistaReporteMantenimiento == idVista)
            {
                historialOperarios = await _consultarUsuario.ObtenerDatosHistorialGeneral(fecha.ToString("dd-MM-yyyy"));
            }
            else
            {
                historialOperarios = await _consultarUsuario.ObtenerDatosHistorial(detalleOperario, fecha.ToString("dd-MM-yyyy"));
            }

            return View(historialOperarios);
        }


        [HttpPost]
        public async Task<JsonResult> GuardarHistorial([FromBody] List<HistorialUsuario> historialUsuarios)
        {
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario detalleOperario = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            Respuesta<List<HistorialUsuario>> respuesta = await _usuario.GuardarHistorial(historialUsuarios,detalleOperario.IdOperario);

            return Json(new  { respuesta = respuesta });
        }

     /*   [HttpPost]
        public async Task<JsonResult> EliminarHistorial([FromBody] string id)
        {
           // Respuesta<List<HistorialUsuario>> respuesta = await _usuario.EliminarDatosHistorial(id);

           // return Json(new { respuesta = respuesta });
        }*/
    }
}
