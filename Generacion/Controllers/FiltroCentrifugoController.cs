using Generacion.Application.FiltroCentrifugo.Query;
using Generacion.Models.DashBoard;
using Generacion.Models;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Generacion.Application.FiltroCentrifugo;
using Generacion.Application.Funciones;
using Generacion.Models.FiltroCentrifugo;
using MediatR;
using Generacion.Application.FiltroCentrifugo.Command;
using Generacion.Infraestructura;
using Generacion.Application.Common;

namespace Generacion.Controllers
{
    public class FiltroCentrifugoController : ApiControllerBase
    {
        private readonly IRegistroFiltroCentrifugo _registroFiltroCentrifugo;
        private readonly DatosFiltroCentrifugo _datosFiltroCentrifugo;
        private readonly Function _function;

        public FiltroCentrifugoController(
            DatosFiltroCentrifugo datosFiltroCentrifugo,
            IRegistroFiltroCentrifugo registroFiltroCentrifugo,
            Function function)
        {
            _function = function;
            _registroFiltroCentrifugo = registroFiltroCentrifugo;
            _datosFiltroCentrifugo = datosFiltroCentrifugo;

        }
        public async Task<IActionResult> Index()
        {
            MantenimientoComponentes request = new MantenimientoComponentes()
            {
                TipoComponente = TipoComponente.filtroCentrifugo,
                RequiereId = false,
                Seleccion =Constante.seleccionarTodo
            };

            var respuesta = await Mediator.Send(request);

            ViewData["especificacionesFiltro"] = respuesta.Detalle["especificacionesFiltro"];

            return View(respuesta.Detalle["datosFiltro"]);
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosFiltro([FromBody] List<DetalleFiltro> detalles)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            string mensajesError = "";
            foreach (var item in detalles)
            {
                mensajesError = item.ValidarPropiedadesNulasOVacias();
                if (!string.IsNullOrEmpty(mensajesError))
                    break;

            }
            if (string.IsNullOrEmpty(mensajesError))
            {
                respuesta = await _registroFiltroCentrifugo.GuardarDatosFiltro(detalles);
            }
            else
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = mensajesError;
            }

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDetalleFiltro([FromBody] List<EspecificacionFiltro> detalles)
        {
            Respuesta<string> respuesta = await _registroFiltroCentrifugo.GuardarDetalleFiltro(detalles);

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarReporteFiltro([FromBody] ReporteFiltro datos)
        {
            Respuesta<string> respuesta = await _registroFiltroCentrifugo.GuardarReporteFiltro(datos);

            return Json(new { respuesta = respuesta });
        }
    }
}
