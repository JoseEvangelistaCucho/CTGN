using Generacion.Models.Almacen.Bujias;
using Generacion.Models;
using Microsoft.AspNetCore.Mvc;
using Generacion.Models.Almacen;
using Generacion.Application.Almacen;
using Generacion.Models.Usuario;
using Newtonsoft.Json;
using Generacion.Application.Bujias;
using Generacion.Models.DatosConsola;
using Generacion.Application.Bujias.Query;
using Generacion.Infraestructura;
using Generacion.Application.Bujias.Command;
using Generacion.Application.Funciones;

namespace Generacion.Controllers
{
    public class BujiasController : ApiControllerBase
    {
        private readonly Function _function;
        private readonly IAlmacen _almacen; 
            
        private readonly IBujias _bujias;
        private readonly ConsultaBujias _consultaBujias;
        public BujiasController(IAlmacen almacen, IBujias bujias, ConsultaBujias consultaBujias, Function function)
        {
            _function = function;
            _consultaBujias = consultaBujias;
            _bujias = bujias;
            _almacen = almacen;
        }
        public async Task<IActionResult> Index()
        {
            Respuesta<Dictionary<string, object>> respuesta = await Mediator.Send(new ObtenerDatosBujias());
            DetalleOperario user = await _function.ObtenerDatosOperario();

            ViewData["DatosOperario"] = user;
            ViewData["DetalleBujiasPrestadas"] = respuesta.Detalle["bujiasEnPrestamo"];
            ViewData["TotalBujias"] = respuesta.Detalle["TotalBujias"];



            return View(respuesta.Detalle["bujiasEnAlmacen"]);
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosPrestamo([FromBody] Prestamo datosPrestamo)
        {
            Respuesta<string> respuesta = await _almacen.GuardarDatosPrestados(datosPrestamo);
            
            return Json(new { respuesta = respuesta });
        }

        public async Task<IActionResult> ControlCambio()
        {
            string cabecera = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(cabecera);

            var datosControlCambio = await Mediator.Send(new ObtenerControlBujias());

            ViewData["Datoscabecera"] = datoscabecera;
            ViewData["diccinarioEG"] = datosControlCambio.Detalle["diccinarioEG"];
            ViewData["promedioTotalPorGE"] = datosControlCambio.Detalle["promedioTotalPorGE"];

            return View(datosControlCambio.Detalle["detalleControlCambio"]);
        }
        public async Task<IActionResult> RegistroControlCambio()
        {
            string cabecera = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(cabecera);

            ViewData["Datoscabecera"] = datoscabecera;

            return View();
        }


        [HttpGet]
        public async Task<JsonResult> ObtenerControlCambio([FromHeader]string lado, [FromHeader] int fila, [FromHeader] int EG)
        {
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);
            Respuesta<List<RegistroBujias>> respuesta = await _consultaBujias.ObtenerdetalleControlCambio(lado, fila, EG,user.IdSitio);


            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDetalleDatosBujias([FromBody] List<RegistroBujias> datos)
        {
            Respuesta<string> respuestaRegistro = await _bujias.GuardarOActualizarRegisto(datos);

            Respuesta<string> actualizarComponente = await Mediator.Send(new GuardarBujiasUtilizadas());

            return Json(new { respuesta = respuestaRegistro });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosBujias([FromBody] ControlCambioData datos)
        {
            Respuesta<string> respuestaControlCambio = await _bujias.GuardarOActualizarControlCambio(datos);

            return Json(new { respuesta = respuestaControlCambio });
        }
    }
}
