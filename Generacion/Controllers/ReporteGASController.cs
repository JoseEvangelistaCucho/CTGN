using Generacion.Application.Funciones;
using Generacion.Application.ReporteDiarioGAS;
using Generacion.Application.ReporteDiarioGAS.Query;
using Generacion.Infraestructura;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using Generacion.Models.ReporteDiarioGAS;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using MReporteGAS = Generacion.Models.ReporteDiarioGAS.ReporteGAS;

namespace Generacion.Controllers
{
    public class ReporteGASController : ApiControllerBase
    {
        public readonly IRegistroDatosGAS _registroDatosGAS;
        private readonly ObtenerDatosReporteGAS _obtenerDatosReporteGAS;
        private readonly Function _function;

        public ReporteGASController(IRegistroDatosGAS registroDatosGAS, ObtenerDatosReporteGAS obtenerDatosReporteGAS, Function function)
        {
            _function = function;
            _registroDatosGAS = registroDatosGAS;
            _obtenerDatosReporteGAS = obtenerDatosReporteGAS;
        }
        public async Task<IActionResult> Index()
        {
            DetalleOperario user = await _function.ObtenerDatosOperario();
            Dictionary<string, CabecerasTabla> datoscabecera = await _function.ObtenerDatosCabecera();
            ViewData["Datoscabecera"] = datoscabecera;

            DateTime dateTime = DateTime.Now;
            var idReporteGas = await _obtenerDatosReporteGAS.ObtenerIdReporteGAS();

            Respuesta<Dictionary<string, List<DetalleReporteGas>>> detalleReporte = new Respuesta<Dictionary<string, List<DetalleReporteGas>>>();

            if (idReporteGas.Detalle.IdReporteGas != null)
            {
                detalleReporte = await _obtenerDatosReporteGAS.ObtenerDetallesReporte(idReporteGas.Detalle.IdReporteGas);
            }

            ViewData["detalleReporte"] = detalleReporte.Detalle;
            ViewData["idReporteGas"] = idReporteGas.Detalle ?? new MReporteGAS();

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GuardarDetalle([FromBody] List<DetalleReporteGas> datos)
        {
            Respuesta<string> respuesta = await _registroDatosGAS.GuardarDetalle(datos);

            DateTime dateTime = DateTime.Now;

            MReporteGAS reporteGAS = new MReporteGAS()
            {
                IdReporteGas = datos[0].IdReporteGas,
                Activo = datos.Count >= 14 ? 1 : 0,
                Fecha = dateTime.ToString("dd/MM/yyyy")
            };

            _registroDatosGAS.guardarIdReporte(reporteGAS);

            return Json(new { respuesta = respuesta });
        }
    }
}
