using Generacion.Models;
using Generacion.Models.Bess;

namespace Generacion.Application.BessLlipata.ReporteProduccion
{
    public interface IReporteBessLlipata
    {
        Task<Respuesta<string>> GuardarDatosReporte(DBBessLlipata datos);
    }
}
