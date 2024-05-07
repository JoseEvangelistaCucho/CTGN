using Generacion.Application.Common;
using Generacion.Models;
using Generacion.Models.ReporteProduccion;
using MediatR.Pipeline;
using OfficeOpenXml;

namespace Generacion.Application.RAM.Command.Processor.PostProcessor
{
    public class _05_AgregarVistaEG2 : IRequestPostProcessor<GuardarRegistroRAM, Respuesta<Dictionary<string, object>>>
    {
        private readonly ProcessExecutionContextExtensions _executionContextExtensions;

        public _05_AgregarVistaEG2(ProcessExecutionContextExtensions executionContextExtensions)
        {
            _executionContextExtensions = executionContextExtensions;
        }
        public Task Process(GuardarRegistroRAM request, Respuesta<Dictionary<string, object>> response, CancellationToken cancellationToken)
        {
            try
            {
                ExcelPackage excelPackage = (ExcelPackage)_executionContextExtensions["excelPackage"];

                Dictionary<int, Dictionary<DateTime, List<RegistroEventos>>> eventos = (Dictionary<int, Dictionary<DateTime, List<RegistroEventos>>>)_executionContextExtensions["eventosPorFecha"];

                Dictionary<string, object> datosRAM = (Dictionary<string, object>)_executionContextExtensions["datosRAM"];

                Dictionary<string, Dictionary<int, Dictionary<string, ArranqueSincronizacion>>> datosArranque = datosRAM["datosArranque"] as Dictionary<string, Dictionary<int, Dictionary<string, ArranqueSincronizacion>>>;

                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[3];

                DateTime fechaActual = DateTime.Now;

                ArranqueSincronizacion numeroArranqueMes = datosArranque != null && datosArranque.ContainsKey(fechaActual.ToString("MMMM").ToLower()) ? datosArranque[fechaActual.ToString("MMMM").ToLower()][2]["arranque"] : null;
                ArranqueSincronizacion numeroSincroMes = datosArranque != null && datosArranque.ContainsKey(fechaActual.ToString("MMMM").ToLower()) ? datosArranque[fechaActual.ToString("MMMM").ToLower()][2]["sincronizacion"] : null;

                int asciiLetraB = Constante.posicionLetraA;

                var index = fechaActual.Month - 1;
                int poscionActual = fechaActual.Month != 1 ? (fechaActual.Month * 2) - 1 : 0;

                char caracterAscii = (char)(asciiLetraB + poscionActual);
                worksheet.Cells[$"{caracterAscii.ToString()}9"].Value = request.DBRAM[index].LHV_kJkgGE2;

                char caracterAsciiYTD = (char)(asciiLetraB + poscionActual + 1);
                worksheet.Cells[$"{caracterAsciiYTD.ToString()}9"].Value = request.DBRAM[index].LHV_kJkgGE2;

                if (numeroArranqueMes != null)
                {
                    worksheet.Cells[$"{caracterAscii.ToString()}12"].Value = numeroArranqueMes.Mensual;
                }
                if (numeroSincroMes != null)
                {
                    worksheet.Cells[$"{caracterAscii.ToString()}13"].Value = numeroSincroMes.Mensual;
                }
                if (numeroArranqueMes != null && numeroSincroMes != null)
                {
                    worksheet.Cells[$"{caracterAscii.ToString()}14"].Value = numeroArranqueMes.Mensual - numeroSincroMes.Mensual;
                }

                worksheet.Cells[$"{caracterAscii.ToString()}28"].Value = request.DBRAM[index].HorasDerateoEquivalenteGE2;


                _executionContextExtensions.TryAdd(Constante.idContextExcel, excelPackage);
            }
            catch (Exception ex)
            {

            }
            return Task.CompletedTask;
        }
    }
}
