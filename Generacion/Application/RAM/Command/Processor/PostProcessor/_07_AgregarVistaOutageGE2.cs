using Generacion.Application.Common;
using Generacion.Models.ReporteProduccion;
using Generacion.Models;
using MediatR.Pipeline;
using OfficeOpenXml;
using System.Net.WebSockets;
using Generacion.Models.Usuario;
using OfficeOpenXml.Style;

namespace Generacion.Application.RAM.Command.Processor.PostProcessor
{
    public class _07_AgregarVistaOutageGE2 : IRequestPostProcessor<GuardarRegistroRAM, Respuesta<Dictionary<string, object>>>
    {
        private readonly ProcessExecutionContextExtensions _executionContextExtensions;

        public _07_AgregarVistaOutageGE2(ProcessExecutionContextExtensions executionContextExtensions)
        {
            _executionContextExtensions = executionContextExtensions;
        }
        public Task Process(GuardarRegistroRAM request, Respuesta<Dictionary<string, object>> response, CancellationToken cancellationToken)
        {
            try
            {
                ExcelPackage excelPackage = (ExcelPackage)_executionContextExtensions["excelPackage"];
                DetalleOperario datosOperario = (DetalleOperario)_executionContextExtensions["datosOperario"];

                Dictionary<string, object> datosRAM = (Dictionary<string, object>)_executionContextExtensions["datosRAM"];
                Dictionary<int, Dictionary<DateTime, List<RegistroEventos>>> eventosPorFecha = (Dictionary<int, Dictionary<DateTime, List<RegistroEventos>>>)_executionContextExtensions["eventosPorFecha"];

                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[5];

                DateTime fechaActual = DateTime.Now;

                int indexAcumulado = 1;
                int inicioPosicion = 1;

                int posicionInicial = 0;

                worksheet.Cells["B1"].Value = $"{ReporteFormatoRAM.tituloOutage}{datosOperario.IdSitio} {fechaActual.ToString("yyyy")}";
                worksheet.Cells.Style.Font.Name = "Calibri";
                if (eventosPorFecha.ContainsKey(2))
                {
                    foreach (var item in eventosPorFecha[2])
                    {
                        if (item.Key.Month == DateTime.Now.Month)
                        {
                            int indexCasilla = 1;

                            for (int i = 0; i < worksheet.Dimension.End.Row; i++)
                            {
                                var cell = worksheet.Cells[$"B{indexAcumulado}"];
                                object cellValue = cell.Value;
                                string stringValue = (cellValue != null) ? cellValue.ToString() : "";
                                string busquedaAnterior = $"({fechaActual.AddMonths(-1).ToString("MMMM")} {fechaActual.ToString("yyyy")})";
                                string busqueda = $"({fechaActual.ToString("MMMM")} {fechaActual.ToString("yyyy")})";

                                indexAcumulado++;

                                if (stringValue.Contains(busquedaAnterior))
                                {
                                    posicionInicial = i + 2;
                                }

                                if (stringValue.Contains(busqueda))
                                {
                                    int descuentoExistente = posicionInicial != 0 ? i - posicionInicial : 0;

                                    i = worksheet.Dimension.End.Row + item.Value.Count;

                                    indexAcumulado = posicionInicial == 0 ? indexAcumulado - 2 : posicionInicial;
                                    inicioPosicion = indexAcumulado;

                                    int cantidadCeldas = item.Value.Count - descuentoExistente - 1;
                                    if (cantidadCeldas != 0)
                                    {
                                        worksheet.InsertRow((indexAcumulado + 1), cantidadCeldas);
                                    }
                                }
                            }

                            foreach (var detalle in item.Value)
                            {
                                worksheet.Cells[$"A{indexAcumulado}"].Value = indexCasilla;
                                AplicarBordes(worksheet.Cells[$"A{indexAcumulado}"].Style.Border);

                                worksheet.Cells[$"B{indexAcumulado}"].Value = detalle.FechaParada;
                                AplicarBordes(worksheet.Cells[$"B{indexAcumulado}"].Style.Border);

                                worksheet.Cells[$"C{indexAcumulado}"].Value = detalle.FechaArranque;
                                AplicarBordes(worksheet.Cells[$"C{indexAcumulado}"].Style.Border);

                                worksheet.Cells[$"D{indexAcumulado}"].Value = detalle.Sistema;
                                AplicarBordes(worksheet.Cells[$"D{indexAcumulado}"].Style.Border);

                                worksheet.Cells[$"E{indexAcumulado}"].Value = detalle.UnidadFuncional;
                                AplicarBordes(worksheet.Cells[$"E{indexAcumulado}"].Style.Border);

                                worksheet.Cells[$"F{indexAcumulado}"].Value = TimeSpan.Parse(detalle.ExternalTrips).TotalDays;
                                worksheet.Cells[$"F{indexAcumulado}"].Style.Numberformat.Format = Constante.formatoExcelHora;
                                AplicarBordes(worksheet.Cells[$"F{indexAcumulado}"].Style.Border);

                                worksheet.Cells[$"G{indexAcumulado}"].Value = TimeSpan.Parse(detalle.ForcedMaint).TotalDays;
                                worksheet.Cells[$"G{indexAcumulado}"].Style.Numberformat.Format = Constante.formatoExcelHora;
                                AplicarBordes(worksheet.Cells[$"G{indexAcumulado}"].Style.Border);

                                worksheet.Cells[$"H{indexAcumulado}"].Value = TimeSpan.Parse(detalle.PlannedMaint).TotalDays;
                                worksheet.Cells[$"H{indexAcumulado}"].Style.Numberformat.Format = Constante.formatoExcelHora;
                                AplicarBordes(worksheet.Cells[$"H{indexAcumulado}"].Style.Border);

                                worksheet.Cells[$"I{indexAcumulado}"].Value = TimeSpan.Parse(detalle.StandBy).TotalDays;
                                worksheet.Cells[$"I{indexAcumulado}"].Style.Numberformat.Format = Constante.formatoExcelHora;
                                AplicarBordes(worksheet.Cells[$"I{indexAcumulado}"].Style.Border);

                                worksheet.Cells[$"J{indexAcumulado}"].Value = detalle.DescripcionEvento;
                                AplicarBordes(worksheet.Cells[$"J{indexAcumulado}"].Style.Border);

                                indexAcumulado++;
                                indexCasilla++;
                            }

                            worksheet.Cells[$"B{indexAcumulado}"].Style.Font.Name = "Arial";

                            worksheet.Cells[$"F{indexAcumulado}"].Formula = $"=SUM(F{inicioPosicion}:F{inicioPosicion + item.Value.Count - 1})";
                            worksheet.Cells[$"F{indexAcumulado}"].Style.Font.Name = "Arial";
                            worksheet.Cells[$"G{indexAcumulado}"].Formula = $"=SUM(G{inicioPosicion}:G{inicioPosicion + item.Value.Count - 1})";
                            worksheet.Cells[$"G{indexAcumulado}"].Style.Font.Name = "Arial";
                            worksheet.Cells[$"H{indexAcumulado}"].Formula = $"=SUM(H{inicioPosicion}:H{inicioPosicion + item.Value.Count - 1})";
                            worksheet.Cells[$"H{indexAcumulado}"].Style.Font.Name = "Arial";
                            worksheet.Cells[$"I{indexAcumulado}"].Formula = $"=SUM(I{inicioPosicion}:I{inicioPosicion + item.Value.Count - 1})";
                            worksheet.Cells[$"I{indexAcumulado}"].Style.Font.Name = "Arial";
                        }
                    }

                    ExcelRange columnas = worksheet.Cells[worksheet.Dimension.Address];
                    columnas.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                _executionContextExtensions.TryAdd(Constante.idContextExcel, excelPackage);
                //excelPackage.Save();

                response.Detalle = new Dictionary<string, object>();
                response.Detalle.Add(Constante.idContextExcel, excelPackage);

            }
            catch (Exception ex)
            {

            }
            return Task.CompletedTask;
        }

        void AplicarBordes(Border border)
        {
            border.Top.Style = ExcelBorderStyle.Thin;
            border.Left.Style = ExcelBorderStyle.Thin;
            border.Right.Style = ExcelBorderStyle.Thin;
            border.Bottom.Style = ExcelBorderStyle.Thin;
        }
    }
}
