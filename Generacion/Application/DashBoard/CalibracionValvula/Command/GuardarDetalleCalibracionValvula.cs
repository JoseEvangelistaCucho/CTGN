using Generacion.Models;
using Generacion.Models.CalibracionValvula;
using MediatR;

namespace Generacion.Application.DashBoard.CalibracionValvula.Command
{
    public class GuardarDetalleCalibracionValvula : IRequest<Respuesta<string>>
    {
        public List<DetalleCalibracionValvula> detalles { get; set; }
    }

    public class GuardarDetalleCalibracionValvulaHandler : IRequestHandler<GuardarDetalleCalibracionValvula, Respuesta<string>>
    {
        private readonly IRegistroCalibracionValvula _calibracionValvula;
        public GuardarDetalleCalibracionValvulaHandler(IRegistroCalibracionValvula calibracionValvula)
        {
            _calibracionValvula = calibracionValvula;
        }
        public async Task<Respuesta<string>> Handle(GuardarDetalleCalibracionValvula request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                foreach (var item in request.detalles)
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
                        respuesta = await _calibracionValvula.GuardarDatosCalibracionValvula(item);
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
