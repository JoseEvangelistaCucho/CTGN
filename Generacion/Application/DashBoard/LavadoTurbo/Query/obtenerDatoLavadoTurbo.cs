using Generacion.Models;
using Generacion.Models.TurboCompresor;
using MediatR;

namespace Generacion.Application.DashBoard.LavadoTurbo.Query
{
    public class obtenerDatoLavadoTurbo : IRequest<Respuesta<DatoLavadoTurbo>>
    {
    }

    public class ObtenerDatoLavadoTurboHandler : IRequestHandler<obtenerDatoLavadoTurbo, Respuesta<DatoLavadoTurbo>>
    {
        private readonly DatosLavadoTurbo _datosLavadoTurbo;

        public ObtenerDatoLavadoTurboHandler(DatosLavadoTurbo datosLavadoTurbo)
        {
            _datosLavadoTurbo = datosLavadoTurbo;
        }

        public async Task<Respuesta<DatoLavadoTurbo>> Handle(obtenerDatoLavadoTurbo request, CancellationToken cancellationToken)
        {
            Respuesta<DatoLavadoTurbo> respuesta = new Respuesta<DatoLavadoTurbo>();

            try
            {
                respuesta = await _datosLavadoTurbo.ObtenerDatosLavadoTurbo();
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
    }
}
