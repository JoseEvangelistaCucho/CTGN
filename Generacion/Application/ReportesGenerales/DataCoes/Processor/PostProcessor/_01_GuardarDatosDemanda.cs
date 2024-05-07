using Generacion.Application.ReportesGenerales.DataCoes.Command;
using Generacion.Models.DataCoes;
using Generacion.Models;
using MediatR.Pipeline;

namespace Generacion.Application.ReportesGenerales.DataCoes.Processor.PostProcessor
{
    public class _01_GuardarDatosDemanda : IRequestPostProcessor<GuardarDatosCOES, Respuesta<List<Demanda>>>
    {
        private readonly IRegistroDataCoes _registroDataCoes;
        public _01_GuardarDatosDemanda(IRegistroDataCoes registroDataCoes)
        {
            _registroDataCoes = registroDataCoes;
        }
        public async Task Process(GuardarDatosCOES request, Respuesta<List<Demanda>> response, CancellationToken cancellationToken)
        {
            try
            {
                Respuesta<string> respuesta = new Respuesta<string>();
                foreach (var item in response.Detalle)
                {
                    if (respuesta.IdRespuesta == 99)
                        continue;

                    respuesta = await _registroDataCoes.GuardarDatosExcelReporte(item);
                }
            }
            catch (Exception ex)
            {
                response.IdRespuesta = 99;
                response.Mensaje = "Error al procesar la solicitud.";
            }
            
            Task.CompletedTask.Wait();
        }
    }
}
