using Generacion.Application.ION.Query;
using Generacion.Models.ION;
using Generacion.Models;
using Microsoft.AspNetCore.Mvc;
using Generacion.Models.Usuario;
using Generacion.Application.DataBase.cache;
using Newtonsoft.Json;
using Generacion.Application.ReporteProduccion;
using Generacion.Models.ReporteProduccion;
using Generacion.Application.DatosConsola.Query;
using Generacion.Models.DatosConsola;
using Generacion.Application.ReporteProduccion.Query;
using Generacion.Application.LecturaCampo.Query;
using Generacion.Models.LecturasCampo;
using Generacion.Infraestructura;
using Generacion.Application.Funciones;
using Generacion.Application.Usuario.Query;
using Generacion.Models.Session;
using Stimulsoft.Report;
using System.Data;
using Generacion.Application.PruebasSemanales.BackStart.Command;
using Generacion.Application.PruebasSemanales.BackStart.Query;
using Generacion.Models.PruebasSemanales.BlackStart;

namespace Generacion.Controllers
{
    public class ReporteProduccionController : ApiControllerBase
    {
        private readonly ConsultarION _consultarION;
        private readonly CacheDatos _cacheDatos;
        private readonly DatosConsola _datosConsola;
        private readonly IReporteProduccion _datosEnergiaProducida;
        private readonly ConsultarProduccion _consultarProduccion;
        private readonly LecturaCampo _lecturaCampo;
        private readonly ILogger<ReporteProduccionController> _logger;
        private readonly ConsultarUsuario _consultarUsuario;
        private readonly FotoServidor _fotoServidor;
        private readonly Function _function;
        private readonly IConfiguration _configuration;
        private readonly ObtenerRegistrosPruebaSemanal _registrosPruebaSemanal;

