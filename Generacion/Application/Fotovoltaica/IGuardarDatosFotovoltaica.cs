using Generacion.Models;
using Generacion.Models.FotoVoltaica;

namespace Generacion.Application.Fotovoltaica
{
    public interface IGuardarDatosFotovoltaica
    {
        Task<Respuesta<string>> GuardarDatosReporte(ReporteFotovoltaica reporte);
        Task<Respuesta<string>> GuardarStatusGenerados(DetalleFotovoltaica detalle);
        Task<Respuesta<string>> GuardarDetalleReporte(FotovoltaicaGenerada fotovoltaica);
    }
}
