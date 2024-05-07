using Generacion.Models;
using Generacion.Models.Bess;

namespace Generacion.Application.ReportesGenerales
{
    public interface IRegistroDataBess
    {
        Task<Respuesta<string>> GuardarDatosArchivosText(DBDataBess bess);
    }
}
