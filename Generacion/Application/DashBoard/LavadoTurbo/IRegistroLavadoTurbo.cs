using Generacion.Models;
using Generacion.Models.TurboCompresor;

namespace Generacion.Application.DashBoard.LavadoTurbo
{
    public interface IRegistroLavadoTurbo
    {

        Task<Respuesta<string>> GuardarDatosTurboLavado(DatoLavadoTurbo request);
        Task<Respuesta<string>> GuardarDetalleTurboLavado(DetalleLavadoTurbo request);

    }
}
