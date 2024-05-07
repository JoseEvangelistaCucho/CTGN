using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report;
using Newtonsoft.Json;
using static Stimulsoft.Report.Help.StiHelpProvider;
using Generacion.Application.Common;
using NuGet.Packaging;
using Stimulsoft.Base;

namespace Generacion.Application.Funciones
{
    public class FotoServidor
    {
        private readonly Function _function;
        private readonly IConfiguration _configuration;
        private readonly ProcessExecutionContextExtensions _context;
        public FotoServidor(Function function, IConfiguration configuration, ProcessExecutionContextExtensions context)
        {
            _context = context;
            _function = function;
            _configuration = configuration;
        }
        public async Task<string> GuardarArchivos(byte[] imagenes, int indexImagen, string casilllaId, string ruta, string extension, string nombreArchivoRuta)
        {
            try
            {
                IConfigurationSection rutaReportes = _configuration.GetSection("RutaReportes");

                var datosOperario = await _function.ObtenerDatosOperario();
                //DateTime fecha = DateTime.Now;
                string nombreArchivo = "";

                
                if (extension.Equals("pdf"))
                {
                    
                    nombreArchivo = $"{nombreArchivoRuta}.{extension}";

                }
                else
                {
                    ruta = $"{rutaReportes["pathImagenes"]}\\{datosOperario.IdSitio}\\{ruta}";
                    nombreArchivo = $"{nombreArchivoRuta}_{indexImagen}_{casilllaId}.{extension} ";


                    Dictionary<int, string> rutaIndex = new Dictionary<int, string>();

                    string rutaCompleta = Path.Combine(ruta, nombreArchivo);

                    if (_context.ContainsKey("rutaArchivoGuardado"))
                    {
                        rutaIndex = (Dictionary<int, string>)_context["rutaArchivoGuardado"];
                        rutaIndex.Add(indexImagen, rutaCompleta);

                        _context["rutaArchivoGuardado"] = rutaIndex;
                    }
                    else
                    {
                        rutaIndex.Add(indexImagen, rutaCompleta);

                        _context.TryAdd("rutaArchivoGuardado", rutaIndex);
                    }
                }
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }
                using (FileStream fs = new FileStream(Path.Combine(ruta, nombreArchivo), FileMode.Create))
                {
                    fs.Write(imagenes, 0, imagenes.Length);
                }

                return "Ok";
            }
            catch (Exception ex)
            {
                return "Error al Guardar";
            }
        }

        public async Task GuardarJson(object objeto, string nombre, Controller controller,string nombreReporte)
        {
            try
            {
                IConfigurationSection idDatosCampo = _configuration.GetSection("RutaReportes");
                var datosOperario = await _function.ObtenerDatosOperario();

                if (!Directory.Exists($"{idDatosCampo["pathJson"]}/{datosOperario.IdSitio}"))
                {
                    Directory.CreateDirectory($"{idDatosCampo["pathJson"]}/{datosOperario.IdSitio}");
                }
                
                string datosObjeto = JsonConvert.SerializeObject(objeto, Formatting.Indented);
                System.IO.File.WriteAllText($"{idDatosCampo["pathJson"]}/{datosOperario.IdSitio}/{nombre}", datosObjeto);

                //ExportReport(controller, nombre.Split('.')[0], nombreReporte);
                ExportReport(controller, nombreReporte);
            }
            catch (Exception ex)
            {
            }

        }
        public async Task ExportReport(Controller controller, string archivoMRT)
        {
            try
            {
                IConfigurationSection idDatosCampo = _configuration.GetSection("RutaReportes");
                var datosOperario = await _function.ObtenerDatosOperario();

                var report = StiReport.CreateNewReport();
                var path = StiNetCoreHelper.MapPath(controller, $@"{idDatosCampo["pathReporteMRT"]}\{archivoMRT}.mrt");
                report.Load(path);
                var datoRreporte = StiNetCoreReportResponse.ResponseAsPdf(report);
                
                GuardarArchivos(datoRreporte.Data, 1, "0", $"{idDatosCampo["pathReporte"]}\\{datosOperario.IdSitio}\\{archivoMRT}\\{DateTime.Now.ToString("yyyy")}\\{DateTime.Now.ToString("MM")}", "pdf", DateTime.Now.ToString("dd-MM-yyyy"));

            }
            catch (Exception ex)
            {

            }
        }
        /* public async Task ExportReport(Controller controller,string archivoMRT, string nombreReporte)
         {
             try
             {
                 IConfigurationSection idDatosCampo = _configuration.GetSection("RutaReportes");
                 var datosOperario = await _function.ObtenerDatosOperario();

                 var report = StiReport.CreateNewReport();

                 var path = StiNetCoreHelper.MapPath(controller, Path.Combine(idDatosCampo["pathReporteMRT"], $"{archivoMRT}.mrt"));

                 report.Load(path);
                 var datoRreporte = StiNetCoreReportResponse.ResponseAsPdf(report);

                 var pathPdf = $"{idDatosCampo["pathReporte"]}\\{datosOperario.IdSitio}\\{nombreReporte}\\{DateTime.Now.ToString("yyyy")}\\{DateTime.Now.ToString("MM")}\\{DateTime.Now.ToString("dd-MM-yyyy")}.pdf";
                 //report.ExportDocument(StiExportFormat.Pdf, pathPdf);
                 System.Diagnostics.Process.Start("cmd", $"/c start \"\" \"{pathPdf}\"");

                 //GuardarArchivos(datoRreporte.Data, 1, "0", $"{idDatosCampo["pathReporte"]}\\{datosOperario.IdSitio}\\{nombreReporte}\\{DateTime.Now.ToString("yyyy")}\\{DateTime.Now.ToString("MM")}", "pdf", DateTime.Now.ToString("dd-MM-yyyy"));

             }
             catch (Exception ex)
             {

             }
         }*/
    }
}
