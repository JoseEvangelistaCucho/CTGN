using Generacion.Models;
using Generacion.Models.ION;
using Generacion.Models.MGD;

namespace Generacion.Application.ION
{
    public interface IDatosION
    {
        Task<Respuesta<DatosFormatoMGD>> GuardarDatosION(DatosMGD datos);
    }
}
