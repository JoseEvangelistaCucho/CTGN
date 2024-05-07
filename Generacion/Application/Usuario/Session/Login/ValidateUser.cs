using Generacion.Application.DataBase;
using Generacion.Models;
using Generacion.Models.Session;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using WebApi.Application.Common.Interfaces;
using Generacion.Models.Usuario;
using Generacion.Application.Usuario.Session;
using Generacion.Application.Usuario;
using Newtonsoft.Json;
using Generacion.Application.DataBase.cache;

namespace Generacion.Application.ValidationSession.Login
{
    public class ValidateUser : IValidateUser
    {
        private readonly IConexionBD _conexion;
        private readonly IActiveDirectoryProvider _activeDirectoryProvider;
        private readonly IUsuario _usuario; 
        private readonly CacheDatos _cache; 
        public ValidateUser(CacheDatos cache, IConexionBD conexion, IActiveDirectoryProvider activeDirectoryProvider, IUsuario usuario)
        {
            _cache = cache;
            _activeDirectoryProvider = activeDirectoryProvider;
            _usuario = usuario;
            _conexion = conexion;
        }
        public async Task<Respuesta<DetalleOperario>> ValidateSessionUser(UsuarioSession usuario)
        {
            Respuesta<DetalleOperario> respuesta = new Respuesta<DetalleOperario>();

            try
            {
                var respuestaClaveRed = true;//_activeDirectoryProvider.ValidateUserCredentials(usuario);
                if (respuestaClaveRed)
                {
                    respuesta = await _usuario.ObtenerDatosOperario(usuario.UsuarioRed);

                    if (respuesta.IdRespuesta == 0 && respuesta.Detalle != null)
                    {
                        respuesta.Mensaje = "Bienvenido.";
                        _cache.GuardarDatosCache("DatoCacheOperario", JsonConvert.SerializeObject(respuesta.Detalle)) ;
                    }
                    else
                    {
                        respuesta.Mensaje = respuesta.Mensaje;
                        respuesta.IdRespuesta = 99;
                    }
                }
                else
                {
                    respuesta.IdRespuesta = 99;
                    respuesta.Mensaje = "Error en credenciales.";
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Ocurrio un Error, intentelo mas tarde.";
            }
            return respuesta;
        }
    }

}
