using Generacion.Application.Funciones;
using Generacion.Models;
using MediatR.Pipeline;

namespace Generacion.Application.PruebasSemanales.BackStart.Command.Processor.PostProcessor
{
    public class _02_DividirPorTipoSistemaIncendios : IRequestPostProcessor<GuardarDatosPruebaSemanal, Respuesta<object>>
    {
        private readonly Function _function;
        private readonly HashSet<string> unidadesAdjuntas = new HashSet<string> { "%" };

        public _02_DividirPorTipoSistemaIncendios(Function function)
        {
            _function = function;
        }

        public async Task Process(GuardarDatosPruebaSemanal request, Respuesta<object> response, CancellationToken cancellationToken)
        {
            if (request.Tipo.Equals("SISTEMAINCENDIOS"))
            {
                response.Detalle = new object();

                var datosCabecera = await _function.ObtenerDatosCabecera();
                HashSet<string> listaBombaElectrica = new HashSet<string>(datosCabecera.Values.Where(x => x.TablaReference.Equals("SistContIncenBombElec")).Select(x => x.IdTipoEngine));
                HashSet<string> listaBombaDiesel = new HashSet<string>(datosCabecera.Values.Where(x => x.TablaReference.Equals("SistContIncenBombDies")).Select(x => x.IdTipoEngine));
                HashSet<string> listaBombaJockey = new HashSet<string>(datosCabecera.Values.Where(x => x.TablaReference.Equals("SistContIncenBombJoc")).Select(x => x.IdTipoEngine));
                HashSet<string> listaNivelTanque = new HashSet<string>(datosCabecera.Values.Where(x => x.TablaReference.Equals("SistContIncenNvlTnq")).Select(x => x.IdTipoEngine));

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

                    var key = listaBombaElectrica.Contains(item.IdSubtitulo) ? "bombaElectrica" : listaBombaDiesel.Contains(item.IdSubtitulo) ? "bombaDiesel" : listaBombaJockey.Contains(item.IdSubtitulo) ? "bombaJockey" : listaNivelTanque.Contains(item.IdSubtitulo) ? "nivelTanque" : null;

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
}
