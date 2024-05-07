using Generacion.Application.Funciones;
using Generacion.Application.InformePerturbacion.Command;
using Generacion.Application.InformePerturbacion.Query;
using Generacion.Infraestructura;
using Generacion.Models;
using Microsoft.AspNetCore.Mvc;

namespace Generacion.Controllers
{
    public class InformePerturbacionController : ApiControllerBase
    {
        private readonly FotoServidor _fotoServidor;

        public InformePerturbacionController(FotoServidor fotoServidor)
        {
            _fotoServidor = fotoServidor;
        }
        public async Task<IActionResult> Index([FromQuery] string fecha,string hora)
        {
            if (!string.IsNullOrEmpty(fecha) && !string.IsNullOrEmpty(hora))
            {
                Respuesta<Dictionary<string, object>> respuesta = await Mediator.Send(new ObtenerDatosPerturbacion()
                {
                    Hora = hora,
                    Fecha = fecha
                });

                ViewData["reload"] = respuesta.Detalle["reload"]; 
                ViewData["datosPrincipal"] = respuesta.Detalle["datosPrincipal"]; 
                ViewData["datosDetallePerturbacion"] = respuesta.Detalle["datosDetallePerturbacion"];
                ViewData["datosSecuencia"] = respuesta.Detalle["datosSecuencia"];
                ViewData["datosSumInterrumpido"] = respuesta.Detalle["datosSumInterrumpido"];
                
                
            }
            else
            {
                ViewData["reload"] =  false;
            }



            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosPerturbacion([FromBody] GuardarDatosInformePerturbacion datos)
        {
            var respuesta = await Mediator.Send(datos);

            _fotoServidor.GuardarJson(datos, "informe_perturbacion.json", this, "Informe_Perturbacion");

            return Json(new { respuesta = respuesta });
        }
    }
}
