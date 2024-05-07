using Generacion.Controllers;
using Generacion.Models;
using Generacion.Models.DataCoes;
using MediatR;
using OfficeOpenXml;
using System.Globalization;

namespace Generacion.Application.ReportesGenerales.DataCoes.Command
{
    public class GuardarDatosCOES : IRequest<Respuesta<List<Demanda>>>
    {
        public string path { get; set; }
    }
    public class GuardarDatosCOESHandler : IRequestHandler<GuardarDatosCOES, Respuesta<List<Demanda>>>
    {
        //se coloca en duro las filas ya que no se obtiene las 3 ultimas filas
        private const int _cantidadFilas = 1492;
        public GuardarDatosCOESHandler()
        {

        }
        public async Task<Respuesta<List<Demanda>>> Handle(GuardarDatosCOES request, CancellationToken cancellationToken)
        {
            Respuesta<List<Demanda>> respuesta = new Respuesta<List<Demanda>>();
            try
            {
                respuesta.Detalle = new List<Demanda>();

                using (var package = new ExcelPackage(new FileInfo(request.path)))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    int rowCount = worksheet.Dimension.Rows + 3;

                    //for (int i = 5; i <= _cantidadFilas; i++)
                    for (int i = 5; i <= rowCount; i++)
                    {
                        Demanda demanda = new Demanda();

                        string[] formatos = { "dd/MM/yyyy H:mm", "M/dd/yyyy H:mm", "d/M/yy H:mm" };
                        DateTime fecha;
                        if (DateTime.TryParseExact(worksheet.Cells[i, 1].Text, formatos, new CultureInfo("es-ES"), DateTimeStyles.None, out fecha))
                        {
                            //Columna A
                            demanda.Fecha = fecha;
                            demanda.Hora = fecha.ToString().Split(' ')[1];

                        }
                        //Columna B
                        demanda.Ejecutado = decimal.Parse(worksheet.Cells[i, 2].Text);
                        //Columna C
                        demanda.ProgramacionDiaria = decimal.Parse(worksheet.Cells[i, 3].Text);
                        //Columna D
                        demanda.ProgramacionSemanal = decimal.Parse(worksheet.Cells[i, 4].Text);

                        respuesta.Detalle.Add(demanda);
                    }
                    return respuesta;
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
    }
}
