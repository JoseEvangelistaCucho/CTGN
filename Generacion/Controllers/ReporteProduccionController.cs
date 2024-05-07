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
using Generacion.Application.Common;
using Generacion.Application.Usuario.Query;
using Generacion.Models.Session;
using Stimulsoft.Report;
using System.Data;

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
        public ReporteProduccionController(ConsultarION consultarION,
            IConfiguration configuration,
            Function function,
            CacheDatos cacheDatos,
            IReporteProduccion datoEnergiaProducida, DatosConsola datosConsola,
            ConsultarProduccion consultarProduccion, LecturaCampo lecturaCampo,
            ILogger<ReporteProduccionController> logger,
            FotoServidor fotoServidor, ConsultarUsuario consultarUsuario)
        {
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
               
                DateTime fechaActual = DateTime.Now;//string.IsNullOrEmpty(fecha) ? DateTime.Now : DateTime.Parse(fecha);

                Respuesta<List<SessionOperario>> datosSessionOperarios = await _consultarUsuario.ObtenerSessionOperarios(fechaActual.ToString("dd/MM/yyyy"));


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

                string datoscabeceraJson = HttpContext.Session.GetString("datoscabecera");

                Respuesta<List<decimal>> datosRegistro = await _datosConsola.ObtenerDatosDetConsola(fechaActual.AddDays(-1).ToString("dd/MM/yyyy"), fechaActual.ToString("dd/MM/yyyy"), $"{user.IdSitio}-BFA901%", 8);
                string fechaFrontend = "09/09/2023";
             
                Respuesta<List<DatosFormatoMGD>> respuesta = await Mediator.Send(new ObtenerDatosIONQuery()
                {
                    Fecha = fechaActual//DateTime.Parse(fechaFrontend)
                });

                Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>> detalleCampoOperacionHoy = await _lecturaCampo.ObtenerDetalleCampoPorFecha("HrsOperacion,Operacion_ci,NivelCarter", 24, fechaActual.ToString("dd/MM/yy"));
                Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>> detalleCampoOperacionAyer = await _lecturaCampo.ObtenerDetalleCampoPorFecha("HrsOperacion,Operacion_ci,NivelCarter", 24, fechaActual.AddDays(-1).ToString("dd/MM/yy"));
                Respuesta<Dictionary<string, List<RegistrosDatosEngine>>> datosEngine = await _datosConsola.ObtenerDatosEngine(fechaActual.ToString("dd/MM/yyyy"), fechaMedianoche.ToString("dd/MM/yyyy"), user.IdSitio);


                Respuesta<List<EnergiaProducida>> datosProduccion = await _consultarProduccion.ObtenerRegistroProduccion(fechaActual.AddDays(-1).ToString("dd/MM/yyyy"),"READING TODAY","dia");
                Respuesta<List<LevelLubeOilCartel>> datosProduccionCarter = await _consultarProduccion.ObtenerRegistroLevelCartel(fechaActual.AddDays(-1).ToString("dd_MM_yyyy"),"dia","TODAY");
                Respuesta<List<CityGateFlow>> datosProduccionCity = await _consultarProduccion.ObtenerRegistroCityGate(fechaActual.AddDays(-1).ToString("dd_MM_yyyy"),"dia", "READING TODAY");
                Respuesta<Dictionary<string, TkCleanLube>> datosProduccionTkClean = await _consultarProduccion.ObtenerRegistroTkCleanPorTipo();
                Respuesta<Dictionary<string, ManttoVessel>> datosMantoVessel = await _consultarProduccion.ObtenerMantoTKVessel(fechaActual.ToString("dd_MM_yyyy"));
                Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(datoscabeceraJson);

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
                ViewData["DatosSincro"] = datosGeneralesProduccion.Detalle["DatosSincro"];
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
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosTkCleanLube(datos);
            return Json(new { respuesta = respuesta });
        }
        [HttpPost]
        public async Task<JsonResult> GuardarReporteProduccion([FromBody] ReporteProducion datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosReporteProduccion(datos);
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
            var datosOperario = await _function.ObtenerDatosOperario();

            DataSet dataSet = new DataSet("ReporteProduccion2");
            
            DataTable[] dataTable = new DataTable[]{
                await CrearDatosGenerales(datos),
                await CrearOperarios(datos),
                await CrearStatus(datos),
                await CrearTotals(datos),
                await CrearEnergyProduce(datos),
                await ObtenerNumeroDeArranque()
            }; 
            dataSet.Tables.AddRange(dataTable);

            return dataSet;
        }
        private async Task<DataTable> ObtenerNumeroSincronizaciones()
        {
            DataTable dataTable = new DataTable("NumeroSincronizaciones");

            dataTable.Columns.Add("diario", typeof(string));
            dataTable.Columns.Add("mensual", typeof(string));

            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();
            fila["diario"] = "";
            fila["mensual"] = "";

            dataTable.Rows.Add(fila);

            // Devolver el DataSet creado
            return dataTable;
        }
        private async Task<DataTable> ObtenerNumeroDeArranque()
        {
            DataTable dataTable = new DataTable("NumeroArranques");

            dataTable.Columns.Add("diario", typeof(string));
            dataTable.Columns.Add("mensual", typeof(string));

            // Agregar una fila al DataTable con los datos correspondientes
            DataRow fila = dataTable.NewRow();
            fila["diario"] = "";
            fila["mensual"] = "";

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
    }

    public class DatosReporteProduccion
    {
        public Produccion? Produccion { get; set; }
        public List<LevelLubeOilCartel>? LevelLubeOilCartel { get; set; }
        public List<RefrescamientoCartel>? RefrescamientoCartel { get; set; }
        public List<CityGateFlow>? CityGateFlow { get; set; }
        public List<ManttoVessel>? ManttoVessel { get; set; }
         public List<TkCleanLube>? TkCleanLube { get; set; }
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
