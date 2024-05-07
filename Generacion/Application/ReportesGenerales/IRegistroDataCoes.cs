using Generacion.Models;
using Generacion.Models.DataCoes;

namespace Generacion.Application.ReportesGenerales
{
    public interface IRegistroDataCoes
    {
        Task<Respuesta<string>> GuardarDatosExcelReporte(Demanda demanda);
    }
}
