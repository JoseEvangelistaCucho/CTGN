using Generacion.Application.Common;
using Generacion.Application.DataBase.cache;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.ReporteModulacion;
using MediatR;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Globalization;

namespace Generacion.Application.ReporteModulacion.Command
{
    public class GuardarCostosMarginalesQuery : IRequest<Respuesta<string>>
    {
        public string RutaArchivo { get; set; }
    }
    public class GuardarCostosMarginalesHandler : IRequestHandler<GuardarCostosMarginalesQuery, Respuesta<string>>
    {
        private readonly CacheDatos _cacheDatos;
        private readonly Function _function;
        public GuardarCostosMarginalesHandler(CacheDatos cacheDatos, Function function)
        {
            _function = function;
            _cacheDatos = cacheDatos;
        }
        public async Task<Respuesta<string>> Handle(GuardarCostosMarginalesQuery request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                List<CostoMarginal> listaCostoMarginal = new List<CostoMarginal>();

                using (var package = new ExcelPackage(new FileInfo(request.RutaArchivo)))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    int rowCount = worksheet.Dimension.Rows;

                    for (int i = 2; i <= rowCount; i++)
                    {
                        CostoMarginal costoMarginal = new CostoMarginal();

                        string[] formatos = { "dd/MM/yyyy H:mm:ss", "M/dd/yyyy H:mm:ss", "d/M/yy H:mm:ss" };
                        DateTime fecha;
                        if (DateTime.TryParseExact(worksheet.Cells[i, 1].Text, formatos, new CultureInfo("es-ES"), DateTimeStyles.None, out fecha))
                        {
                            costoMarginal.Fecha = fecha;
                            costoMarginal.Hora = fecha.ToString().Split(' ')[1];
                        }

                        costoMarginal.Costo = decimal.Parse(worksheet.Cells[i, 2].Text);
                        costoMarginal.CostoConvertido = decimal.Parse(worksheet.Cells[i, 2].Text);

                        listaCostoMarginal.Add(costoMarginal);
                    }
                }
                var datosOperario = await _function.ObtenerDatosOperario();

                string key = $"{Constante.seleccionCostosMarginales}_{datosOperario.IdSitio}";
                _cacheDatos.GuardarDatosCache(key, JsonConvert.SerializeObject(listaCostoMarginal));
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }
    }
}
