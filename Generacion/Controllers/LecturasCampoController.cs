
using Microsoft.AspNetCore.Mvc;
using System;
using Generacion.Application.LecturaCampo.Query;
using Generacion.Application.LecturaCampo;
using Generacion.Models.LecturasCampo;
using Generacion.Models;
using Newtonsoft.Json;
using Generacion.Models.DatosConsola;
using Generacion.Models.Usuario;
using Generacion.Application.DataBase.cache;
using Generacion.Infraestructura;
using Generacion.Application.Usuario.Query;
using Generacion.Models.Session;

namespace Generacion.Controllers
{
    public class LecturasCampoController : ApiControllerBase
    {
        private readonly LecturaCampo _lecturaCampo;
        private readonly ILecturaCampo _datosRegistroCampo;
        private readonly CacheDatos _cacheDatos;
        private readonly ConsultarUsuario _consultarUsuario;

        public LecturasCampoController(ConsultarUsuario consultarUsuario, LecturaCampo lecturaCampo, ILecturaCampo datosRegistroCampo, CacheDatos cacheDatos)
        {
            _consultarUsuario = consultarUsuario;
            _cacheDatos = cacheDatos;
            _datosRegistroCampo = datosRegistroCampo;
            _lecturaCampo = lecturaCampo;
        }

        public async Task<IActionResult> Index([FromQuery] string fecha)
        {
            DateTime fechaActual = string.IsNullOrEmpty(fecha) ? DateTime.Now : DateTime.Parse(fecha);
            DateTime fechaMedianoche = string.IsNullOrEmpty(fecha) ? DateTime.Now : DateTime.Parse(fecha);


            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            string datoscabeceraJson = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(datoscabeceraJson);

            if (string.IsNullOrEmpty(fecha))
            {
                if (string.IsNullOrEmpty(fecha))
            {
                if (int.Parse(fechaActual.ToString("HH")) >= 0 && int.Parse(fechaActual.ToString("HH")) < 2)
                {
                    fechaActual = fechaActual.AddDays(-1);
                }
            }
            }


            Respuesta<List<TiposRegistroCampo>> tipoRegistrosCampo = await _lecturaCampo.ObtenerTiposDeRegistro();
            var datosCampo = await _lecturaCampo.ObtenerDetalleCampo(fechaActual.ToString("dd/MM/yyyy"), fechaMedianoche.ToString("dd/MM/yyyy"), user.IdSitio);

            Respuesta<string> observacion = await _lecturaCampo.ObtenerObservacionPorFecha(fechaActual.ToString("dd/MM/yy"));

            Respuesta<List<SessionOperario>> datosSessionOperarios = await _consultarUsuario.ObtenerSessionOperarios(fechaActual.ToString("dd/MM/yyyy"));
            Dictionary<string, List<SessionOperario>> horarioOperarios = datosSessionOperarios.Detalle
                                                                        .GroupBy(x => x.Horario)
                                                                            .ToDictionary(
                                                                                    outerGroup => outerGroup.Key,
                                                                                    outerGroup => outerGroup.ToList());

            ViewBag.FechaSeleccionado = fecha;
            ViewBag.Perfil = user.IdCargo;
            ViewData["horarioOperarios"] = horarioOperarios;

            ViewData["TipoRegistros"] = tipoRegistrosCampo.Detalle;
            ViewData["DatoscabeceraCampo"] = datoscabecera;
            ViewData["DatosCampo"] = datosCampo.Detalle;
            ViewData["observacion"] = observacion.Detalle ?? string.Empty;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosConsola([FromBody] List<DatosFormatoCampo> datos)
        {
            Respuesta<string> respuesta = await _datosRegistroCampo.GuardarDatosPrincipal(datos);
            return Json( new { respuesta = respuesta } );
        }

        [HttpPost]
        public async Task<JsonResult> GuardarFormato([FromBody] FormatoCampo datos)
        {
            Respuesta<string> respuesta = await _datosRegistroCampo.GuardarDatoCampo(datos);
            return Json(new { respuesta = respuesta });
        }
    }
}
