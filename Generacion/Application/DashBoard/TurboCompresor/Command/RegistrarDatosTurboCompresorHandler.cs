using Generacion.Models;
using Generacion.Models.TurboCompresor;
using MediatR;

namespace Generacion.Application.DashBoard.TurboCompresor.Command
{
    public class RegistrarDatosTurboCompresor : IRequest<Respuesta<string>>
    {
        public List<DetalleTurboCompresor> detalleTurbo { get; set; }
        public DatosTurboCompresor datosTurbo { get; set; }
    }
    public class RegistrarDatosTurboCompresorHandler : IRequestHandler<RegistrarDatosTurboCompresor, Respuesta<string>>
    {
        private readonly IRegistroTurboCompresor _registroTurbo;
        public RegistrarDatosTurboCompresorHandler(IRegistroTurboCompresor registroTurbo)
        {
            _registroTurbo = registroTurbo;
        }

        public async Task<Respuesta<string>> Handle(RegistrarDatosTurboCompresor request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            string mensajesError = request.datosTurbo.ValidarPropiedadesNulasOVacias();
            if (mensajesError.Any())
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = mensajesError;
            }
            else
            {
                respuesta = await _registroTurbo.GuardarDatosTurboCompresor(request.datosTurbo);

                foreach (var item in request.detalleTurbo)
                {
                    if (respuesta.IdRespuesta == 99)
                        continue;

                     mensajesError = item.ValidarPropiedadesNulasOVacias();
                    if (mensajesError.Any())
                    {
                        respuesta.IdRespuesta = 99;
                        respuesta.Mensaje = mensajesError.Replace("<<>>",item.Tipo);
                    }
                    else
                    {
                        respuesta = await _registroTurbo.GuardarDetalleTurboCompresor(item);
                    }
                }
            }

            return respuesta;
        }
    }
}
