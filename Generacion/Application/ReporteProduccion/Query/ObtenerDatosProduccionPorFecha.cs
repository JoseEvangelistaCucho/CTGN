using Generacion.Application.DataBase.cache;
using DBdatosConsola = Generacion.Application.DatosConsola.Query.DatosConsola;
using DBlectruraCampó = Generacion.Application.LecturaCampo.Query.LecturaCampo;
using Generacion.Application.Funciones;
using Generacion.Application.ION.Query;
using Generacion.Controllers;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using Generacion.Models.ION;
using Generacion.Models.LecturasCampo;
using Generacion.Models.ReporteProduccion;
using Generacion.Models.Usuario;
using MediatR;
using Newtonsoft.Json;

namespace Generacion.Application.ReporteProduccion.Query
{
    public class ObtenerDatosProduccionPorFecha : IRequest<Respuesta<Dictionary<string, object>>>
    {
        public string Fecha { get; set; }
    }
    public class ObtenerDatosProduccionPorFechaHandler : IRequestHandler<ObtenerDatosProduccionPorFecha, Respuesta<Dictionary<string, object>>>
    {
        private readonly ConsultarION _consultarION;
        private readonly CacheDatos _cacheDatos;
        private readonly DBdatosConsola _datosConsola;
        private readonly IReporteProduccion _datosEnergiaProducida;
        private readonly ConsultarProduccion _consultarProduccion;
        private readonly DBlectruraCampó _lecturaCampo;
        private readonly ILogger<ReporteProduccionController> _logger;
        private readonly FotoServidor _fotoServidor;
        public ObtenerDatosProduccionPorFechaHandler(ConsultarION consultarION,
            CacheDatos cacheDatos,
            IReporteProduccion datoEnergiaProducida, DBdatosConsola datosConsola,
            ConsultarProduccion consultarProduccion, DBlectruraCampó lecturaCampo,
            ILogger<ReporteProduccionController> logger,
            FotoServidor fotoServidor)
        {
            _fotoServidor = fotoServidor;
            _lecturaCampo = lecturaCampo;
            _consultarProduccion = consultarProduccion;
            _datosConsola = datosConsola;
            _cacheDatos = cacheDatos;
            _consultarION = consultarION;
            _datosEnergiaProducida = datoEnergiaProducida;
            _logger = logger;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerDatosProduccionPorFecha request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();

            try
            {
                //string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
              //  DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);
                /*Dictionary<int, List<string>> horarioOperarios = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(_cacheDatos.ObtenerContenidoCache("HorarioOperario"));
                ViewData["horarioOperarios"] = horarioOperarios;*/
              //  Dictionary<string, Dictionary<int, List<string>>> horarioOperarios = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, List<string>>>>(_cacheDatos.ObtenerContenidoCache("HorarioOperario"));

              //  var llavesAEliminar = horarioOperarios.Keys.Where(llave => llave != user.IdSitio).ToList();

                // Eliminar las llaves que no coinciden con la validación
             /*   foreach (var llave in llavesAEliminar)
                {
                    horarioOperarios.Remove(llave);
                }
                */
                //ViewData["horarioOperarios"] = horarioOperarios[user.IdSitio];



               /* DateTime fechaActual = DateTime.Now;
                DateTime fechaMedianoche = DateTime.Now;
                if (int.Parse(fechaActual.ToString("HH")) >= 0 && int.Parse(fechaActual.ToString("HH")) < 2)
                {
                    fechaActual = fechaActual.AddDays(-1);
                }

               // string datoscabeceraJson = HttpContext.Session.GetString("datoscabecera");

                Respuesta<List<decimal>> datosRegistro = await _datosConsola.ObtenerDatosDetConsola(fechaActual.AddDays(-1).ToString("dd/MM/yyyy"), fechaActual.ToString("dd/MM/yyyy"), $"{user.IdSitio}-BFA901%", 8);
                var datosSincro = await _consultarProduccion.ObtenerNumeroArranque($"{user.IdSitio}-ELD-RPT-PROD_{fechaActual.AddDays(-1).ToString("yyyy-MM-dd")}");
                //Respuesta<List<DatosFormatoMGD>> respuesta = await _consultarION.ObtenerDatosION("09/01/23", "09/02/23"); //(fechaActual.ToString("dd/MM/yyyy"));
                string fechaFrontend = "09/09/2023";
                TimeSpan horaEspecifica = new TimeSpan(5, 0, 0);
                DateTime fechaHoraEspecifica = DateTime.Now;
                if (DateTime.TryParseExact(fechaFrontend, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fechaParseada))
                {
                    fechaHoraEspecifica = fechaParseada.Date + horaEspecifica;
                }
                Respuesta<List<DatosFormatoMGD>> respuestaIon = await _consultarION.ObtenerDatosIONSQL("441", "12889,12890,12891,12892,12893,12894,537,540", fechaHoraEspecifica);
                //Respuesta<ReporteDiarioMantenimiento> horasGen = await _datosMantenimiento.ObtenerDatosMGD(fechaActual.ToString("dd/MM/yy"));
                Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>> detalleCampoOperacionHoy = await _lecturaCampo.ObtenerDetalleCampoPorFecha("HrsOperacion,Operacion_ci,NivelCarter", 24, fechaActual.ToString("dd/MM/yy"));
                Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>> detalleCampoOperacionAyer = await _lecturaCampo.ObtenerDetalleCampoPorFecha("HrsOperacion,Operacion_ci,NivelCarter", 24, fechaActual.AddDays(-1).ToString("dd/MM/yy"));
                Respuesta<Dictionary<string, List<RegistrosDatosEngine>>> datosEngine = await _datosConsola.ObtenerDatosEngine(fechaActual.ToString("dd/MM/yyyy"), fechaMedianoche.ToString("dd/MM/yyyy"), user.IdSitio);


                Respuesta<List<EnergiaProducida>> datosProduccion = await _consultarProduccion.ObtenerRegistroProduccion(fechaActual.ToString("dd_MM_yyyy"));
                Respuesta<List<LevelLubeOilCartel>> datosProduccionCarter = await _consultarProduccion.ObtenerRegistroLevelCartel(fechaActual.ToString("dd_MM_yyyy"));
                Respuesta<List<CityGateFlow>> datosProduccionCity = await _consultarProduccion.ObtenerRegistroCityGate(fechaActual.ToString("dd_MM_yyyy"));
                //  Respuesta<List<TkCleanLube>> datosProduccionTkClean = await _consultarProduccion.ObtenerRegistroTkClean(fechaActual.ToString("dd_MM_yyyy"));
                Respuesta<Dictionary<string, TkCleanLube>> datosProduccionTkClean = await _consultarProduccion.ObtenerRegistroTkCleanPorTipo();
                Respuesta<Dictionary<string, ManttoVessel>> datosMantoVessel = await _consultarProduccion.ObtenerMantoTKVessel(fechaActual.ToString("dd_MM_yyyy"));

                */

            }
            catch (Exception ex)
            {



            }

            return respuesta;
        }
    }
}
