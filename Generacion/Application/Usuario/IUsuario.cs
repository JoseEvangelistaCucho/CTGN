using Generacion.Models;
using Generacion.Models.Usuario;

namespace Generacion.Application.Usuario
{
    public interface IUsuario
    {
        Task<Respuesta<List<HistorialUsuario>>> GuardarHistorial(List<HistorialUsuario> historialUsuario , string idOperario);
        Task<Respuesta<DetalleOperario>> ObtenerDatosOperario(string idOperario);
    }
}
