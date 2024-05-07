using Generacion.Models;
using Generacion.Models.LecturasCampo;


namespace Generacion.Application.LecturaCampo
{
    public interface ILecturaCampo
    {
        Task<Respuesta<string>> GuardarDatosPrincipal(List<DatosFormatoCampo> datosFormatoCampo);
        Task<Respuesta<string>> GuardarDatoCampo(FormatoCampo registo);
    }
}
