using Generacion.Application.Common;
using MediatR.Pipeline;
using OfficeOpenXml;

namespace Generacion.Application.DescargaReporte.Query.Processor.Preprocessor
{
    public class _03_ObtenerExcel : IRequestPreProcessor<ObtenerRegistroReporte>
    {
        private readonly IConfiguration _configuration;
        private readonly ProcessExecutionContextExtensions _executionContextExtensions;
        public _03_ObtenerExcel(ProcessExecutionContextExtensions executionContextExtensions, IConfiguration configuration)
        {
            _executionContextExtensions = executionContextExtensions;
            _configuration = configuration;
        }
        public  Task Process(ObtenerRegistroReporte request, CancellationToken cancellationToken)
        {
            try
            {
                IConfigurationSection rutaReportes = _configuration.GetSection("RutaReportes");
                string rutaArchivo = $"{rutaReportes["pathReporteExcel"]}\\ReporteCompleto.xlsx";

                if (!File.Exists(rutaArchivo))
                {
                    throw new FileNotFoundException("El archivo Excel no existe.", rutaArchivo);
                }

                ExcelPackage excelPackage = new ExcelPackage(new FileInfo(rutaArchivo));

                _executionContextExtensions.TryAdd(Constante.idContextExcel, excelPackage);
            }
            catch (Exception ex)
            {
                throw;
            }
            return Task.CompletedTask;
        }
    }
}
