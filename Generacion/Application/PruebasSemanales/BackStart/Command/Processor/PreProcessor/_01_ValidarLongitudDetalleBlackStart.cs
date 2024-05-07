using Generacion.Application.Common;
using Generacion.Models.PruebasSemanales.BlackStart;
using MediatR.Pipeline;

namespace Generacion.Application.PruebasSemanales.BackStart.Command.Processor.PreProcessor
{
    public class _01_ValidarLongitudDetallePruebaSemanal : IRequestPreProcessor<GuardarDatosPruebaSemanal>
    {
        private readonly ProcessExecutionContextExtensions _processcontext;
        public _01_ValidarLongitudDetallePruebaSemanal()
        {

        }
        public async Task Process(GuardarDatosPruebaSemanal request, CancellationToken cancellationToken)
        {

            if (request.PruebaSemanal != null)
            {
                request.PruebaSemanal.Activo = request.PruebaSemanal.Activo + 1;
            }
            else
            {
                if (request.DetallePruebaSemanal.Count>0)
                {
                    PruebaSemanal pruebaSemanal = new PruebaSemanal()
                    {
                        Activo = 1,
                        Fecha = request.DetallePruebaSemanal[0].Fecha,
                        IdPruebaSemanal = request.DetallePruebaSemanal[0].IdPruebaSemanal
                    };
                    request.PruebaSemanal = new PruebaSemanal();
                    request.PruebaSemanal = pruebaSemanal;
                }
            }
        }
    }
}
