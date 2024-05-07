using Generacion.Models;
using Generacion.Models.TurboCompresor;
using MediatR;

namespace Generacion.Application.DashBoard.LavadoTurbo.Query
{
    public class obtenerDetalleLavadoTurbo : IRequest<Respuesta<Dictionary<int, Dictionary<string, DetalleLavadoTurbo>>>>
    {
        public string idLavado { get; set; }
    }

    public class obtenerDetalleLavadoTurboHandler : IRequestHandler<obtenerDetalleLavadoTurbo, Respuesta<Dictionary<int, Dictionary<string, DetalleLavadoTurbo>>>>
    {
        private readonly DatosLavadoTurbo _datosLavadoTurbo;

        public obtenerDetalleLavadoTurboHandler(DatosLavadoTurbo datosLavadoTurbo)
        {
            _datosLavadoTurbo = datosLavadoTurbo;
        }

        public async Task<Respuesta<Dictionary<int, Dictionary<string, DetalleLavadoTurbo>>>> Handle(obtenerDetalleLavadoTurbo request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<int, Dictionary<string, DetalleLavadoTurbo>>> respuesta = new Respuesta<Dictionary<int, Dictionary<string, DetalleLavadoTurbo>>>();

            try
            {
                var respuestaConsulta = await _datosLavadoTurbo.ObtenerDetalleLavadoTurbo(request.idLavado);


                var respuestaDetalle = respuestaConsulta.Detalle
              .GroupBy(c => c.NumeroGenerador)
              .ToDictionary(
                  groupGE => groupGE.Key,
                  groupGE => groupGE
                      .ToDictionary(
                          groupPosition => groupPosition.Tipo
                          )
               );

                respuesta.Detalle = new Dictionary<int, Dictionary<string, DetalleLavadoTurbo>>();
                respuesta.Detalle = respuestaDetalle;
            }
            catch (Exception ex)
            {

            }

            return respuesta;


        }
    }
}
