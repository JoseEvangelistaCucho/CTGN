using Generacion.Application.DashBoard.ControlGAS.Command;
using Generacion.Models;
using Generacion.Models.ControlGAS;

namespace Generacion.Application.DashBoard.ControlGAS
{
    public interface IRegistroControlGas
    {
        Task<Respuesta<string>> RegistrarContratosControl(ContratoGas contratoGas);
        Task<Respuesta<string>> RegistrarDatosControl(ConsumoGas consumoGas); 
        Task<Respuesta<string>> RegistrarDetalleControl(DetalleConsumoGas detalleGas); 

    }
}
