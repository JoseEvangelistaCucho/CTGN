using Generacion.Models;
using Generacion.Models.Aceite;
using Generacion.Models.TurboCompresor;
using MediatR;
using System.Text.RegularExpressions;

namespace Generacion.Application.DashBoard.TurboCompresor.Query
{
    public class ObtenerDatosTurboCompresorPorID : IRequest<Respuesta<Dictionary<int, Dictionary<string, Dictionary<int, Dictionary<string, DetalleTurboCompresor>>>>>>
    {
        public string idTurboCompresor { get; set; }
    }

    public class ObtenerDatosTurboCompresorPorIDHandler : IRequestHandler<ObtenerDatosTurboCompresorPorID, Respuesta<Dictionary<int, Dictionary<string, Dictionary<int, Dictionary<string, DetalleTurboCompresor>>>>>>
    {
        private readonly ConsultaDatosTurboCompresor _consultaDatosTurbo;
        public ObtenerDatosTurboCompresorPorIDHandler(ConsultaDatosTurboCompresor consultaDatosTurbo)
        {
            _consultaDatosTurbo = consultaDatosTurbo;
        }
        public async Task<Respuesta<Dictionary<int, Dictionary<string, Dictionary<int, Dictionary<string, DetalleTurboCompresor>>>>>> Handle(ObtenerDatosTurboCompresorPorID request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<int, Dictionary<string, Dictionary<int, Dictionary<string, DetalleTurboCompresor>>>>> respuesta = new Respuesta<Dictionary<int, Dictionary<string, Dictionary<int, Dictionary<string, DetalleTurboCompresor>>>>>();
            var respuestaConsulta = await _consultaDatosTurbo.ObtenerDetalleTurboCompresor(request.idTurboCompresor);

            respuesta.Detalle = new Dictionary<int, Dictionary<string, Dictionary<int, Dictionary<string, DetalleTurboCompresor>>>>();

            var respuestaDetalle = respuestaConsulta.Detalle
               .GroupBy(c => c.NumeroGenerador)
               .ToDictionary(
                   groupGE => groupGE.Key,
                   groupGE => groupGE
                       .GroupBy(c => c.Tipo)
                       .ToDictionary(
                           groupPosition => groupPosition.Key,
                           groupPosition => groupPosition
                               .GroupBy(c => c.Posicion)
                               .ToDictionary(
                               groupTipo => groupTipo.Key,
                               groupTipo => groupTipo
                                      .ToDictionary(
                                      groupTipo => groupTipo.Lado,
                                      groupTipo => groupTipo
                                      )
                               )
                           )
                );
            
            respuesta.Detalle = respuestaDetalle;

            return respuesta;
        }
    }
}
