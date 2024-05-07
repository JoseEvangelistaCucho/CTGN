using Generacion.Application.Common;
using Generacion.Application.Funciones;
using Generacion.Models;
using MediatR.Pipeline;
using OfficeOpenXml;

namespace Generacion.Application.RAM.Command.Processor.PostProcessor
{
    public class _01_ObtenerExcel : IRequestPostProcessor<GuardarRegistroRAM, Respuesta<Dictionary<string, object>>>
    {
        private readonly IConfiguration _configuration;
        private readonly Function _function;
        private readonly ProcessExecutionContextExtensions _executionContextExtensions;
        public _01_ObtenerExcel(Function function, ProcessExecutionContextExtensions executionContextExtensions, IConfiguration configuration)
        {
            _function = function;
            _executionContextExtensions = executionContextExtensions;
            _configuration = configuration;
        }
        public async  Task Process(GuardarRegistroRAM request, Respuesta<Dictionary<string, object>> response, CancellationToken cancellationToken)
        {
            try
            {
                IConfigurationSection rutaReportes = _configuration.GetSection("RutaReportes");
                var datosOperario = await _function.ObtenerDatosOperario();

                string rutaArchivo = $"{rutaReportes["pathReporteExcel"]}\\RAM CGD.xlsx";

                string rutaArchivoCentral = $"{rutaReportes["pathReporte"]}\\{datosOperario.IdSitio}\\RAM CGD.xlsx";


                ExcelPackage excelPackage = new ExcelPackage();

                if (!File.Exists(rutaArchivoCentral))
                {
                    if (!File.Exists(rutaArchivo))
                    {
                        throw new FileNotFoundException("El archivo Excel no existe.", rutaArchivo);
                    }
                    else
                    {
                        excelPackage = new ExcelPackage(new FileInfo(rutaArchivo));
                    }
                }
                else
                {
                    excelPackage = new ExcelPackage(new FileInfo(rutaArchivoCentral));
                }

                _executionContextExtensions.TryAdd(Constante.idContextExcel, excelPackage);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
