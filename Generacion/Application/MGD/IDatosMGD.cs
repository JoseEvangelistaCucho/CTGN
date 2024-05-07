using Generacion.Models;
using Generacion.Models.ION;
using Generacion.Models.MGD;

namespace Generacion.Application.MGD
{
    public interface IDatosMGD
    {
        Task<Respuesta<string>> GuardarDatosConsultaMGD(DatosMGD datos);
    }
}
