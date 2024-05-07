using Generacion.Models;
using Generacion.Models.DashBoard;
using MediatR.Pipeline;

namespace Generacion.Application.FiltroCentrifugo.Command.Processor.PostProcessor
{
    public class _01_OrdenarDatosdeFiltro : IRequestPostProcessor<MantenimientoComponentes, Respuesta<Dictionary<string, object>>>
    {
        public Task Process(MantenimientoComponentes request, Respuesta<Dictionary<string, object>> response, CancellationToken cancellationToken)
        {

            if (response.Detalle.ContainsKey("datosFiltro"))
            {
                var datosFiltro = (List<DetalleFiltro>)response.Detalle["datosFiltro"];
                response.Detalle["datosFiltro"] = datosFiltro.OrderBy(x =>
                {
                    DateTime fecha;
                    return DateTime.TryParse(x.Fecha, out fecha) ? fecha : DateTime.MinValue;
                }).ToList();
            }

            return Task.CompletedTask;
        }
    }
}
