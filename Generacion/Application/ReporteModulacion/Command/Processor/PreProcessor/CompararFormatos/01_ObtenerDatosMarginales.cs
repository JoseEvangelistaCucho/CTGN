using Generacion.Application.Common;
using Generacion.Application.DataBase.cache;
using Generacion.Application.Funciones;
using Generacion.Application.ReporteModulacion.Command;
using Generacion.Models.ReporteModulacion;
using MediatR.Pipeline;
using Newtonsoft.Json;

namespace Generacion.Application.ReporteModulacion.Command.Processor.PreProcessor.CompararFormatos
{
    public class _01_ObtenerDatosMarginales : IRequestPreProcessor<CompararFormatosCommand>
    {
        private readonly ProcessExecutionContextExtensions _context;
        private readonly Function _function;
        private readonly CacheDatos _cacheDatos;

        public _01_ObtenerDatosMarginales(Function function, ProcessExecutionContextExtensions context, CacheDatos cacheDatos)
        {
            _function = function;
            _context = context;
            _cacheDatos = cacheDatos;
        }
        public async Task Process(CompararFormatosCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();

                string key = $"{Constante.seleccionCostosMarginales}_{datosOperario.IdSitio}";

                string datosCSV = _cacheDatos.ObtenerContenidoCache(key);
                List<CostoMarginal> datos = JsonConvert.DeserializeObject<List<CostoMarginal>>(datosCSV);

                datos.ForEach(c => c.CostoConvertido = RedondearHaciaArribaSiMayorCinco((c.CostoConvertido / request.Tc * 1000).ToString()));

                _context.TryAdd("costosMarginales", datos);
            }
            catch (Exception ex)
            {

            }
            Task.CompletedTask.Wait();
        }

        public static decimal RedondearHaciaArribaSiMayorCinco(string valor)
        {
            int indice = valor.IndexOf('.') + 1;
            int digito = valor[indice + 2];

            decimal valorRedondeado = 0;
            if (digito > 4)
            {
                valorRedondeado = Math.Round(decimal.Parse(valor), 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                valorRedondeado = Math.Ceiling(decimal.Parse(valor) * 100) / 100;
            }

            return valorRedondeado;
        }
    }

}
