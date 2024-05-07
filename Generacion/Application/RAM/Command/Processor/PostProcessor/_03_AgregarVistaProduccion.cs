using Generacion.Application.Common;
using Generacion.Models;
using MediatR.Pipeline;
using OfficeOpenXml;

namespace Generacion.Application.RAM.Command.Processor.PostProcessor
{
    public class _03_AgregarVistaProduccion : IRequestPostProcessor<GuardarRegistroRAM, Respuesta<Dictionary<string, object>>>
    {
        private readonly ProcessExecutionContextExtensions _executionContextExtensions;

        public _03_AgregarVistaProduccion(ProcessExecutionContextExtensions executionContextExtensions)
        {
            _executionContextExtensions = executionContextExtensions;
        }
        public Task Process(GuardarRegistroRAM request, Respuesta<Dictionary<string, object>> response, CancellationToken cancellationToken)
        {
            try 
            {
                ExcelPackage excelPackage = GetExcelPackage();

                ExcelWorksheet worksheet = GetWorksheet(excelPackage, 1);

                int asciiLetraB = Constante.posicionLetraA+1;

                var index = 0;
                for (int i = asciiLetraB; i < 90; i = i + 2)
                {
                    char caracterAscii = (char)i;

                    worksheet.Cells[$"{caracterAscii.ToString()}22"].Value = request.DBRAM[index].ConsumoServiciosAuxiliares;
                    index++;
                }

                _executionContextExtensions.TryAdd("excelPackage", excelPackage);

                //SaveInformationNewView(excelPackage);

            }
            catch (Exception ex)
            {

            }
            return Task.CompletedTask;
        }
        private ExcelPackage GetExcelPackage()
        {
            return (ExcelPackage)_executionContextExtensions["excelPackage"];
        }
        private ExcelWorksheet GetWorksheet(ExcelPackage excelPackage, int worksheetIndex)
        {
            return excelPackage.Workbook.Worksheets[worksheetIndex];
        }
        private void SaveInformationNewView(ExcelPackage excelPackage)
        {
            ExcelWorksheet sourceWorksheet = GetWorksheet(excelPackage, 1);
            ExcelWorksheet destinationWorksheet = GetWorksheet(excelPackage, 2);

            string sourceRange = "A2:Y23";

            int destinationStartRow = 1;
            int destinationStartColumn = 1;

            CopyRange(sourceWorksheet, destinationWorksheet, sourceRange, destinationStartRow, destinationStartColumn);
            //RemoveView(excelPackage,1);
            //RemoveView(excelPackage,0);
        }

        private void CopyRange(ExcelWorksheet sourceWorksheet, ExcelWorksheet destinationWorksheet, string sourceRange, int destinationStartRow, int destinationStartColumn)
        {
            ExcelRange sourceRangeCells = sourceWorksheet.Cells[sourceRange];

            int numRows = sourceRangeCells.Rows;
            int numCols = sourceRangeCells.Columns;

            for (int i = 1; i <= numRows; i++)
            {
                for (int j = 1; j <= numCols; j++)
                {
                    ExcelRangeBase sourceCell = sourceRangeCells[i, j];
                    ExcelRangeBase destinationCell = destinationWorksheet.Cells[destinationStartRow + i - 1, destinationStartColumn + j - 1];

                    destinationCell.Value = sourceCell.Value;
                }
            }
        }
        private void RemoveView(ExcelPackage excelPackage, int worksheetIndex)
        {
            excelPackage.Workbook.Worksheets.Delete(worksheetIndex);
        }
    }
}