        private DateTime _fechaActual = new DateTime(2025, 2, 2, 0, 30, 0); //  DateTime.Now;  //
        public ReporteProduccionController(ConsultarION consultarION,
            IConfiguration configuration,
            Function function,
            CacheDatos cacheDatos,
            IReporteProduccion datoEnergiaProducida, DatosConsola datosConsola,
            ConsultarProduccion consultarProduccion, LecturaCampo lecturaCampo,
            ILogger<ReporteProduccionController> logger,
            FotoServidor fotoServidor, ConsultarUsuario consultarUsuario,
            ObtenerRegistrosPruebaSemanal registrosPruebaSemanal)
        {
            _registrosPruebaSemanal = registrosPruebaSemanal;
            _function = function;
            _configuration = configuration;
            _fotoServidor = fotoServidor;
            _lecturaCampo = lecturaCampo;
            _consultarProduccion = consultarProduccion;
            _datosConsola = datosConsola;
            _cacheDatos = cacheDatos;
            _consultarION = consultarION;
            _datosEnergiaProducida = datoEnergiaProducida;
            _logger = logger;
            _consultarUsuario = consultarUsuario;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
                DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

                DateTime fechaActual = _fechaActual;//  DateTime.Now; // string.IsNullOrEmpty(fecha) ? DateTime.Now : DateTime.Parse(fecha);

                Respuesta <List<SessionOperario>> datosSessionOperarios = await _consultarUsuario.ObtenerSessionOperarios(fechaActual.ToString("dd/MM/yyyy"));


                Dictionary<string, List<SessionOperario>> horarioOperarios = datosSessionOperarios.Detalle
                                                                            .GroupBy(x => x.Horario)
                                                                                .ToDictionary(
                                                                                        outerGroup => outerGroup.Key,
                                                                                        outerGroup => outerGroup.ToList());

                ViewData["horarioOperarios"] = horarioOperarios;

                DateTime fechaMedianoche = DateTime.Now;
                if (int.Parse(fechaActual.ToString("HH")) >= 0 && int.Parse(fechaActual.ToString("HH")) < 2)
                {
                    fechaActual = fechaActual.AddDays(-1);
                }
                ViewBag.Fecha = fechaActual;

                string datoscabeceraJson = HttpContext.Session.GetString("datoscabecera");

                Respuesta<List<decimal>> datosRegistro = await _datosConsola.ObtenerDatosDetConsola(fechaActual.AddDays(-1).ToString("dd/MM/yyyy"), fechaActual.ToString("dd/MM/yyyy"), $"{user.IdSitio}-BFA901%", 8);
             
                Respuesta<List<DatosFormatoMGD>> respuesta = await Mediator.Send(new ObtenerDatosIONQuery()
                {
                    Fecha = fechaActual
                });

                /***********************************************/

                Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>> detalleCampoOperacionHoy = await _lecturaCampo.ObtenerDetalleCampoPorFecha("HrsOperacion,Operacion_ci,NivelCarter", 24, fechaActual.ToString("dd/MM/yy"));
                Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>> detalleCampoOperacionAyer = await _lecturaCampo.ObtenerDetalleCampoPorFecha("HrsOperacion,Operacion_ci,NivelCarter", 24, fechaActual.AddDays(-1).ToString("dd/MM/yy"));
                Respuesta<Dictionary<string, List<RegistrosDatosEngine>>> datosEngine = await _datosConsola.ObtenerDatosEngine(fechaActual.ToString("dd/MM/yyyy"), fechaMedianoche.ToString("dd/MM/yyyy"), user.IdSitio);

                Respuesta<List<EnergiaProducida>> datosProduccion = await _consultarProduccion.ObtenerRegistroProduccion(fechaActual.AddDays(-1).ToString("dd/MM/yyyy"),"READING TODAY","dia");
                Respuesta<List<LevelLubeOilCartel>> datosProduccionCarter = await _consultarProduccion.ObtenerRegistroLevelCartel(fechaActual.AddDays(-1).ToString("dd_MM_yyyy"),"dia","TODAY");
                Respuesta<List<CityGateFlow>> datosProduccionCity = await _consultarProduccion.ObtenerRegistroCityGate(fechaActual.AddDays(-1).ToString("dd_MM_yyyy"),"dia", "READING TODAY");
                Respuesta<Dictionary<string, Dictionary<string, TkCleanLube>>>  datosProduccionTkClean = await _consultarProduccion.ObtenerRegistroTkCleanPorTipo(fechaActual.AddDays(-1).ToString("dd_MM_yyyy"));
                Respuesta<Dictionary<string, ManttoVessel>> datosMantoVessel = await _consultarProduccion.ObtenerMantoTKVessel(fechaActual.ToString("dd_MM_yyyy"));
                Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(datoscabeceraJson);

                Respuesta<List<ReporteProducion>> datosProduccionAnt = await _consultarProduccion.ObtenerDatosProduccion(fechaActual.AddDays(-1).ToString("dd/MM/yy"), "dia");

                string idPruebaSemanal = user.IdSitio+"_SISTEMAINCENDIOS-"+ fechaActual.AddDays(-1).ToString("dd_MM_yyyy");
                Respuesta<List<DetallePruebaSemanal>> detallePruebaSemanal = await _registrosPruebaSemanal.ObtenerDetallePruebaSemanal(idPruebaSemanal);

                ViewBag.FechaSeleccionado = fechaActual.ToString("dd/MM/yyyy");

                ViewData["detallePruebaSemanal"] = detallePruebaSemanal.Detalle.Where(x => x.IdSubtitulo.Equals("NivelSCIBD")).Select(x => x.DetalleNumerico).FirstOrDefault(); //ReporteProducion
                ViewData["datosProduccionAnt"] = datosProduccionAnt.Detalle.FirstOrDefault(); //ReporteProducion
                ViewData["DatoscabeceraCampo"] = datoscabecera;
                ViewData["Produccion"] = datosProduccion.Detalle;
                ViewData["ProduccionCarter"] = datosProduccionCarter.Detalle;
                ViewData["ProduccionCity"] = datosProduccionCity.Detalle;
                ViewData["ProduccionTkClean"] = datosProduccionTkClean.Detalle;
                ViewData["datosMantoVessel"] = datosMantoVessel.Detalle;

                ViewData["DetalleOperacionCampoHoy"] = detalleCampoOperacionHoy.Detalle;
                ViewData["DetalleOperacionCampoAyer"] = detalleCampoOperacionAyer.Detalle;

                ViewData["DatosGraficoION"] = respuesta.Detalle
                    .OrderBy(x => x.Hora == "00:00" ? TimeSpan.FromHours(24) : TimeSpan.Parse(x.Hora))
                    .ToList();

                ViewData["datosEngine"] = datosEngine.Detalle;
                ViewData["datosBFA901"] = datosRegistro.Detalle;

                var datosGeneralesProduccion = await Mediator.Send(new ObtenerDatosProduccion()
                {
                    Fecha = fechaActual
                });
                ViewData["DatosSincro"] = datosGeneralesProduccion.Detalle["DatosSincro"]; //este valor a veces trae problemas
                ViewData["datosLecturas"] = datosGeneralesProduccion.Detalle["datosLecturas"];

            }
            catch (Exception ex)
            {
                _logger.LogError("ReporteProduccionController  Error : {ErrorDetails}", ex.ToString());
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosEnergiaProducida([FromBody] EnergiaProducida datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosEnergiaProducida(datos);
            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDetalleProduccion([FromBody] Produccion datos)
        {
            Respuesta<string> respuesta = null;
            try
            {
                if (datos != null)
                {
                    int respuestaProduccion = await _datosEnergiaProducida.InsertOrUpdateProduccionStatus(datos.ReporteProduccionStatus);
                    var respuestaDetalle = await _datosEnergiaProducida.InsertOrUpdateDetalleProduccion(datos.DetalleProduccion);
                    var respuestaSincro = await _datosEnergiaProducida.InsertOrUpdateArranqueSincro(datos.ArranqueSincronizacion);
                    var respuestaDatosReporte = await _datosEnergiaProducida.GuardarDatosReporteProduccion(datos.DatosGeneralesroduccion);
                    respuesta = await _datosEnergiaProducida.InsertOrUpdateRegistroEventos(datos.RegistroEventos);
                }
            }catch(Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar";
            }

            return Json(new { respuesta = respuesta });
        }
        [HttpPost]
        public async Task<JsonResult> GuardarDatosLevelOilCarter([FromBody] LevelLubeOilCartel datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosLevelOilCarter(datos);
            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosRefCarter([FromBody] RefrescamientoCartel datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosRefCarter(datos);
            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosCityGate([FromBody] CityGateFlow datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosCityGate(datos);
            return Json(new { respuesta = respuesta });
        }
        [HttpPost]
        public async Task<JsonResult> GuardarDatosManttoVessel([FromBody] ManttoVessel datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosManttoVessel(datos);
            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosTkCleanLube([FromBody] TkCleanLube datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            if (datos != null)
            {
                respuesta = await _datosEnergiaProducida.GuardarDatosTkCleanLube(datos);
            }
            return Json(new { respuesta = respuesta });
        }
        //[HttpPost]
        //public async Task<JsonResult> GuardarReporteProduccion([FromBody] ReporteProducion datos)
        //{
        //    Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosReporteProduccion(datos);
        //    return Json(new { respuesta = respuesta });
        //}

        [HttpPost]
        public async Task<JsonResult> GuardarDetalleSemanal([FromBody] GuardarDatosPruebaSemanal datos)
        {
            var respuesta = await Mediator.Send(datos);

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task GuardarDatosReporteJson([FromBody] DatosReporteProduccion datos)
        {

            try
            {
                IConfigurationSection rutaReporte = _configuration.GetSection("RutaReportes");
                var nombreReporte = "Report-Produccion.mrt";

                var report = new StiReport();
                report.Load(Path.Combine(rutaReporte["pathReporteMRT"], nombreReporte));

                /*DataSet dataSet2 = await CrearOperarios(datos);
                report.RegData("ReporteProduccion2", dataSet2); */

                DataSet dataSet = await CrearDataSet(datos);

                report.Dictionary.Clear();
                report.RegData(dataSet);

                await report.Dictionary.SynchronizeAsync();
                await report.RenderAsync();

                report.ExportDocument(StiExportFormat.Pdf, "C:\\aplicaciones\\PortalGeneracion\\Reportes\\Centrales\\PRUEBAS\\Report-Produccion.pdf");

            }
            catch (Exception ex)
            {

            }

            // _fotoServidor.GuardarJson(datos, "Reporte-Produccion.json", this, "Reporte-Produccion");
        }
        private async Task<DataSet> CrearDataSet(DatosReporteProduccion datos)
        {
            DateTime fechaActual = _fechaActual; //  DateTime.Now; // string.IsNullOrEmpty(fecha) ? DateTime.Now : DateTime.Parse(fecha);
            var datosOperario = await _function.ObtenerDatosOperario();

            Respuesta<List<decimal>> datosRegistro = await _datosConsola.ObtenerDatosDetConsola(fechaActual.AddDays(-1).ToString("dd/MM/yyyy"), fechaActual.ToString("dd/MM/yyyy"), $"{datosOperario.IdSitio}-BFA901%", 8);
            Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>> detalleCampoOperacionHoy = await _lecturaCampo.ObtenerDetalleCampoPorFecha("HrsOperacion,Operacion_ci,NivelCarter", 24, fechaActual.ToString("dd/MM/yy"));
            Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>> detalleCampoOperacionAyer = await _lecturaCampo.ObtenerDetalleCampoPorFecha("HrsOperacion,Operacion_ci,NivelCarter", 24, fechaActual.AddDays(-1).ToString("dd/MM/yy"));


            DataSet dataSet = new DataSet("ReporteProduccion2");
            
            DataTable[] dataTable = new DataTable[]{
                await CrearDatosGenerales(datos),
                await CrearOperarios(datos),
                await CrearStatus(datos),
                await CrearTotals(datos),
                await CrearEnergyProduce(datos),
                await ObtenerNumeroDeArranque(datos),
                await ObtenerNumeroSincronizaciones(datos),
                await ObtenerOilCarter(datos),
                await ObtenerInternalService(datosRegistro.Detalle),
                await ObtenerRefrescamientoCarter(datos),
                await ObtenerGateFlow(datos),
                await ObtenerTkCleanLubeOil(datos),
                await ObtenerCompresors(detalleCampoOperacionHoy.Detalle,detalleCampoOperacionAyer.Detalle),
                await ObtenerTkOilUsed(datos),
                await ObtenerFuelLevelBlack()
            }; 
            dataSet.Tables.AddRange(dataTable);

            return dataSet;
        }


        private async Task<DataTable> ObtenerFuelLevelBlack()
        {
            DataTable dataTable = new DataTable("FuelLevelBlackStart");

            dataTable.Columns.Add("Yesterday", typeof(string));
            dataTable.Columns.Add("Today", typeof(string));



            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();
            fila["Yesterday"] = "";
            fila["Today"] = "";



            dataTable.Rows.Add(fila);

            // Devolver el DataSet creado
            return dataTable;
        }


        private async Task<DataTable> ObtenerTkCleanLubeOil(DatosReporteProduccion datos)
        {
            DataTable dataTable = new DataTable("TkCleanLubeOil");

            Dictionary<string, TkCleanLube> lube = datos.TkCleanLubeOil.ToDictionary(x => x.Tipo);

            //dataTable.Columns.Add("TKCLEANLUBEOIL", typeof(string));
            dataTable.Columns.Add("Yesterday", typeof(string));
            dataTable.Columns.Add("Today", typeof(string));
            dataTable.Columns.Add("Diference", typeof(string));
            dataTable.Columns.Add("Received", typeof(string));
            dataTable.Columns.Add("ContadorToday", typeof(string));
            dataTable.Columns.Add("ContadorYesterday", typeof(string));
            dataTable.Columns.Add("AdicionOil", typeof(string));

            dataTable.Columns.Add("Yesterday2", typeof(string));
            dataTable.Columns.Add("Today2", typeof(string));
            dataTable.Columns.Add("Diference2", typeof(string));
            dataTable.Columns.Add("Received2", typeof(string));

            DataRow fila = dataTable.NewRow();

            //fila["TKCLEANLUBEOIL"] = "";
            fila["Yesterday"] = lube["YESTERDAY"].TkLevel; 
            fila["Today"] = lube["TODAY"].TkLevel;
            fila["DIFERENCE"] = lube["DIFERENCE"].TkLevel;
            fila["Received"] = lube["RECEIVED"].TkLevel;
            fila["ContadorToday"] = lube["CONTADOR TODAY"].TkLevel;
            fila["ContadorYesterday"] = lube["CONTADOR YESTERDAY"].TkLevel;
            fila["AdicionOil"] = lube["ADICION OIL"].TkLevel;

            fila["Yesterday2"] = lube["YESTERDAY"].TkRead;
            fila["Today2"] = lube["TODAY"].TkRead;
            fila["Diference2"] = lube["DIFERENCE"].TkRead;
            fila["Received2"] = lube["RECEIVED"].TkRead;

            dataTable.Rows.Add(fila);

            return dataTable;
        }

        private async Task<DetalleFecha>  ValidarcionValorDecimal(Dictionary<int, Dictionary<string , DetalleFecha>> valor, int key, string busqueda)
        {
            DetalleFecha respuesta = new DetalleFecha();
            if (valor.ContainsKey(key))
            {
                respuesta = valor[key][busqueda];
            }
            return respuesta;
        }

        private async Task<DataTable> ObtenerCompresors(Dictionary<int, Dictionary<string, DetalleFecha>> hoy , Dictionary<int, Dictionary<string, DetalleFecha>> ayer)
        {
            var hoyHrsOpe1 = await ValidarcionValorDecimal(hoy, 1, "HrsOperacion");
            var hoyHrsOpe2 = await ValidarcionValorDecimal(hoy, 2, "HrsOperacion");
            var hoyOpe1 = await ValidarcionValorDecimal(hoy, 1, "Operacion_ci");
            var hoyOpe2 = await ValidarcionValorDecimal(hoy, 2, "Operacion_ci");

            var ayerHrsOpe1 = await ValidarcionValorDecimal(ayer, 1, "HrsOperacion");
            var ayerHrsOpe2 = await ValidarcionValorDecimal(ayer, 2, "HrsOperacion");
            var ayerOpe1 = await ValidarcionValorDecimal(ayer, 1, "Operacion_ci");
            var ayerOpe2 = await ValidarcionValorDecimal(ayer, 2, "Operacion_ci");


            DataTable dataTable = new DataTable("Compresors");

            dataTable.Columns.Add("HrsToday", typeof(string));
            dataTable.Columns.Add("HrsToday2", typeof(string));
            dataTable.Columns.Add("HrsToday3", typeof(string));
            dataTable.Columns.Add("HrsToday4", typeof(string));

            dataTable.Columns.Add("HrsYester", typeof(string));
            dataTable.Columns.Add("HrsYester2", typeof(string));
            dataTable.Columns.Add("HrsYester3", typeof(string));
            dataTable.Columns.Add("HrsYester4", typeof(string));

            dataTable.Columns.Add("Diference", typeof(string));
            dataTable.Columns.Add("Diference2", typeof(string));
            dataTable.Columns.Add("Diference3", typeof(string));
            dataTable.Columns.Add("Diference4", typeof(string));


            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();
            fila["HrsToday"] = hoyHrsOpe1.detalle;
            fila["HrsToday2"] = hoyHrsOpe2.detalle;
            fila["HrsToday3"] = hoyOpe1.detalle;
            fila["HrsToday4"] = hoyOpe2.detalle;

            fila["HrsYester"] = ayerHrsOpe1.detalle;
            fila["HrsYester2"] = ayerHrsOpe2.detalle;
            fila["HrsYester3"] = ayerOpe1.detalle;
            fila["HrsYester4"] = ayerOpe2.detalle;

            fila["Diference"] = hoyHrsOpe1.detalle- ayerHrsOpe1.detalle;
            fila["Diference2"] = hoyHrsOpe2.detalle - ayerHrsOpe2.detalle;
            fila["Diference3"] = hoyOpe1.detalle - ayerOpe1.detalle;
            fila["Diference4"] = hoyOpe2.detalle - ayerOpe2.detalle;


            dataTable.Rows.Add(fila);

            // Devolver el DataSet creado
            return dataTable;
        }

        private async Task<DataTable> ObtenerTkOilUsed(DatosReporteProduccion datos)
        {
            Dictionary<string, TkCleanLube> lube = datos.TkCleanLubeUsed.ToDictionary(x => x.Tipo);

            DataTable dataTable = new DataTable("TkLubeOilUsed");

            dataTable.Columns.Add("Yesterday", typeof(string));
            dataTable.Columns.Add("Today", typeof(string));
            dataTable.Columns.Add("Consumed", typeof(string));
            dataTable.Columns.Add("Received", typeof(string));

            dataTable.Columns.Add("Yesterday2", typeof(string));
            dataTable.Columns.Add("Today2", typeof(string));
            dataTable.Columns.Add("Consumed2", typeof(string));
            dataTable.Columns.Add("Received2", typeof(string));


            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();

            fila["Yesterday"] = lube["YESTERDAY"].TkLevel;
            fila["Today"] = lube["TODAY"].TkLevel;
            fila["Consumed"] = lube["CONSUMED"].TkLevel;
            fila["Received"] = lube["RECEIVED"].TkLevel;

            fila["Yesterday2"] = lube["YESTERDAY"].TkRead;
            fila["Today2"] = lube["TODAY"].TkRead;
            fila["Consumed2"] = lube["CONSUMED"].TkRead;
            fila["Received2"] = lube["RECEIVED"].TkRead;

            dataTable.Rows.Add(fila);

            // Devolver el DataSet creado
            return dataTable;
        }


        private async Task<DataTable> ObtenerInternalService(List<decimal> datos)
        {
            DataTable dataTable = new DataTable("InternalService");



            dataTable.Columns.Add("yesterday", typeof(string));
            dataTable.Columns.Add("today", typeof(string));
            dataTable.Columns.Add("total", typeof(string));

            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();
            fila["yesterday"] = datos[0];
            fila["today"] = datos[1];
            fila["total"] = datos[1] - datos[0];

            dataTable.Rows.Add(fila);

            // Devolver el DataSet creado
            return dataTable;
        }

        private async Task<DataTable> ObtenerRefrescamientoCarter(DatosReporteProduccion datos)
        {
            DataTable dataTable = new DataTable("RefrescamientoDelCarter");

            Dictionary<string, RefrescamientoCartel> refrescamiento = datos.RefrescamientoCartel.ToDictionary(x => x.TipoRefrescamiento);


            dataTable.Columns.Add("extraer", typeof(string));
            dataTable.Columns.Add("rellenar", typeof(string));
            dataTable.Columns.Add("extraer2", typeof(string));
            dataTable.Columns.Add("rellenar2", typeof(string));
            dataTable.Columns.Add("extraer3", typeof(string));
            dataTable.Columns.Add("rellenar3", typeof(string));

            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();
            fila["extraer"] = refrescamiento["SE RELLENA"].Generador1; 
            fila["rellenar"] = refrescamiento["SE EXTRAE"].Generador1;
            fila["extraer2"] = refrescamiento["SE EXTRAE"].Generador2;
            fila["rellenar2"] = refrescamiento["SE RELLENA"].Generador2;
            fila["extraer3"] = refrescamiento["SE EXTRAE"].TotalRefrescamiento;
            fila["rellenar3"] = refrescamiento["SE RELLENA"].TotalRefrescamiento;


            dataTable.Rows.Add(fila);

            // Devolver el DataSet creado
            return dataTable;
        }


        private async Task<DataTable> ObtenerGateFlow(DatosReporteProduccion datos)
        {
            DataTable dataTable = new DataTable("GateFlowMeter");

            Dictionary<string, CityGateFlow> valores = datos.CityGateFlow.ToDictionary(x => x.Tipo);

            dataTable.Columns.Add("ReadingToday", typeof(string));
            dataTable.Columns.Add("ReadingYesterday", typeof(string));
            dataTable.Columns.Add("Consumption", typeof(string));

            dataTable.Columns.Add("ReadingToday2", typeof(string));
            dataTable.Columns.Add("ReadingYesterday2", typeof(string));
            dataTable.Columns.Add("Consumption2", typeof(string));
            //dataTable.Columns.Add("TKMantto(cms)", typeof(string));
            //dataTable.Columns.Add("TKVessel(cms)", typeof(string));

            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();
            fila["ReadingToday"] = valores["READING TODAY"].KgEng1; 
            fila["ReadingYesterday"] = valores["READING YESTERDAY"].KgEng1;
            fila["Consumption"] = valores["CONSUMPTION"].KgEng1;

            fila["ReadingToday2"] = valores["READING TODAY"].KgEng2;
            fila["ReadingYesterday2"] = valores["READING YESTERDAY"].KgEng2;
            fila["Consumption2"] = valores["CONSUMPTION"].KgEng2;
            //fila["TKMantto(cms)"] = "";
            //fila["TKVessel(cms)"] = "";


            dataTable.Rows.Add(fila);

            // Devolver el DataSet creado
            return dataTable;
        }

        private async Task<DataTable> ObtenerOilCarter(DatosReporteProduccion datos)
        {
            DataTable dataTable = new DataTable("LevelLubeOilCarter");

            Dictionary<string, LevelLubeOilCartel> lube = datos.LevelLubeOilCartel.ToDictionary(x => x.TipoCarter);

            dataTable.Columns.Add("yesterday", typeof(string));
            dataTable.Columns.Add("today", typeof(string));
            dataTable.Columns.Add("added", typeof(string));
            dataTable.Columns.Add("yesterday2", typeof(string));
            dataTable.Columns.Add("today2", typeof(string));
            dataTable.Columns.Add("added2", typeof(string));
            dataTable.Columns.Add("total", typeof(string));

            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();
            fila["yesterday"] = lube["YESTERDAY"].Generador1;
            fila["today"] = lube["TODAY"].Generador1;
            fila["added"] = lube["ADDED"].Generador1;
            fila["yesterday2"] = lube["YESTERDAY"].Generador2;
            fila["today2"] = lube["TODAY"].Generador2;
            fila["added2"] = lube["ADDED"].Generador2;
            fila["total"] = lube["ADDED"].TotalAdded;

            dataTable.Rows.Add(fila);

            // Devolver el DataSet creado
            return dataTable;
        }
        private async Task<DataTable> ObtenerNumeroSincronizaciones(DatosReporteProduccion datos)
        {
            DataTable dataTable = new DataTable("NumeroSincronizaciones");

            Dictionary<string, Dictionary<int, ArranqueSincronizacion>> sincro = datos.Produccion.ArranqueSincronizacion
               .GroupBy(x => x.Tipo)
               .ToDictionary(
                   g => g.Key,
                   g => g.ToDictionary(a => a.NumeroGenerador, a => a)
               );

            dataTable.Columns.Add("diario", typeof(string));
            dataTable.Columns.Add("mensual", typeof(string));
            dataTable.Columns.Add("anual", typeof(string));
            dataTable.Columns.Add("diario2", typeof(string));
            dataTable.Columns.Add("mensual2", typeof(string));
            dataTable.Columns.Add("anual2", typeof(string));

            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();
            fila["diario"] = sincro["sincronizacion"][1].Diario;
            fila["mensual"] = sincro["sincronizacion"][1].Mensual;
            fila["anual"] = sincro["sincronizacion"][1].Anual;


            fila["diario2"] = sincro["sincronizacion"][2].Diario;
            fila["mensual2"] = sincro["sincronizacion"][2].Mensual;
            fila["anual2"] = sincro["sincronizacion"][2].Anual;

            dataTable.Rows.Add(fila);

            // Devolver el DataSet creado
            return dataTable;
        }
        private async Task<DataTable> ObtenerNumeroDeArranque(DatosReporteProduccion datos)
        {
            DataTable dataTable = new DataTable("Arranques");

            Dictionary<string, Dictionary<int, ArranqueSincronizacion>> arranque = datos.Produccion.ArranqueSincronizacion
                .GroupBy(x => x.Tipo) 
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(a => a.NumeroGenerador, a => a) 
                );


            dataTable.Columns.Add("diario", typeof(string));
            dataTable.Columns.Add("mensual", typeof(string));
            dataTable.Columns.Add("anual", typeof(string));
            dataTable.Columns.Add("diario2", typeof(string));
            dataTable.Columns.Add("mensual2", typeof(string));
            dataTable.Columns.Add("anual2", typeof(string));

            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();
            fila["diario"] = arranque["sincronizacion"][1].Diario;
            fila["mensual"] = arranque["sincronizacion"][1].Mensual;
            fila["anual"] = arranque["sincronizacion"][1].Anual;


            fila["diario2"] = arranque["sincronizacion"][2].Diario;
            fila["mensual2"] = arranque["sincronizacion"][2].Mensual;
            fila["anual2"] = arranque["sincronizacion"][2].Anual;

            dataTable.Rows.Add(fila);

            // Devolver el DataSet creado
            return dataTable;
        }
        private async Task<DataTable> CrearDatosGenerales(DatosReporteProduccion datos)
        {
            var datosOperario = await _function.ObtenerDatosOperario();

            DataTable dataTable = new DataTable("Datos");

            string[] stringsFecha = datos.Produccion.DatosGeneralesroduccion.Fecha.Split('/');

            dataTable.Columns.Add("Sitio", typeof(string));
            dataTable.Columns.Add("Dia", typeof(string));
            dataTable.Columns.Add("Mes", typeof(string));
            dataTable.Columns.Add("Año", typeof(string));
            dataTable.Columns.Add("Manager", typeof(string));
            dataTable.Columns.Add("Safety", typeof(string));
            dataTable.Columns.Add("HumanResources", typeof(string));

            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();
            fila["Sitio"] = datosOperario.IdSitio;
            fila["Dia"] = stringsFecha[0];
            fila["Mes"] = stringsFecha[1];
            fila["Año"] = stringsFecha[2];
            fila["Manager"] = datos.Produccion.DetalleProduccion.Where(x => x.Tipo.Equals("manager")).Select(x => x.Detalle.Replace("|#", " ")).FirstOrDefault();
            fila["Safety"] = datos.Produccion.DetalleProduccion.Where(x => x.Tipo.Equals("safety")).Select(x => x.Detalle).FirstOrDefault();
            fila["HumanResources"] = datos.Produccion.DetalleProduccion.Where(x => x.Tipo.Equals("humanResources")).Select(x => x.Detalle).FirstOrDefault();


            dataTable.Rows.Add(fila);

            // Devolver el DataSet creado
            return dataTable;
        }
        private async Task<DataTable> CrearOperarios(DatosReporteProduccion datos)
        {
            var datosOperario = await _function.ObtenerDatosOperario();

            DataTable dataTable = new DataTable("Operarios");

            dataTable.Columns.Add("Operario1", typeof(string));
            dataTable.Columns.Add("Operario2", typeof(string));
            dataTable.Columns.Add("Operario3", typeof(string));
            dataTable.Columns.Add("Operario4", typeof(string));
            dataTable.Columns.Add("Operario5", typeof(string));
            dataTable.Columns.Add("Operario6", typeof(string));

            string[] stringsOperarios = datos.Produccion.DatosGeneralesroduccion.Operadores.Split("|#");

            DataRow fila = dataTable.NewRow();
            fila["Operario1"] = stringsOperarios[0].Trim().Contains(",") ? stringsOperarios[0].Split(',')[0] : string.Empty;
            fila["Operario2"] = stringsOperarios[0].Trim().Contains(",") ? stringsOperarios[0].Split(',')[1] : string.Empty;
            fila["Operario3"] = stringsOperarios[1].Trim().Contains(",") ? stringsOperarios[1].Split(',')[0] : string.Empty;
            fila["Operario4"] = stringsOperarios[1].Trim().Contains(",") ? stringsOperarios[1].Split(',')[1] : string.Empty;
            fila["Operario5"] = stringsOperarios[2].Trim().Contains(",") ? stringsOperarios[2].Split(',')[0] : string.Empty;
            fila["Operario6"] = stringsOperarios[2].Trim().Contains(",") ? stringsOperarios[2].Split(',')[1] : string.Empty;

            dataTable.Rows.Add(fila);

            return dataTable;
        }

        private async Task<DataTable> CrearTotals(DatosReporteProduccion datos)
        {

            DataTable dataTable = new DataTable("DetalleTotals");

            dataTable.Columns.Add("TotalPlantGen", typeof(string));
            dataTable.Columns.Add("TotalPlantExportIon", typeof(string));
            dataTable.Columns.Add("TotalGasConsumption", typeof(string));
            dataTable.Columns.Add("Efficiency", typeof(string));
            dataTable.Columns.Add("ConsumoServiciosAuxiliares", typeof(string));


            DataRow fila = dataTable.NewRow();
            fila["TotalPlantGen"] = datos.Produccion.DatosGeneralesroduccion.TotalPlantGen;
            fila["TotalPlantExportIon"] = datos.Produccion.DatosGeneralesroduccion.TotalPlantExportION;
            fila["TotalGasConsumption"] = datos.Produccion.DatosGeneralesroduccion.TotalGasConsumption;
            fila["Efficiency"] = datos.Produccion.DatosGeneralesroduccion.Efficiency;
            fila["ConsumoServiciosAuxiliares"] = datos.Produccion.DatosGeneralesroduccion.ConsumoServiciosAuxiliares;

            dataTable.Rows.Add(fila);

            return dataTable;
        }

        private async Task<DataTable> CrearEnergyProduce(DatosReporteProduccion datos)
        {

            DataTable dataTable = new DataTable("EnergyProduce");

            dataTable.Columns.Add("ReadingToday1", typeof(string));
            dataTable.Columns.Add("ReadingToday2", typeof(string));
            dataTable.Columns.Add("ReadingYesterday1", typeof(string));
            dataTable.Columns.Add("ReadingYesterday2", typeof(string));
            dataTable.Columns.Add("EnergyProduced1", typeof(string));
            dataTable.Columns.Add("EnergyProduced2", typeof(string));
            dataTable.Columns.Add("AvailabilityFactor1", typeof(string));
            dataTable.Columns.Add("AvailabilityFactor2", typeof(string));
            dataTable.Columns.Add("Efficiency1", typeof(string));
            dataTable.Columns.Add("Efficiency2", typeof(string));
            dataTable.Columns.Add("UtilizationFactor1", typeof(string));
            dataTable.Columns.Add("UtilizationFactor2", typeof(string));
            dataTable.Columns.Add("CapacityFactor1", typeof(string));
            dataTable.Columns.Add("CapacityFactor2", typeof(string));
            dataTable.Columns.Add("LoadFactor1", typeof(string));
            dataTable.Columns.Add("LoadFactor2", typeof(string));
            dataTable.Columns.Add("HeatRate1", typeof(string));
            dataTable.Columns.Add("HeatRate2", typeof(string));
            dataTable.Columns.Add("GasConsumed1", typeof(string));
            dataTable.Columns.Add("GasConsumed2", typeof(string));
            dataTable.Columns.Add("Plant", typeof(string));


            DataRow fila = dataTable.NewRow();
            fila["ReadingToday1"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("READING TODAY")).Select(x => x.PmuEng_01).FirstOrDefault();
            fila["ReadingToday2"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("READING TODAY")).Select(x => x.PmuEng_02).FirstOrDefault();
            fila["ReadingYesterday1"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("READING YESTERDAY")).Select(x => x.PmuEng_01).FirstOrDefault();
            fila["ReadingYesterday2"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("READING YESTERDAY")).Select(x => x.PmuEng_02).FirstOrDefault();
            fila["EnergyProduced1"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("ENERGY PRODUCED")).Select(x => x.PmuEng_01).FirstOrDefault();
            fila["EnergyProduced2"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("ENERGY PRODUCED")).Select(x => x.PmuEng_02).FirstOrDefault();
            fila["AvailabilityFactor1"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("AVAILABILITY FACTOR")).Select(x => x.PmuEng_01).FirstOrDefault();
            fila["AvailabilityFactor2"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("AVAILABILITY FACTOR")).Select(x => x.PmuEng_02).FirstOrDefault();
            fila["Efficiency1"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("EFFICIENCY")).Select(x => x.PmuEng_01).FirstOrDefault();
            fila["Efficiency2"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("EFFICIENCY")).Select(x => x.PmuEng_02).FirstOrDefault();
            fila["UtilizationFactor1"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("UTILIZATION FACTOR")).Select(x => x.PmuEng_01).FirstOrDefault();
            fila["UtilizationFactor2"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("UTILIZATION FACTOR")).Select(x => x.PmuEng_02).FirstOrDefault();
            fila["CapacityFactor1"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("CAPACITY FACTOR")).Select(x => x.PmuEng_01).FirstOrDefault();
            fila["CapacityFactor2"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("CAPACITY FACTOR")).Select(x => x.PmuEng_02).FirstOrDefault();
            fila["LoadFactor1"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("LOAD FACTOR")).Select(x => x.PmuEng_01).FirstOrDefault();
            fila["LoadFactor2"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("LOAD FACTOR")).Select(x => x.PmuEng_02).FirstOrDefault();
            fila["HeatRate1"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("HEAT RATE")).Select(x => x.PmuEng_01).FirstOrDefault();
            fila["HeatRate2"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("HEAT RATE")).Select(x => x.PmuEng_02).FirstOrDefault();
            fila["GasConsumed1"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("GAS  CONSUMED")).Select(x => x.PmuEng_01).FirstOrDefault();
            fila["GasConsumed2"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("GAS  CONSUMED")).Select(x => x.PmuEng_02).FirstOrDefault();
            fila["Plant"] = datos.EnergiaProducida.Where(x => x.TipoEnergia.Equals("PLANT")).Select(x => x.PmuEng_01).FirstOrDefault();

            dataTable.Rows.Add(fila);

            return dataTable;
        }

        private async Task<DataTable> CrearStatus(DatosReporteProduccion datos)
        {
            DataTable dataTable = new DataTable("Status");

            dataTable.Columns.Add("ServiceAcumulate1", typeof(string));
            dataTable.Columns.Add("ServiceAcumulate2", typeof(string));
            dataTable.Columns.Add("PlannedMainten1", typeof(string));
            dataTable.Columns.Add("PlannedMainten2", typeof(string));
            dataTable.Columns.Add("ForcedMaintMech1", typeof(string));
            dataTable.Columns.Add("ForcedMaintMech2", typeof(string));
            dataTable.Columns.Add("ForcedMaintElec1", typeof(string));
            dataTable.Columns.Add("ForcedMaintElec2", typeof(string));
            dataTable.Columns.Add("ForcedAuxiliares1", typeof(string));
            dataTable.Columns.Add("ForcedAuxiliares2", typeof(string));
            dataTable.Columns.Add("ExternalTrips1", typeof(string));
            dataTable.Columns.Add("ExternalTrips2", typeof(string));
            dataTable.Columns.Add("StandBy1", typeof(string));
            dataTable.Columns.Add("StandBy2", typeof(string));
            dataTable.Columns.Add("RunningHours1", typeof(string));
            dataTable.Columns.Add("RunningHours2", typeof(string));
            dataTable.Columns.Add("HoursAvailable1", typeof(string));
            dataTable.Columns.Add("HoursAvailable2", typeof(string));


            ReporteProduccionStatus eg1 =  datos.Produccion.ReporteProduccionStatus.Where(x => x.NumeroGenerador.Equals(1)).Select(x => x).FirstOrDefault();
            ReporteProduccionStatus eg2 =  datos.Produccion.ReporteProduccionStatus.Where(x => x.NumeroGenerador.Equals(2)).Select(x => x).FirstOrDefault();

           DataRow fila = dataTable.NewRow();
            fila["ServiceAcumulate1"] = eg1.ServiceAccumulated;
            fila["ServiceAcumulate2"] = eg2.ServiceAccumulated;
            fila["PlannedMainten1"] = eg1.PlannedMainten;
            fila["PlannedMainten2"] = eg2.PlannedMainten;
            fila["ForcedMaintMech1"] = eg1.ForcedMaintMech;
            fila["ForcedMaintMech2"] = eg2.ForcedMaintMech;
            fila["ForcedMaintElec1"] = eg1.ForcedMaintElec;
            fila["ForcedMaintElec2"] = eg2.ForcedMaintElec;
            fila["ForcedAuxiliares1"] = eg1.ForcedAuxiliaries;
            fila["ForcedAuxiliares2"] = eg2.ForcedAuxiliaries;
            fila["ExternalTrips1"] = eg1.ExternalTrips;
            fila["ExternalTrips2"] = eg2.ExternalTrips;
            fila["StandBy1"] = eg1.StandBy;
            fila["StandBy2"] = eg2.StandBy;
            fila["RunningHours1"] = eg1.RunningHours;
            fila["RunningHours2"] = eg2.RunningHours;
            fila["HoursAvailable1"] = eg1.HoursAvailable;
            fila["HoursAvailable2"] = eg2.HoursAvailable;

            dataTable.Rows.Add(fila);

            return dataTable;
        }

        private async Task<DataTable> LevelLubeOilCarter(DatosReporteProduccion datos)
        {
            DataTable dataTable = new DataTable("OIL");

            dataTable.Columns.Add("ServiceAcumulate1", typeof(string));
            dataTable.Columns.Add("ServiceAcumulate2", typeof(string));


            DataRow fila = dataTable.NewRow();
            fila["ServiceAcumulate1"] = "";
            dataTable.Rows.Add(fila);

            return dataTable;
        }
    }

    public class DatosReporteProduccion
    {
        public Produccion? Produccion { get; set; }
        public List<LevelLubeOilCartel>? LevelLubeOilCartel { get; set; }
        public List<RefrescamientoCartel>? RefrescamientoCartel { get; set; }
        public List<CityGateFlow>? CityGateFlow { get; set; }
        public List<ManttoVessel>? ManttoVessel { get; set; }
        public List<TkCleanLube>? TkCleanLubeOil { get; set; }
        public List<TkCleanLube>? TkCleanLubeUsed { get; set; }
        public List<EnergiaProducida>? EnergiaProducida { get; set; }
        public datosGraficoION? datosGraficoION { get; set; }
    }
    public class datosGraficoION
    {
        public string[] Horas { get; set; }
        public decimal[] Valores { get; set; }
    }
    public class Produccion
    {
        public List<ArranqueSincronizacion> ArranqueSincronizacion { get; set; }
        public List<ReporteProduccionStatus> ReporteProduccionStatus { get; set; }
        public List<RegistroEventos> RegistroEventos { get; set; }
        public List<DetalleProduccion> DetalleProduccion { get; set; }
        public ReporteProducion DatosGeneralesroduccion { get; set; }
    }
}
