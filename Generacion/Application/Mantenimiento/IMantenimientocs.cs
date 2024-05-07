using Generacion.Models;
using Generacion.Models.Mantenimiento;

namespace Generacion.Application.Mantenimiento
{
    public interface IMantenimiento
    {
        Task<string> GuardarDatosMotoGenerador(List<MotoGenerador> listaMotoGeneradores);
        Task<string> GuardarDatosServ(List<MantenimientoServicios> detalleServicio);
        Task<Respuesta<string>> GuardarDatosAceiteCarter(CilindroAceiteCarter datos);
        Task<string> GuardarExpansionVessel(List<ExpansionVersel> listaMotoGeneradores);
        Task<Respuesta<string>> GuardarReporteDiario(ReporteDiarioMantenimiento datosReporte);


    }
}
