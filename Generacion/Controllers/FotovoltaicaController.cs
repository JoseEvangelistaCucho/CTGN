using Generacion.Application.Fotovoltaica.Command;
using Generacion.Application.Fotovoltaica.Query;
using Generacion.Application.Funciones;
using Generacion.Infraestructura;
using Generacion.Models.FotoVoltaica;
using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Report;
using System.Data;
using System.Text.Json;

namespace Generacion.Controllers
{
    public class FotovoltaicaController : ApiControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly FotoServidor _fotoServidor;
        public FotovoltaicaController(IConfiguration configuration, FotoServidor fotoServidor)
        {
            _configuration = configuration;
            _fotoServidor = fotoServidor;
        }

        public async Task<IActionResult> Index()//[FromQuery] string fecha)
        {
            string fecha = "";
            if (string.IsNullOrEmpty(fecha))
            {
                fecha = DateTime.Now.ToString("dd/MM/yyyy");
            }

            var respuesta = await Mediator.Send(new ObtenerRegistrosFotoVoltaica
            {
                Fecha = fecha
            });


            ViewData["datosCabecera"] = respuesta.Detalle["datosCabecera"];
            ViewData["datosReporte"] = respuesta.Detalle["datosReporte"];
            ViewData["detalleGenerado"] = respuesta.Detalle["detalleGenerado"];

            ViewBag.Fecha = fecha;

            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GuardarDetalleGenerado([FromBody] RegistroReporteFotoVoltaica datos)
        {
            var respuesta = await Mediator.Send(datos);

            _fotoServidor.GuardarJson(datos, "Reporte_fotovoltaica.json", this, "Reporte_fotovoltaica");

            //EjemploReporte();

            return Json(new { respuesta = respuesta });
        }

        public void EjemploReporte()
        {
            try
            {
            IConfigurationSection rutaReporte = _configuration.GetSection("RutaReportes");
                var nombreReporte = "Report.mrt";

                var dataSet = CrearDataSet();
                var report = new StiReport();
                report.Load(Path.Combine(rutaReporte["pathReporteMRT"], nombreReporte));
                report.RegData("Report", dataSet);
                report.Render(false);
                report.ExportDocument(StiExportFormat.Pdf, "C:\\aplicaciones\\PortalGeneracion\\Reportes\\Centrales\\PRUEBAS\\Report.pdf");

            }
            catch (Exception ex)
            {

            }

        }

        static DataSet CrearDataSet()
        {
            DataSet dataSet = new DataSet("ejemplo");

            DataTable dataTable = new DataTable("Datos1");

            dataTable.Columns.Add("Nombre", typeof(string));

            dataTable.Rows.Add( "Juan");
            dataTable.Rows.Add( "María");
            dataTable.Rows.Add("Pedro");

            // Agregar la tabla al DataSet
            dataSet.Tables.Add(dataTable);

            // Devolver el DataSet creado
            return dataSet;
        }
    }
}
