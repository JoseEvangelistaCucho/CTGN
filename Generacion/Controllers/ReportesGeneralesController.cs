using Generacion.Application.Common;
using Generacion.Application.DataBase.cache;
using Generacion.Application.Funciones;
using Generacion.Application.ReporteModulacion.Command;
using Generacion.Application.ReportesGenerales.DataBess.Command;
using Generacion.Application.ReportesGenerales.DataCoes.Command;
using Generacion.Infraestructura;
using Generacion.Models;
using Generacion.Models.Bess;
using Generacion.Models.Mantenimiento;
using Microsoft.AspNetCore.Mvc;

namespace Generacion.Controllers
{
    public class ReportesGeneralesController : ApiControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly FotoServidor _fotoServidor;
        private readonly Function _function;
        private readonly CacheDatos _cacheDatos;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportesGeneralesController(IHttpContextAccessor httpContextAccessor, FotoServidor fotoServidor, Function function, IConfiguration configuration, CacheDatos cacheDatos)
        {
            _httpContextAccessor = httpContextAccessor;
            _cacheDatos = cacheDatos;
            _configuration = configuration;
            _function = function;
            _fotoServidor = fotoServidor;
        }
        [HttpPost]
        public async Task<IActionResult> GuardarImagen([FromBody] ImagenReporte imagenBase64)
        {
            try
            {
                string base64Data = imagenBase64.base64.Split(',').Last();
                byte[] imageBytes = Convert.FromBase64String(base64Data);

                _fotoServidor.GuardarArchivos(imageBytes, 1, "", $"{imagenBase64.NombreReporte}/Reportes/{DateTime.Now.ToString("yyyy")}/{DateTime.Now.ToString("MM")}", imagenBase64.TipoReporte, DateTime.Now.ToString("dd-MM-yyyy"));
                return Json(new { mensaje = "Imágenes recibidas correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al recibir las imágenes: " + ex.Message });
            }
        }

        public async Task<IActionResult> DescargarArchivo(string name, string reporte)
        {
            try
            {
                IConfigurationSection idDatosCampo = _configuration.GetSection("RutaReportes");

                var datosOperario = await _function.ObtenerDatosOperario();

                string filePath = $"{idDatosCampo["pathReporte"]}/{datosOperario.IdSitio}/{reporte}/{DateTime.Now.ToString("yyyy")}/{DateTime.Now.ToString("MM")}/{DateTime.Now.ToString("dd-MM-yyyy")}.pdf";
                byte[] content = System.IO.File.ReadAllBytes(filePath);

                return File(content, "application/pdf", name);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al descargar el archivo PDF.");
            }
        }

        /*  public async Task<IActionResult> DescargarArchivo(string name, string reporte)
          {
              try
              {
                  StiReport report = new StiReport();
                  report.Load(@"C:\aplicaciones\Reportes\MRT\informe_perturbacion.mrt");
                  report.Render(false);

                  report.ExportDocument(StiExportFormat.Pdf, @"C:\aplicaciones\Reportes\MRT\informe_perturbacion.pdf");

                  System.Diagnostics.Process.Start("cmd", $"/c start \"\" \"C:\\aplicaciones\\Reportes\\MRT\\informe_perturbacion.pdf\"");
              }
              catch (Exception ex)
              {
                  TempData["mensajeError"] = ex;
              }

              return RedirectToAction("Index");
          }*/




