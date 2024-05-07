using Generacion.Models;
using Generacion.Models.Almacen.Bujias;

namespace Generacion.Application.Bujias
{
	public interface IBujias
	{
		Task<Respuesta<string>> GuardarOActualizarRegisto(List<RegistroBujias> registro);
		Task<Respuesta<string>> GuardarOActualizarControlCambio(ControlCambioData controlCambioData);

    }
}
