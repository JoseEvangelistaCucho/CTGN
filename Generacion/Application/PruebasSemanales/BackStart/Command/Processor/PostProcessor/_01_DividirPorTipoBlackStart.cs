using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using MediatR.Pipeline;

namespace Generacion.Application.PruebasSemanales.BackStart.Command.Processor.PostProcessor
{
    public class _01_DividirPorTipoBlackStart : IRequestPostProcessor<GuardarDatosPruebaSemanal, Respuesta<object>>
    {
        private readonly Function _function;
        private readonly HashSet<string> unidadesAdjuntas = new HashSet<string> { "%" };

        public _01_DividirPorTipoBlackStart(Function function)
        {
            _function = function;
        }

        public async Task Process(GuardarDatosPruebaSemanal request, Respuesta<object> response, CancellationToken cancellationToken)
        {
            if (request.Tipo.Equals("BLACKSTART"))
            {
                response.Detalle = new object();

                var datosCabecera = await _function.ObtenerDatosCabecera();
                HashSet<string> listaMotor = new HashSet<string>(datosCabecera.Values.Where(x => x.TablaReference.Equals("blackStartMotor")).Select(x => x.IdTipoEngine));
                HashSet<string> listaGenerador = new HashSet<string>(datosCabecera.Values.Where(x => x.TablaReference.Equals("blackStartGenerador")).Select(x => x.IdTipoEngine));

                var datosAgrupados = new Dictionary<string, List<datosJsonPruebaSemanal>>();

                foreach (var item in request.DetallePruebaSemanal)
                {
                    var cabeceraParseado = datosCabecera.Values.FirstOrDefault(x => x.IdTipoEngine.Equals(item.IdSubtitulo));

                    var valorParseado = string.IsNullOrEmpty(item.DetalleCadena) ? item.DetalleNumerico.ToString() : item.DetalleCadena;
                    var unidadAdjunto = unidadesAdjuntas.Contains(cabeceraParseado.Descripcion) ? cabeceraParseado.Descripcion : string.Empty;

                    var datosJsonPruebaSemanal = new datosJsonPruebaSemanal
                    {
                        CabecerasTabla = cabeceraParseado,
                        DetalleCadena = $"{valorParseado}{unidadAdjunto}",
                        Fecha = item.Fecha,
                        Observaciones = item.Observaciones,
                        ValorReferencia = item.ValorReferencia
                    };

                    var key = listaMotor.Contains(item.IdSubtitulo) ? "motor" : listaGenerador.Contains(item.IdSubtitulo) ? "generador" : null;

                    if (key != null)
                    {
                        if (!datosAgrupados.ContainsKey(key))
                        {
                            datosAgrupados[key] = new List<datosJsonPruebaSemanal>();
                        }

                        datosAgrupados[key].Add(datosJsonPruebaSemanal);
                    }
                }

                response.Detalle = datosAgrupados;
            }

            await Task.CompletedTask;
        }
    }
    public class datosJsonPruebaSemanal
    {
        public CabecerasTabla CabecerasTabla { get; set; }
        public string ValorReferencia { get; set; }
        public string DetalleCadena { get; set; }
        public string Observaciones { get; set; }
        public string Fecha { get; set; }
    }
}
