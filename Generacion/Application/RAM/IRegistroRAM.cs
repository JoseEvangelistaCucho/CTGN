using Generacion.Models;
using Generacion.Models.ReporteRAM;

namespace Generacion.Application.RAM
{
    public interface IRegistroRAM
    {
        Task<Respuesta<string>> GuardarDatosRAM(DBRAM dbRAM);
        Task<Respuesta<string>> GuardarDatosRAMOil(ViewOIL dbRAM);
    }
}
