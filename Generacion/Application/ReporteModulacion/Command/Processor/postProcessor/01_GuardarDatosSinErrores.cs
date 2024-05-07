using Generacion.Controllers;
using Generacion.Models;
using MediatR;
using MediatR.Pipeline;

namespace Generacion.Application.ReporteModulacion.Command.Processor.postProcessor
{
    public class _01_GuardarDatosSinErrores : IRequestPostProcessor<CompararFormatosCommand, Respuesta<List<DetalleFacturacionModulacion>>>
    {
        private readonly GuardarDatosModulacion _guardarDatos;
        public _01_GuardarDatosSinErrores(GuardarDatosModulacion guardarDatos)
        {
            _guardarDatos = guardarDatos;
        }

        public async Task Process(CompararFormatosCommand request, Respuesta<List<DetalleFacturacionModulacion>> response, CancellationToken cancellationToken)
        {
            try
            {
                if (response.Detalle == null || response.Detalle.Count == 0)
                {
                    var respuesta = await _guardarDatos.GuardarDetallesModulacion(DateTime.Now, request.Tc, request.ValoresPEMActuales);

                }

            }
            catch (Exception ex)
            {

            }


            Task.CompletedTask.Wait();
        }
    }

}
