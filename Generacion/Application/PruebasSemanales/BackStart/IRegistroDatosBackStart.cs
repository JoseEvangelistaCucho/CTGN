using Generacion.Models.PruebasSemanales.BlackStart;
using Generacion.Models;

namespace Generacion.Application.PruebasSemanales.BackStart
{
    public interface IRegistroDatosPruebaSemanal
    {
        Task<Respuesta<string>> GuardarDetallePruebaSemanal(DetallePruebaSemanal datos);
        Task<Respuesta<string>> GuardarDatosPruebaSemanal(PruebaSemanal datos, string tipo);
    }
}
