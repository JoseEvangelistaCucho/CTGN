using Generacion.Application.ION;
using Generacion.Application.ION.Query;
using Generacion.Application.MGD;
using Generacion.Models.ION;
using Generacion.Models;
using Microsoft.AspNetCore.Mvc;
using Generacion.Infraestructura;
using Generacion.Application.Funciones;

namespace Generacion.Controllers
{
    public class IONController : ApiControllerBase
    {
        private readonly ConsultarION _consultarION;
        private readonly IDatosMGD _datosMGD;
        private readonly IDatosION _datosION;
        private readonly FotoServidor _fotoServidor;

        public IONController(ConsultarION consultarION, IDatosMGD datosMGD, IDatosION datosION, FotoServidor fotoServidor)
        {
            _fotoServidor = fotoServidor;
            _consultarION = consultarION;
            _datosMGD = datosMGD;
            _datosION = datosION;
        }
        public async Task<IActionResult> Index([FromQuery] string fecha)
        {
            Respuesta<List<DatosFormatoMGD>> respuesta = new Respuesta<List<DatosFormatoMGD>>();
            if (!string.IsNullOrEmpty(fecha))
            {
                respuesta = await Mediator.Send(new ObtenerDatosIONQuery()
                {
                    Fecha = DateTime.Parse(fecha)
                });
                _fotoServidor.GuardarJson(respuesta.Detalle, "DATOS_ION.json", this, "Reporte-ION");

                ViewBag.ValidacionReporte = respuesta.Detalle.Count > 0;
            }
                return View(respuesta.Detalle);
        }
    }
}
