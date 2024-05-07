using Generacion.Models;
using Generacion.Models.Aceite;

namespace Generacion.Application.DashBoard.CambioAceite
{
    public interface IRegistroControlAceite
    {
        Task<Respuesta<string>> GuardarDatosControlAceite(ControlCambioAceite request);
        Task<Respuesta<string>> GuardarDetalleControlAceite(DetalleControlAceite request);
    }
}
