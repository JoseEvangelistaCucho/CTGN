using Generacion.Application.Almacen.Query;
using Generacion.Models.Almacen;
using Generacion.Models;
using Microsoft.AspNetCore.Mvc;
using Generacion.Application.Almacen;
using Generacion.Models.Usuario;
using Newtonsoft.Json;
using Generacion.Infraestructura;

namespace Generacion.Controllers
{
    public class RegistroComponentesController : ApiControllerBase
    {
        private readonly ConsultasAlmacen _consultasAlmacen;
        private readonly IAlmacen _almacen;
        public RegistroComponentesController(ConsultasAlmacen consultasAlmacen, IAlmacen almacen)
        {
            _almacen = almacen; 
            _consultasAlmacen = consultasAlmacen;
        }
        public async Task<IActionResult> Index()
        {
            Respuesta<List<TipoComponente>> tiposComponentes = await _consultasAlmacen.ObtenerTipo();
            return View(tiposComponentes.Detalle);
        }

        [HttpPost]
        public async Task<JsonResult> GuardarComponentes(Componentes componentes)
        {
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            Respuesta<string> tiposComponentes = await _almacen.GuardarDatosAlmacen(componentes, user.IdSitio);

            return Json(new { respuesta = tiposComponentes.IdRespuesta });
        }
    }
}
