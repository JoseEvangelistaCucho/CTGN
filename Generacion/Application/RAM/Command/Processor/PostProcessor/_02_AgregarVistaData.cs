using Generacion.Application.Common;
using Generacion.Models;
using Generacion.Models.ReporteDiarioGAS;
using Generacion.Models.ReporteProduccion;
using MediatR.Pipeline;
using OfficeOpenXml;

namespace Generacion.Application.RAM.Command.Processor.PostProcessor
{
    public class _02_AgregarVistaData : IRequestPostProcessor<GuardarRegistroRAM, Respuesta<Dictionary<string, object>>>
    {
        private readonly ProcessExecutionContextExtensions _executionContextExtensions;

        public _02_AgregarVistaData(ProcessExecutionContextExtensions executionContextExtensions)
        {
            _executionContextExtensions = executionContextExtensions;
        }
        public Task Process(GuardarRegistroRAM request, Respuesta<Dictionary<string, object>> response, CancellationToken cancellationToken)
        {
            try
            {
                Dictionary<string, object> datosRAM = (Dictionary<string, object>)_executionContextExtensions["datosRAM"];
                ExcelPackage excelPackage = (ExcelPackage)_executionContextExtensions["excelPackage"];

                //ReporteFormatoRAM   "datosGeneralesProduccion"
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];

                DateTime fechaActual = DateTime.Now;

                int totalDias = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                int inicioCasilla = DiasAcumulados();

                ///Gen. act.energy 
                DateTime startDateOfMonth = new DateTime(fechaActual.Year, fechaActual.Month, 1);
                Dictionary<DateTime, List<EnergiaProducida>> data = (Dictionary<DateTime, List<EnergiaProducida>>)datosRAM["datosGeneralesProduccion"];
                Dictionary<DateTime, Dictionary<int, List<ReporteProduccionStatus>>> datosDetalleProduccion = (Dictionary<DateTime, Dictionary<int, List<ReporteProduccionStatus>>>)datosRAM["datosDetalleProduccion"];
                var indexData = 0;

                //Horas de operación acumuladas Gen
                List<EnergiaProducida> dataActual = data.FirstOrDefault(pair => pair.Key.Date == startDateOfMonth).Value;
                Dictionary<int, List<ReporteProduccionStatus>> detalleActualProd = datosDetalleProduccion.FirstOrDefault(pair => pair.Key.Date == startDateOfMonth).Value;
                var indexDetalle = 0;

                //Gen. Consumo de gas en kg
                List<CityGateFlow> cityGateToday = datosRAM["cityGateToday"] as List<CityGateFlow>;
                List<CityGateFlow> cityGateYesterday = datosRAM["cityGateYesterday"] as List<CityGateFlow>;
                cityGateToday = cityGateToday.Where(x => DateTime.Parse(x.Fecha).Month == fechaActual.Month).Select(x => x).ToList();
                cityGateYesterday = cityGateYesterday.Where(x => DateTime.Parse(x.Fecha).Month == fechaActual.Month).Select(x => x).ToList();
                var indexGasGe= 0;

                //ION
                List<ReporteProducion> datosION = datosRAM["datosION"] as List<ReporteProducion>;
                datosION = datosION.Where(x => DateTime.Parse(x.Fecha).Month == fechaActual.Month).Select(x => x).ToList();
                var indexION = 0;

                //Consumo gas sm3
                List<DetalleReporteGas> reporteGas = datosRAM["reporteGas"] as List<DetalleReporteGas>;
                reporteGas = reporteGas.Where(x => DateTime.Parse(x.Fecha).Month == fechaActual.Month).Select(x => x).ToList();
                var indexGas = 0;


                //ACEITE
                Dictionary<string, List<LevelLubeOilCartel>> datosCarter = datosRAM["datosAceite"] as Dictionary<string, List<LevelLubeOilCartel>>;
                List<LevelLubeOilCartel> nivelAceiteGe1 = datosCarter["TODAY"].Where(x => DateTime.Parse(x.Fecha).Month == fechaActual.Month).Select(x => x).ToList();
                List<LevelLubeOilCartel> nivelAceiteGe2 = datosCarter["TODAY"].Where(x => DateTime.Parse(x.Fecha).Month == fechaActual.Month).Select(x => x).ToList();
                List<LevelLubeOilCartel> nivelAceiteGalonGe1 = datosCarter["ADDED"].Where(x => DateTime.Parse(x.Fecha).Month == fechaActual.Month).Select(x => x).ToList();
                List<LevelLubeOilCartel> nivelAceiteGalonGe2 = datosCarter["ADDED"].Where(x => DateTime.Parse(x.Fecha).Month == fechaActual.Month).Select(x => x).ToList();
                var indexAceite = 0;

                //"cityGateToday" "cityGateYesterday"

                for (int casillas = inicioCasilla + 1; casillas <= totalDias + inicioCasilla; casillas++)
                {
                    //Energia Generada
                    if (dataActual.Count > indexData)
                    {
                        worksheet.Cells[$"B{casillas}"].Value = dataActual[indexData].PmuEng_01;
                        worksheet.Cells[$"C{casillas}"].Value = dataActual[indexData].PmuEng_02;
                        indexData++;
                    }

                    //Horas Operacion
                    if (detalleActualProd[1].Count > indexDetalle && detalleActualProd[2].Count > indexDetalle)
                    {
                        DateTime fechaDetalle = DateTime.Parse(detalleActualProd[1][indexDetalle].Fecha);

                        if (casillas >= fechaDetalle.Day + inicioCasilla)
                        {
                            worksheet.Cells[$"E{casillas}"].Value = detalleActualProd[1][indexDetalle].RunningHours;
                            worksheet.Cells[$"F{casillas}"].Value = detalleActualProd[2][indexDetalle].RunningHours;
                            indexDetalle++;
                        }
                    }

                    //Consumo Gas
                    if (cityGateToday.Count > indexGasGe && cityGateYesterday.Count > indexGasGe)
                    {
                        DateTime fechaDetalleGas = DateTime.Parse(cityGateToday[indexGasGe].Fecha);

                        if (casillas >= fechaDetalleGas.Day + inicioCasilla)
                        {
                            worksheet.Cells[$"G{casillas}"].Value = cityGateToday[indexGasGe].KgEng1 - cityGateYesterday[indexGasGe].KgEng1;
                            worksheet.Cells[$"H{casillas}"].Value = cityGateToday[indexGasGe].KgEng2 - cityGateYesterday[indexGasGe].KgEng2;
                            indexGasGe++;
                        }
                    }

                    //ION
                    if (datosION.Count > indexION && datosION.Count > indexION)
                    {
                        DateTime fechaDetalleION = DateTime.Parse(datosION[indexION].Fecha);

                        if (casillas >= fechaDetalleION.Day + inicioCasilla)
                        {
                            worksheet.Cells[$"I{casillas}"].Value = datosION[indexION].TotalPlantExportION;
                            indexION++;
                        }
                    }

                    //GasTotal
                    if (reporteGas.Count > indexGas && reporteGas.Count > indexGas)
                    {
                        DateTime fechaDetalleGas = DateTime.Parse(reporteGas[indexGas].Fecha);

                        if (casillas >= fechaDetalleGas.Day + inicioCasilla)
                        {
                            worksheet.Cells[$"J{casillas}"].Value = reporteGas[indexGas].DP;
                            indexGas++;
                        }
                    }
                    
                    //AceiteTotal
                    if (nivelAceiteGe1.Count > indexAceite )
                    {
                        DateTime fechaDetalleAceite = DateTime.Parse(nivelAceiteGe1[indexAceite].Fecha);

                        if (casillas >= fechaDetalleAceite.Day + inicioCasilla)
                        {
                            worksheet.Cells[$"K{casillas}"].Value = nivelAceiteGe1[indexAceite].Generador1;
                            worksheet.Cells[$"L{casillas}"].Value = nivelAceiteGe2[indexAceite].Generador2;
                            worksheet.Cells[$"M{casillas}"].Value = nivelAceiteGalonGe1[indexAceite].Generador1;
                            worksheet.Cells[$"N{casillas}"].Value = nivelAceiteGalonGe2[indexAceite].Generador2;
                            indexAceite++;
                        }
                    }


                }
                _executionContextExtensions.TryAdd(Constante.idContextExcel, excelPackage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al manipular el archivo Excel: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public int DiasAcumulados()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            int acumuladoDiasAnteriores = 0;

            for (int m = 1; m < month; m++)
            {
                int cabecera = m == 1 ? ReporteFormatoRAM.espacioCabeceraVistaData : ReporteFormatoRAM.espacioCabeceraVistaData + ReporteFormatoRAM.espacioTablaVistaData;

                acumuladoDiasAnteriores += DateTime.DaysInMonth(year, m) + cabecera;
            }

            int cabeceraInicial = month == 1 ? ReporteFormatoRAM.espacioCabeceraVistaData : ReporteFormatoRAM.espacioCabeceraVistaData + ReporteFormatoRAM.espacioTablaVistaData;

            acumuladoDiasAnteriores = acumuladoDiasAnteriores + cabeceraInicial;

            return acumuladoDiasAnteriores;
        }
    }
}
