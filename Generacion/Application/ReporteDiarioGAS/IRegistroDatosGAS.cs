using Generacion.Models;
using Generacion.Models.ReporteDiarioGAS;
using MReporteGAS = Generacion.Models.ReporteDiarioGAS.ReporteGAS;

namespace Generacion.Application.ReporteDiarioGAS
{
    public interface IRegistroDatosGAS
    {
        Task<Respuesta<string>> GuardarDetalle(List<DetalleReporteGas> datos);
        void guardarIdReporte(MReporteGAS reporteGAS);

    }
}
