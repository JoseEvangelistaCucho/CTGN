using Generacion.Models.DashBoard;
using Generacion.Models;
using Microsoft.AspNetCore.Mvc;
using Generacion.Application.Common;
using Generacion.Application.FiltroCentrifugo.Command;
using MediatR;
using Generacion.Infraestructura;

namespace Generacion.Controllers
{
    public class FiltroAutomaticoController : ApiControllerBase
    {
        public async Task<IActionResult> Index()
        {
            MantenimientoComponentes request = new MantenimientoComponentes()
            {
                TipoComponente = TipoComponente.filtroAutomatico,
                RequiereId = true,
                Seleccion = string.Empty
            };

            var respuesta = await Mediator.Send(request);

            ViewData["especificacionesFiltro"] = respuesta.Detalle["especificacionesFiltro"];

            return View(respuesta.Detalle["datosFiltro"]);
        }

      
    }
}
