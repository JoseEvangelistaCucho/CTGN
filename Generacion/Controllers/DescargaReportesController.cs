using Generacion.Application.BessLlipata.DataBess.Query;
using Generacion.Application.BessLlipata.DataCoes.Query;
using Generacion.Application.Common;
using Generacion.Application.DescargaReporte.Query;
using Generacion.Models.Usuario;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Generacion.Controllers
{
    public class DescargaReportesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ISender _sender;

        public DescargaReportesController(IConfiguration configuration, ISender sender)
        {
            _sender = sender;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            DetalleOperario detalleOperario = new DetalleOperario()
            {
                Apellidos = "",
                DescripcionCargo = "CONSULTAS",
                IdCargo = "CONSULTAS",
                IdOperario = "Con-Rp",
                IdSitio = "PRUEBAS",
                IdTurno = 0,
                Nombre = ""
            };
            HttpContext.Session.SetString("usuarioDetail", JsonConvert.SerializeObject(detalleOperario));

            string fecha = "";
            if (string.IsNullOrEmpty(fecha))
            {
                fecha = DateTime.Now.ToString("dd/MM/yyyy");
            }

            var respuesta = await _sender.Send(new ObtenerRegistroReporte()
            {
                Fecha = fecha,
                TipoReporte ="año"
            });
            ExcelPackage excelPackage = (ExcelPackage)respuesta.Detalle[Constante.idContextExcel];
            await GuardarARchivoExcel(excelPackage);

            var tempFilePath = Path.GetTempFileName();
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                await excelPackage.SaveAsAsync(fileStream);
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);

            System.IO.File.Delete(tempFilePath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");

            return View();
        }
        public async Task GuardarARchivoExcel(ExcelPackage excelPackage)
        {
            string rutaArchivo = "C:\\aplicaciones\\PortalGeneracion\\Reportes\\Excel\\archivo.xlsx"; // Especifica la ruta deseada aquí

            using (var fileStream = new FileStream(rutaArchivo, FileMode.Create))
            {
                await excelPackage.SaveAsAsync(fileStream);
            }
        }
        public async Task<IActionResult> ObtenerReporte([FromBody] DatosReportes datos)
        {
            string fecha = "";
            if (string.IsNullOrEmpty(fecha))
            {
                fecha = DateTime.Now.ToString("dd/MM/yyyy");
            }

            var respuesta = await _sender.Send(new ObtenerRegistroReporte()
            {
                Fecha = fecha,
                TipoReporte = "año"
            });

            ExcelPackage excelPackage = (ExcelPackage)respuesta.Detalle[Constante.idContextExcel];
            await GuardarARchivoExcel(excelPackage);

            var tempFilePath = Path.GetTempFileName();
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                await excelPackage.SaveAsAsync(fileStream);
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);

            System.IO.File.Delete(tempFilePath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "archivo.xlsx");
        }
    }
}
