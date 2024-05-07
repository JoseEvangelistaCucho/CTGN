using Generacion.Application.Funciones;
using Generacion.Application.ION.Query;
using Generacion.Application.MGD;
using Generacion.Application.MGD.Query;
using Generacion.Infraestructura;
using Generacion.Models;
using Generacion.Models.ION;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Generacion.Controllers
{
    public class MGDController : ApiControllerBase
    {
        private readonly ConsultarDatosMGD _consultarDatosMGD;
        private readonly FotoServidor _fotoServidor;

        public MGDController(ConsultarDatosMGD consultarDatosMGD, FotoServidor fotoServidor)
        {
            _fotoServidor = fotoServidor;
            _consultarDatosMGD = consultarDatosMGD;
        }
        public async Task<IActionResult> Index([FromQuery] string fecha)
        {
            Respuesta<List<DatosFormatoMGD>> datosMGD = new Respuesta<List<DatosFormatoMGD>>();
            if (!string.IsNullOrEmpty(fecha))
            {
                DateTime fechaParse = DateTime.Parse(fecha);
              /*  TimeSpan horaEspecifica = new TimeSpan(5, 0, 0);
                fechaParse = fechaParse + horaEspecifica;*/

                datosMGD = await _consultarDatosMGD.ObtenerDatosMGD(fechaParse, fechaParse.AddDays(1));

                string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
                DetalleOperario detalleOperario = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

                ViewData["DetalleOperario"] = detalleOperario;

                ViewBag.ValidacionReporte = datosMGD.Detalle.Count > 0;

                string directorioProyecto = AppDomain.CurrentDomain.BaseDirectory;
                _fotoServidor.GuardarJson(datosMGD.Detalle, "Datos-MGD.json", this, "Reporte-MGD");
            }

            return View(datosMGD.Detalle);
        }
    }
}
