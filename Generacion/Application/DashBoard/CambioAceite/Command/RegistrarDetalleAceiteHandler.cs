using Generacion.Application.Common;
using Generacion.Models;
using Generacion.Models.Aceite;
using MediatR;

namespace Generacion.Application.DashBoard.CambioAceite.Command
{
    public class RegistrarDetalleAceite : IRequest<Respuesta<string>>
    {
        public List<DetalleControlAceite> detalleControlAceites { get; set; }
    }
    public class RegistrarDetalleAceiteHandler : IRequestHandler<RegistrarDetalleAceite, Respuesta<string>>
    {
        public readonly IRegistroControlAceite _registroControlAceite;
        public RegistrarDetalleAceiteHandler(
            IRegistroControlAceite registroControlAceite
            )
        {
            _registroControlAceite = registroControlAceite;
        }
        public async Task<Respuesta<string>> Handle(RegistrarDetalleAceite request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                foreach (var item in request.detalleControlAceites)
                {
                    if (respuesta.IdRespuesta == 99)
                        continue;


                    string mensajesError = item.ValidarPropiedadesNulasOVacias();
                    if (mensajesError.Any())
                    {
                        respuesta.IdRespuesta = 99;
                        respuesta.Mensaje = mensajesError;
                    }
                    else
                    {
                        respuesta = await _registroControlAceite.GuardarDetalleControlAceite(item);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }
    }
}
