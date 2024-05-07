
using Generacion.Models;
using Generacion.Models.DatosConsola;

namespace Generacion.Application.DatosConsola
{
    public interface IDatosRegistroConsola
    {
        Task<Respuesta<string>> GuardarDatosEG(List<DatosFormatoConsola> datosConsola);
        Task<Respuesta<string>> GuardarDatosGenerador(List<RegistroDatosGenerator> datosConsola);
        Task<Respuesta<string>> GuardarDatosEngine(List<RegistroDetalleEngine> datosConsola);
        Task<Respuesta<string>> GuardarLectMedianoche(LecturasMedianoche lecturasMedianoche);
        Task<Respuesta<string>> GuardarDetallesBAO(List<OutGoingFeeder> datos);
        Task<Respuesta<string>> GuardarDatoFormato(FormatoConsola datos);

    }
}
