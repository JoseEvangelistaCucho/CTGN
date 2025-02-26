using Generacion.Application.Common;
using Generacion.Models.Session;
using System.DirectoryServices.AccountManagement;

namespace WebApi.Application.ValidatePassword.Queries
{
    public class ActiveDirectoryProvider : IActiveDirectoryProvider
    {
        private PrincipalContext _principalContext;
        public ActiveDirectoryProvider(PrincipalContext principalContext)
        {
            _principalContext = principalContext;
        }
        public bool ValidateUserCredentials(UsuarioSession usuario)
        {
            return _principalContext.ValidateCredentials(usuario.UsuarioRed.ToLower(), usuario.Clave);
        }
    }
}
