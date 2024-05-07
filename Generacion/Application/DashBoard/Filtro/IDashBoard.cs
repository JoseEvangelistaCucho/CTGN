using Generacion.Models;
using Generacion.Models.DashBoard;

namespace Generacion.Application.DashBoard.Filtro
{
    public interface IDashBoard
    {
        Task<Respuesta<string>> GuardarDatosFiltro(List<DashboardDetalleFiltro> detalleFiltro, string idSitio);
    }
}
