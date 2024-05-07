using Generacion.Application.ReporteModulacion.Command;
using Generacion.Infraestructura;
using Microsoft.AspNetCore.Mvc;

namespace Generacion.Controllers
{
    public class ReporteModulacionController : ApiControllerBase
    {
        public IActionResult Index()
        {
            ViewBag.mostrarSubirArchivo = true;

            return View();
        }


        [HttpPost]
        public async Task<JsonResult> CompararReporte([FromBody] CompararFormatosCommand datos)
        {


            var respuesta = await Mediator.Send(datos);

            return Json(new { respuesta = respuesta });
        }


    }
}
