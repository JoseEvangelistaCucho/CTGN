using Generacion.Application.Common;
using Generacion.Application.DataBase.cache;
using Generacion.Application.Funciones;
using Generacion.Application.ReporteModulacion.Command;
using Generacion.Controllers;
using Generacion.Models.ReporteModulacion;
using MediatR.Pipeline;
using Newtonsoft.Json;

namespace Generacion.Application.ReporteModulacion.Command.Processor.PreProcessor.CompararFormatos
{
    public class _02_ObtenerFacturacionModulacion : IRequestPreProcessor<CompararFormatosCommand>
    {
        private readonly ProcessExecutionContextExtensions _context;
        private readonly Function _function;
        private readonly CacheDatos _cacheDatos;

        public _02_ObtenerFacturacionModulacion(Function function, ProcessExecutionContextExtensions context, CacheDatos cacheDatos)
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

                string key = $"{Constante.facturaModulacion}_{datosOperario.IdSitio}";

                string datosCSV = _cacheDatos.ObtenerContenidoCache(key);

                FacturacionModulacion datos = JsonConvert.DeserializeObject<FacturacionModulacion>(datosCSV);

                _context.TryAdd("facutarcionModulacion", datos);

            }
            catch (Exception ex)
            {

            }
            Task.CompletedTask.Wait();
        }

    }

}
