using Generacion.Models;
using Generacion.Models.TurboCompresor;

namespace Generacion.Application.DashBoard.TurboCompresor
{
    public interface IRegistroTurboCompresor
    {
        public Task<Respuesta<string>> GuardarDatosTurboCompresor(DatosTurboCompresor datos);
        public Task<Respuesta<string>> GuardarDetalleTurboCompresor(DetalleTurboCompresor detalle);
    }
}
