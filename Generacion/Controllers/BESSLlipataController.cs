using Generacion.Application.BessLlipata.DataBess.Query;
using Generacion.Application.BessLlipata.DataCoes.Query;
using Generacion.Application.BessLlipata.ReporteProduccion.Command;
using Generacion.Application.Funciones;
using Generacion.Infraestructura;
using Microsoft.AspNetCore.Mvc;

namespace Generacion.Controllers
{
    public class BESSLlipataController : ApiControllerBase
    {
        private readonly FotoServidor _fotoServidor;
        public BESSLlipataController(FotoServidor fotoServidor)
        {
            _fotoServidor = fotoServidor;
        }
        public async Task<IActionResult> Index([FromQuery] string fecha)
        {
            if (string.IsNullOrEmpty(fecha))
            {
                fecha = DateTime.Now.ToString("dd/MM/yyyy");
            }

            DateTime primerDiaDelMes = new DateTime(DateTime.Parse(fecha).Year, DateTime.Parse(fecha).Month, 1, 0, 30, 0);
            DateTime ultimoDiaDelMes = primerDiaDelMes.AddMonths(1);

            var datosCoes = await Mediator.Send(new ObtenerDatosDataCoes()
            {
                FechaInicio = primerDiaDelMes,
                FechaFin = ultimoDiaDelMes,
                Fecha = DateTime.Parse(fecha)
            });
            var datosBess = await Mediator.Send(new ObtenerDatosBESSQuery()
            {
                TipoConsulta="dia",
                Fecha = DateTime.Parse(fecha).ToString("dd/MM/yy HH:mm:ss")
            });


            ViewBag.Fecha = fecha;
            ViewData["listaDataBESS"] = datosBess.Detalle["listaDataBESS"];
            ViewData["fechaSeleccionado"] = fecha;
            ViewData["datosCoes"] = datosCoes.Detalle["DatosCoes"];
            ViewData["requiereArchivos"] = datosCoes.Detalle["requiereArchivos"];
            ViewData["demandaCoindicente"] = datosCoes.Detalle["demandaCoindicente"];
            ViewData["diasCoincidentesDia"] = datosCoes.Detalle["diasCoincidentesDia"];
            ViewData["diasCoincidentesHora"] = datosCoes.Detalle["diasCoincidentesHora"];
            ViewData["detallePorFechaSeleccionado"] = datosCoes.Detalle["detallePorFechaSeleccionado"];

            return View();
        }
        public async Task<IActionResult> DataCoes([FromQuery] string fecha)
        {
            if (string.IsNullOrEmpty(fecha))
            {
                fecha = DateTime.Now.ToString("dd/MM/yyyy");
            }
            DateTime primerDiaDelMes = new DateTime(DateTime.Parse(fecha).Year, DateTime.Parse(fecha).Month, 1, 0, 30, 0);
            DateTime ultimoDiaDelMes = primerDiaDelMes.AddMonths(1);

            var datosCoes = await Mediator.Send(new ObtenerDatosDataCoes()
            {
                FechaInicio = primerDiaDelMes,
                FechaFin = ultimoDiaDelMes,
                Fecha = DateTime.Parse(fecha)
            });


            ViewData["datosCoes"] = datosCoes.Detalle["DatosCoes"];
            ViewData["requiereArchivos"] = datosCoes.Detalle["requiereArchivos"];

            return View();
        }
        public async Task<IActionResult> Demanda([FromQuery] string fecha)
        {
            if (string.IsNullOrEmpty(fecha))
            {
                fecha = DateTime.Now.ToString("dd/MM/yyyy");
            }
            DateTime primerDiaDelMes = new DateTime(DateTime.Parse(fecha).Year, DateTime.Parse(fecha).Month, 1, 0, 30, 0);
            DateTime ultimoDiaDelMes = primerDiaDelMes.AddMonths(1);
            var datosCoes = await Mediator.Send(new ObtenerDatosDataCoes()
            {
                FechaInicio = primerDiaDelMes,
                FechaFin = ultimoDiaDelMes,
                Fecha = DateTime.Parse(fecha)
            });
            var datosBess = await Mediator.Send(new ObtenerDatosBESSQuery()
            {
                TipoConsulta = "dia",
                Fecha = DateTime.Parse(fecha).ToString("dd/MM/yy HH:mm:ss")
            });

            ViewData["listaDataBESS"] = datosBess.Detalle["listaDataBESS"];
            ViewData["fechaSeleccionado"] = fecha;
            ViewData["datosCoes"] = datosCoes.Detalle["DatosCoes"];
            ViewData["requiereArchivos"] = datosCoes.Detalle["requiereArchivos"];
            ViewData["demandaCoindicente"] = datosCoes.Detalle["demandaCoindicente"];
            ViewData["diasCoincidentesDia"] = datosCoes.Detalle["diasCoincidentesDia"];
            ViewData["diasCoincidentesHora"] = datosCoes.Detalle["diasCoincidentesHora"];
            ViewData["detallePorFechaSeleccionado"] = datosCoes.Detalle["detallePorFechaSeleccionado"];

            //HttpContext.Session.SetString("demandaCoindicente",JsonConvert.SerializeObject(datosCoes.Detalle["demandaCoindicente"]));


            return View();
        }
        public async Task<IActionResult> DataBess([FromQuery] string fecha)
        {
            if (string.IsNullOrEmpty(fecha))
            {
                fecha = DateTime.Now.ToString("dd/MM/yyyy");
            }

            var datosBess = await Mediator.Send(new ObtenerDatosBESSQuery()
            {
                TipoConsulta ="dia",
                Fecha = DateTime.Parse(fecha).ToString("dd/MM/yy HH:mm:ss")
            });

            ViewData["listaDataBESS"] = datosBess.Detalle["listaDataBESS"];
            ViewData["requiereArchivos"] = datosBess.Detalle["requiereArchivos"];


            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDetalleProduccion([FromBody] ProduccionBessLlipata datos)
        {
            var respuesta = await Mediator.Send(datos);

            _fotoServidor.GuardarJson(datos, "BessLlipata.json", this, "Bess-Llipata");

            return Json(new { respuesta = respuesta });
        }
    }

}
