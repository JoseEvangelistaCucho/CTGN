using Generacion.Application.Mantenimiento;
using Generacion.Models;
using Generacion.Models.Mantenimiento;
using MediatR;

namespace Generacion.Application.DashBoard.TKVessel.Command
{
    public class GuardarDetalleTKVessel : IRequest<Respuesta<string>>
    {
        public List<CilindroAceiteCarter> cilindroAceiteCarters { get; set; }
    }

    public class GuardarDetalleTKVesselHandler : IRequestHandler<GuardarDetalleTKVessel, Respuesta<string>>
    {
        private readonly IMantenimiento _mantenimiento;

        public GuardarDetalleTKVesselHandler(IMantenimiento mantenimiento)
        {
            _mantenimiento = mantenimiento;
        }
        public async Task<Respuesta<string>> Handle(GuardarDetalleTKVessel request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                foreach (var item in request.cilindroAceiteCarters)
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
                        respuesta = await _mantenimiento.GuardarDatosAceiteCarter(item);
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la solicitud";
            }

            return respuesta;
        }
    }
}
