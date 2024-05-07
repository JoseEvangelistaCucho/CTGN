using Generacion.Models;
using MediatR.Pipeline;
using Generacion.Application.ReportesGenerales.DataBess.Command;
using Generacion.Models.Bess;
using Generacion.Application.ReportesGenerales.DataCoes.Command;

namespace Generacion.Application.ReportesGenerales.DataBess.Processor
{
    public class _01_GuardarDatosBess : IRequestPostProcessor<GuardarDatosBess, Respuesta<List<DBDataBess>>>
    {
        private readonly IRegistroDataBess _registroDataBess;
        public _01_GuardarDatosBess(IRegistroDataBess registroDataBess)
        {
            _registroDataBess = registroDataBess;
        }
        public async Task Process(GuardarDatosBess request, Respuesta<List<DBDataBess>> response, CancellationToken cancellationToken)
        {
            try
            {
                Respuesta<string> respuesta = new Respuesta<string>();
                foreach (var item in response.Detalle)
                {
                    if (respuesta.IdRespuesta == 99)
                        continue;

                    respuesta = await _registroDataBess.GuardarDatosArchivosText(item);
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
