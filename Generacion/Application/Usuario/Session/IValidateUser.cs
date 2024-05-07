using Generacion.Models.Session;
using Generacion.Models.Usuario;
using Generacion.Models;

namespace Generacion.Application.Usuario.Session
{
    public interface IValidateUser
    {
        Task<Respuesta<DetalleOperario>> ValidateSessionUser(UsuarioSession usuario);
    }
}
