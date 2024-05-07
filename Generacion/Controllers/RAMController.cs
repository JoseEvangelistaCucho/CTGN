using Generacion.Application.Common;
using Generacion.Application.Funciones;
using Generacion.Application.RAM.Command;
using Generacion.Application.RAM.Query;
using Generacion.Infraestructura;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using Generacion.Models.ReporteRAM;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Generacion.Controllers
{
    public class RAMController : ApiControllerBase
    {
        private readonly FotoServidor _fotoServidor;
        private readonly IConfiguration _configuration;
        private readonly Function _function;
        public RAMController(FotoServidor fotoServidor, Function function, IConfiguration configuration)
        {
            _fotoServidor = fotoServidor;
            _function = function;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()//[FromQuery] string fecha)
        {

            string tipoConsulta = "año";
            string fecha = DateTime.Now.ToString("yyyy");

            var respuesta = await Mediator.Send(new ObtenerDatosEvento()
            {
                Fecha = fecha,
                Tipo = tipoConsulta
            });

            DateTime fechaConsulta = DateTime.Now;

            var respuestaData = await Mediator.Send(new DatosProduccionQuery()
            {
                TipoBusqueda = tipoConsulta,
                Fecha = fecha
            });

            Dictionary<string, CabecerasTabla> datosCabecera = await _function.ObtenerDatosCabecera();
            ViewData["datosCabecera"] = datosCabecera;

            ViewData["datosGeneralesProduccion"] = respuestaData.Detalle["datosGeneralesProduccion"];
            ViewData["datosDetalleProduccion"] = respuestaData.Detalle["datosDetalleProduccion"];
            ViewData["datosION"] = respuestaData.Detalle["datosION"];
            ViewData["reporteGas"] = respuestaData.Detalle["reporteGas"];
            ViewData["datosAceite"] = respuestaData.Detalle["datosAceite"];
            ViewData["cityGateToday"] = respuestaData.Detalle["cityGateToday"];
            ViewData["cityGateYesterday"] = respuestaData.Detalle["cityGateYesterday"];
            ViewData["datosRamPorMes"] = respuestaData.Detalle["datosRamPorMes"];
            ViewData["datosArranque"] = respuestaData.Detalle["datosArranque"];
            ViewData["datosVistaOil"] = respuestaData.Detalle["datosVistaOil"];
            ViewData["datosGraficosOil"] = respuestaData.Detalle["datosGraficosOil"];
            
            ViewBag.fechaSeleccionada = fecha;

            return View(respuesta.Detalle["eventosPorFecha"]);
        }

        public async Task<IActionResult> Data([FromQuery] string fecha)
        {
           

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GuardarDatosRAMOIL([FromBody] DatosReportes datos)
        {


            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();

            byte[] fileBytes = null;
            try
            {
                respuesta = await Mediator.Send(new GuardarRegistroRAM()
                {
                    DBRAM = datos.DBRAM,
                    //Tipo = "oil",
                    VistaAceite = datos.ListaViewOil
                });


                ExcelPackage excelPackage = (ExcelPackage)respuesta.Detalle[Constante.idContextExcel];
                await GuardarARchivoExcel(excelPackage);

                await EliminarVistasDesde(excelPackage, 2);
                var tempFilePath = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await excelPackage.SaveAsAsync(fileStream);
                }

                fileBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);

                System.IO.File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al Guardar.";

                throw new ArgumentException($"Error : {ex}");
            }

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");
        }
        private async Task EliminarVistasDesde(ExcelPackage excelPackage, int indiceInicio)
        {
            // Verificar si el índice de inicio es válido
            if (indiceInicio <= 1)
            {
                throw new ArgumentException("El índice de inicio debe ser mayor que 1.");
            }

            // Eliminar las vistas desde el índice de inicio hasta el final
            for (int i = excelPackage.Workbook.Worksheets.Count - 1; i >= indiceInicio; i--)
            {
                excelPackage.Workbook.Worksheets.Delete(i);
            }
        }

        public async Task GuardarARchivoExcel(ExcelPackage excelPackage)
        {
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                IConfigurationSection rutaReportes = _configuration.GetSection("RutaReportes");

                string rutaArchivo = $"{rutaReportes["pathReporte"]}\\{datosOperario.IdSitio}\\RAM CGD.xlsx";
                //string rutaArchivo = "C:\\aplicaciones\\PortalGeneracion\\Reportes\\Excel\\RAM CGD.xlsx"; // Especifica la ruta deseada aquí

                using (var fileStream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    await excelPackage.SaveAsAsync(fileStream);
                }
            }catch (Exception ex)
            {

            }
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosRAMOIL2([FromBody] List<ViewOIL> datos)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();
            try
            {
                respuesta = await Mediator.Send(new GuardarRegistroRAM()
                {
                    //Tipo = "oil",
                    VistaAceite = datos
                });
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al Guardar.";
            }

            return Json(new { respuesta = respuesta });
        }



        /* [HttpPost]
         public async Task<JsonResult> ExportarReporte([FromBody] DatosReportes datosReportes)
         //public async Task<JsonResult> ExportarReporte([FromBody] Dictionary<int, Dictionary<int, HorasTotalesPorMes>> datosCalculados)
         {
             Respuesta<Dictionary<string, object>> respuesta = null;
             try
             {
                 string tipoConsulta = string.Empty;
                 if (string.IsNullOrEmpty(datosReportes.Fecha))
                 {
                     datosReportes.Fecha = DateTime.Now.ToString("yyyy");
                     tipoConsulta = "año";
                 }
                 else
                 {
                     tipoConsulta = "dia";
                 }


                 respuesta = await Mediator.Send(new ObtenerDatosEvento()
                 {
                     Fecha = datosReportes.Fecha,
                     Tipo = tipoConsulta
                 });
                 datosReportes.DatosEvento.EventoMes = new Dictionary<int, Dictionary<DateTime, List<RegistroEventos>>>();
                 datosReportes.DatosEvento.EventoMes = (Dictionary<int, Dictionary<DateTime, List<RegistroEventos>>>)respuesta.Detalle["eventosPorFecha"];

                  //DatosReportes datosReportes = new DatosReportes()
                  //{
                  //    EventoMes = (Dictionary<int, Dictionary<DateTime, List<RegistroEventos>>>)respuesta.Detalle["eventosPorFecha"],
                  //   // HorasTotalesPorMes = datosCalculados
                  //};



                 _fotoServidor.GuardarJson(datosReportes.DatosEvento, "Outage.json", this, "Outage");
             }
             catch (Exception ex)
             {

             }
             return Json(new { respuesta = respuesta });
         }*/
    }

    public class DatosReportes
    {
        public List<DBRAM>? DBRAM { get; set; }
        public List<ViewOIL>? ListaViewOil { get; set; }
    }

    /*public class DatosReportes
    {
        public DatosEvento DatosEvento { get; set; }
        public string Fecha { get; set; }
    }

    public class DatosEvento
    {
        public Dictionary<int, Dictionary<int, HorasTotalesPorMes>>? HorasTotalesPorMes { get; set; }
        public Dictionary<int, Dictionary<DateTime, List<RegistroEventos>>>? EventoMes { get; set; }
    }

    public class HorasTotalesPorMes
    {
        public string Subtitulo { get; set; }
        public string TotalExternalTrips { get; set; }
        public string TotalForcedMaint { get; set; }
        public string TotalPlannedMaint { get; set; }
        public string TotalStandBy { get; set; }
    }*/
}
