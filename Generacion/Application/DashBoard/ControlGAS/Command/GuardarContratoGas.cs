using Generacion.Models;
using Generacion.Models.ControlGAS;
using MediatR;

namespace Generacion.Application.DashBoard.ControlGAS.Command
{
    public class GuardarContratoGas : IRequest<Respuesta<string>>
    {
        public ContratoGas contratoGas { get; init; }
    }
    public class RegistroControlGasHandler : IRequestHandler<GuardarContratoGas, Respuesta<string>>
    {
        private readonly IRegistroControlGas _registroControlGas;
        public RegistroControlGasHandler(IRegistroControlGas registroControlGas)
        {
            _registroControlGas = registroControlGas;
        }
        public async Task<Respuesta<string>> Handle(GuardarContratoGas request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                string mensajesError = request.contratoGas.ValidarPropiedadesNulasOVacias();

                if (mensajesError.Any())
                {
                    respuesta.IdRespuesta = 99;
                    respuesta.Mensaje = mensajesError;
                }
                else
                {
                    respuesta = await _registroControlGas.RegistrarContratosControl(request.contratoGas);
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la solicitud.";
            }

            return respuesta;
        }
    }
}
