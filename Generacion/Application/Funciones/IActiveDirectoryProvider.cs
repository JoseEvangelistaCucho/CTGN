using Generacion.Models.Session;
using System.DirectoryServices.AccountManagement;

namespace WebApi.Application.Common.Interfaces
{
    public interface IActiveDirectoryProvider
    {
        bool ValidateUserCredentials(UsuarioSession usuario);
    }
}
