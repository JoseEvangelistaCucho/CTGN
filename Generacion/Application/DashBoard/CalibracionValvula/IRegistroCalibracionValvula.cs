using Generacion.Models;
using Generacion.Models.CalibracionValvula;

namespace Generacion.Application.DashBoard.CalibracionValvula
{
    public interface IRegistroCalibracionValvula
    {
        Task<Respuesta<string>> GuardarDatosCalibracionValvula(DetalleCalibracionValvula datos);
    }
}
