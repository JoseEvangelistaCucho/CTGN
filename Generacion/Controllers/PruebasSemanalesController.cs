using Generacion.Application.Funciones;
using Generacion.Application.PruebasSemanales.BackStart.Command;
using Generacion.Application.PruebasSemanales.BackStart.Query;
using Generacion.Infraestructura;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using Generacion.Models.PruebasSemanales.BlackStart;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Generacion.Controllers
{
    public class PruebasSemanalesController : ApiControllerBase
    {
        private readonly FotoServidor _fotoServidor;
        private readonly Function _function;
        public PruebasSemanalesController( FotoServidor fotoServidor, Function function)
        {
            _fotoServidor = fotoServidor;
            _function = function;
        }
        public async Task<IActionResult> Index()
        {
            Dictionary<string, CabecerasTabla> datosCabecera = await _function.ObtenerDatosCabecera();
            ViewData["datosCabecera"] = datosCabecera;

            var respuesta = await Mediator.Send(new ObtenerDetallePruebaSemanalQuery()
            {
                Tipo = "BLACKSTART"
            });

            List<string> keys = (List<string>)respuesta.Detalle["keys"];

            if (keys.All(key => respuesta.Detalle.ContainsKey(key)))
            {
                ViewData["BlackStart"] = respuesta.Detalle["pruebaSemanal"];
            }
            if (keys.All(key => respuesta.Detalle.ContainsKey(key)))
            {
                ViewData["DetalleBlackStart"] = respuesta.Detalle["detallePruebaSemanal"];
            }

            return View();
        }

        public async Task<IActionResult> SistemaContraIncendios()
        {
            Dictionary<string, CabecerasTabla> datosCabecera = await _function.ObtenerDatosCabecera();
            ViewData["datosCabecera"] = datosCabecera;

            var respuesta = await Mediator.Send(new ObtenerDetallePruebaSemanalQuery()
            {
                Tipo = "SISTEMAINCENDIOS"
            });

            List<string> keys = (List<string>)respuesta.Detalle["keys"];

            if (keys.All(key => respuesta.Detalle.ContainsKey(key)))
            {
                ViewData["SistemaIncendios"] = respuesta.Detalle["pruebaSemanal"];
            }
            if (keys.All(key => respuesta.Detalle.ContainsKey(key)))
            {
                ViewData["DetalleSistemaIncendios"] = respuesta.Detalle["detallePruebaSemanal"];
            }

            return View();
        }


        [HttpPost]
        public async Task<JsonResult> GuardarDetalleBlackStart([FromBody] GuardarDatosPruebaSemanal datos)
        {
            var respuesta = await Mediator.Send(datos);

            if (datos.Tipo.Equals("BLACKSTART"))
            {
                _fotoServidor.GuardarJson(respuesta.Detalle, "BlackStart.json", this, "Black-Start");

            }
            if(datos.Tipo.Equals("SISTEMAINCENDIOS"))
            {
                _fotoServidor.GuardarJson(respuesta.Detalle, "SistemaIncendios.json", this, "Sistemas-Incendios");

            }

            return Json(new { respuesta = respuesta });
        }
        [HttpPost]
        public IActionResult GuardarDatosSistemaContraIncendios()
        {

            return View();
        }
    }
}
