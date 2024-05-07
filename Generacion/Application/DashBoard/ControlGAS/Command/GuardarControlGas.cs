using Generacion.Models;
using Generacion.Models.ControlGAS;
using MediatR;

namespace Generacion.Application.DashBoard.ControlGAS.Command
{
    public class GuardarControlGas : IRequest<Respuesta<string>>
    {
        public ConsumoGas consumoGas { get; init; }
    }

    public class GuardarControlGasHandler : IRequestHandler<GuardarControlGas, Respuesta<string>>
    {
        private readonly IRegistroControlGas _registroControlGas;

        public GuardarControlGasHandler(IRegistroControlGas registroControlGas)
        {
            _registroControlGas = registroControlGas;
        }
        public async Task<Respuesta<string>> Handle(GuardarControlGas request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                string mensajesError = request.consumoGas.ValidarPropiedadesNulasOVacias();

                if (mensajesError.Any())
                {
                    respuesta.IdRespuesta = 99;
                    respuesta.Mensaje = mensajesError;
                }
                else
                {
                    respuesta = await _registroControlGas.RegistrarDatosControl(request.consumoGas);
                }
            }
            catch (Exception e)
            {

            }
            return respuesta;
        }
    }
}
