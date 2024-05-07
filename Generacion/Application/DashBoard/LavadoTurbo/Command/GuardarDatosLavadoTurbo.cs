using Generacion.Models;
using Generacion.Models.TurboCompresor;
using MediatR;

namespace Generacion.Application.DashBoard.LavadoTurbo.Command
{
    public class GuardarDatosLavadoTurbo : IRequest<Respuesta<string>>
    {
        public DatoLavadoTurbo lavadoTurbo { get; set; }
        public List<DetalleLavadoTurbo>  detalleTurbo { get; set; }
    }

    public class GuardarDatosLavadoTurboHanlder : IRequestHandler<GuardarDatosLavadoTurbo, Respuesta<string>>
    {
        private readonly IRegistroLavadoTurbo _registroLavadoTurbo;
        public GuardarDatosLavadoTurboHanlder(IRegistroLavadoTurbo registroLavadoTurbo)
        {
            _registroLavadoTurbo = registroLavadoTurbo;
        }
        public async Task<Respuesta<string>> Handle(GuardarDatosLavadoTurbo request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                respuesta = await _registroLavadoTurbo.GuardarDatosTurboLavado(request.lavadoTurbo);
                if(respuesta.IdRespuesta != 99)
                {
                    foreach (var item in request.detalleTurbo)
                    {
                        if (respuesta.IdRespuesta == 99) 
                            continue; 

                        string mensajeError = item.ValidarPropiedadesNulasOVacias();
                        if (mensajeError.Any())
                        {
                            respuesta.IdRespuesta = 99;
                            respuesta.Mensaje = mensajeError;
                        }
                        else
                        {
                            respuesta = await _registroLavadoTurbo.GuardarDetalleTurboLavado(item);
                        }
                    }

                    
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
