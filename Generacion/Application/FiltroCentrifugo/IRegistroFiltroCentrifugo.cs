using Generacion.Models;
using Generacion.Models.DashBoard;
using Generacion.Models.FiltroCentrifugo;

namespace Generacion.Application.FiltroCentrifugo
{
    public interface IRegistroFiltroCentrifugo
    {
        Task<Respuesta<string>> GuardarDatosFiltro(List<DetalleFiltro> datosFiltro);
        Task<Respuesta<string>> GuardarDetalleFiltro(List<EspecificacionFiltro> detalleFiltro);
        Task<Respuesta<string>> GuardarReporteFiltro(ReporteFiltro datos);

    }
}
