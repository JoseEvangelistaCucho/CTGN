using Generacion.Models.InformePerturbacion;
using Generacion.Models;

namespace Generacion.Application.InformePerturbacion
{
    public interface IRegistroDatosPerturbacion
    {
        Task<Respuesta<string>> GuardarDatosPrincipal(InformeGeneralPerturbacion datos);
        Task<Respuesta<string>> GuardarDetallesPrincipal(DetalleInformePerturbacion datos, string rutaImagenes);
        Task<Respuesta<string>> GuardarSecuenciaPrincipal(SecuenciaCronologica datos);
        Task<Respuesta<string>> GuardarSuministrosinterrumpidos(SuministrosInterrumpidos datos);

    }
}
