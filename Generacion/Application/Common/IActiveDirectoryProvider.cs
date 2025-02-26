using Generacion.Models.Session;

namespace Generacion.Application.Common
{
    public interface IActiveDirectoryProvider
    {
        bool ValidateUserCredentials(UsuarioSession usuario);
    }
}