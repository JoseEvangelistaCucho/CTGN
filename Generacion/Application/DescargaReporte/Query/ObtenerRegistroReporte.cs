using MediatR;
using Generacion.Models;
using Generacion.Application.Common;
using OfficeOpenXml;
using Generacion.Models.Bess;
using Generacion.Models.DataCoes;

namespace Generacion.Application.DescargaReporte.Query
{
    public class ObtenerRegistroReporte : IRequest<Respuesta<Dictionary<string, object>>>
    {
        public string Fecha { get; set; }
        public string TipoReporte { get; set; }
    }

    public class ObtenerRegistroReporteHandler : IRequestHandler<ObtenerRegistroReporte, Respuesta<Dictionary<string, object>>>
    {
        public ProcessExecutionContextExtensions _context;
        public ObtenerRegistroReporteHandler(ProcessExecutionContextExtensions context)
        {
            _context = context;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerRegistroReporte request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();

            try
            {

                ExcelPackage excelPackage = GetExcelPackage();

                ExcelWorksheet worksheet = GetWorksheet(excelPackage, 0);
                Dictionary<string, List<Demanda>> coes = GetDataCoes();



                ConfigureCells(worksheet, coes);


                respuesta.Detalle = new Dictionary<string, object>();
                respuesta.Detalle.Add("excelPackage", excelPackage);


            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
        private ExcelPackage GetExcelPackage()
        {
            return (ExcelPackage)_context["excelPackage"];
        }
        private Dictionary<string, List<Demanda>> GetDataCoes()
        {
            return (Dictionary<string, List<Demanda>>)_context["DatosCoes"];
        }
        private List<DBDataBess> GetDataBess()
        {
            return (List<DBDataBess>)_context["listaDataBESS"];
        }
        private ExcelWorksheet GetWorksheet(ExcelPackage excelPackage, int worksheetIndex)
        {
            return excelPackage.Workbook.Worksheets[worksheetIndex];
        }
        private void ConfigureCells(ExcelWorksheet worksheet, Dictionary<string, List<Demanda>> demandaCoes)
        {
            int asciiLetraI = Constante.posicionLetraA + 8;
            int index = 0;

          /*  foreach (var item in demandaCoes)
            {
                var sumaPorMes = item.Value.Sum(x => x.Ejecutado);

                char caracterAscii = (char)(asciiLetraI + index);

                worksheet.Cells[$"{caracterAscii.ToString()}2"].Value = sumaPorMes;
                index++;
                //  worksheet.Cells[$"A{indexAcumulado}"].Value = indexCasilla;
                //  AplicarBordes(worksheet.Cells[$"A{indexAcumulado}"].Style.Border);
            }*/

            Dictionary<string, decimal> sumaPorMes = new Dictionary<string, decimal>();

            // Iterar sobre cada entrada del diccionario de demandas por día
            foreach (var kvp in demandaCoes)
            {
                // Obtener el mes de la fecha
                string mes = DateTime.Parse(kvp.Key).ToString("MM/yyyy");

                // Sumar los valores de la lista de demandas
                decimal suma = kvp.Value.Sum(demanda => demanda.Ejecutado);

                // Agregar la suma al diccionario de suma por mes
                if (!sumaPorMes.ContainsKey(mes))
                {
                    sumaPorMes[mes] = suma;
                }
                else
                {
                    sumaPorMes[mes] += suma;
                }
            }

            // Imprimir los resultados
            foreach (var kvp in sumaPorMes)
            {
                char caracterAscii = (char)(asciiLetraI + index);
                worksheet.Cells[$"{caracterAscii.ToString()}2"].Value = kvp.Value;
                index++;
            }


        }
    }
}