        [HttpPost]
        public async Task<IActionResult> GuardarDatosCSV([FromBody] DataCSV datos)
        {
            string mensaje = string.Empty;
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();

                string key = $"{datos.Tipo}_{Constante.keyCSV}_{datosOperario.IdSitio}";
                _cacheDatos.GuardarDatosCache(key, datos.JsonData);

                mensaje = "Ok";
            }
            catch (Exception ex)
            {
                mensaje = "Error";
            }
            return Json(new { data = mensaje });
        }

        [HttpPost("procesar")]
        public async Task<IActionResult> SubirArchivoText([FromForm] IFormFile archivo, string seleccion)
        {
            Respuesta<List<DBDataBess>> respuesta = new Respuesta<List<DBDataBess>>();

            try
            {
                if (archivo == null || archivo.Length == 0)
                    return BadRequest("No se ha enviado ningún archivo.");

                if (seleccion.Equals(Constante.dataBess))
                {
                    respuesta = await Mediator.Send(new GuardarDatosBess()
                    {
                        Archivo = archivo
                    });
                }
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = "Error en el servidor: ";
            }
            return Json(new { data = respuesta.Mensaje });
        }

        [HttpPost]
        public async Task<IActionResult> SubirArchivo(IFormFile archivoXLSX, string seleccion)
        {
            string mensaje = string.Empty;

            try
            {
                if (archivoXLSX != null && archivoXLSX.Length > 0)
                {
                    // Guardar el archivo en el servidor (ajusta la ruta según tus necesidades)
                    string rutaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ArchivosSubidos", archivoXLSX.FileName);
                    using (var stream = new FileStream(rutaDestino, FileMode.Create))
                    {
                        await archivoXLSX.CopyToAsync(stream);
                    }

                    if (seleccion.Equals(Constante.dataCoes))
                    {
                        var respuesta = Mediator.Send(new GuardarDatosCOES()
                        {
                            path = rutaDestino
                        });
                    }



                    if (seleccion.Equals(Constante.facturaModulacion))
                    {
                        var respuesta = await Mediator.Send(new GuardarfacturaModulacionQuery()
                        {
                            RutaArchivo = rutaDestino
                        });
                        /*
                        FacturacionModulacion datosColumna = await ObtenerDatosColumna(rutaDestino);

                        

                        _cacheDatos.GuardarDatosCache(Constante.facturaModulacion, JsonConvert.SerializeObject(datosColumna));
                        */
                        mensaje = "Archivo subido correctamente";
                    }
                    if (seleccion.Equals(Constante.seleccionCostosMarginales))
                    {
                        var respuesta = await Mediator.Send(new GuardarCostosMarginalesQuery()
                        {
                            RutaArchivo = rutaDestino
                        });

                    }

                }
                else
                {
                    mensaje = "No se ha proporcionado ningún archivo";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error en el servidor: ";
            }
            return Json(new { data = mensaje });
        }
    }

    public class DataCSV
    {
        public string JsonData { get; set; }
        public string Tipo { get; set; }
    }

    public class FacturacionModulacion
    {
        public List<DetalleFacturacionModulacion> DetalleFacturacionModulacion { get; set; }

        public Pef_MGD Pef_MGD_Luren { get; set; }
        public Pef_MGD Pef_MGD_Pedreal { get; set; }
        public ValoresPEM ValoresPEM { get; set; }

        /// <summary>
        /// Valor que se obtiene del 1 al 3
        /// </summary>
        public ValoresPEMFSoles PEMFAnterior { get; set; }
        /// <summary>
        /// Valor que inicia el 4 hasta el ultimo dia del mes
        /// </summary>
        public ValoresPEMFSoles PEMFActual { get; set; }
        public decimal TC { get; set; }

    }
    public class Pef_MGD
    {
        public decimal Pef { get; set; }
        public decimal Operacion1 { get; set; }
        public decimal Operacion2 { get; set; }

    }

    public class DetalleFacturacionModulacion
    {
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public int Dia { get; set; }
        public int BH { get; set; }
        public CTCentralModulacion CTLuren { get; set; }
        public CTCentralModulacion CTPedregal { get; set; }
        public CostosBarraYMarginal CTLurenTarifas { get; set; }
        public CostosBarraYMarginal CTPedregalTarifas { get; set; }

    }


    public class ValoresPEM
    {
        public Dictionary<string, decimal> Potencia { get; set; }
        public Dictionary<string, decimal> Energia { get; set; }
    }

    public class CTCentralModulacion
    {
        public decimal MWH { get; set; }
        public decimal MW { get; set; }
    }
    public class ValoresMT
    {
        public decimal SistemaMAT { get; set; }
        public decimal SistemaMATAT { get; set; }
        public decimal SistemaAT { get; set; }
        public decimal SistemaMT { get; set; }
    }


    public class ValoresPEMFSoles
    {
        public decimal PEMP { get; set; }
        public decimal PEMF { get; set; }
    }

    public class CostosBarraYMarginal
    {
        /// <summary>
        ///  VBEM 
        /// </summary>
        public decimal VBEM { get; set; }

        /// <summary>
        ///  Tarifa en barra ($/MWh) Independencia 
        /// </summary>
        public decimal TarifaEnBarra { get; set; }

        /// <summary>
        ///  CMg ($/MWh) 
        /// </summary>
        public decimal CostoMarginal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal TM { get; set; }
        public decimal PagoPorModulacion { get; set; }
    }
}
