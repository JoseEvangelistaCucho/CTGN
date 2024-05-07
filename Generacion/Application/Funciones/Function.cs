using Generacion.Application.DataBase.cache;
using Generacion.Models.Aceite;
using Generacion.Models.Usuario;
using Newtonsoft.Json;
using NuGet.Packaging;
using System.ComponentModel;
using Generacion.Models.DatosConsola;

namespace Generacion.Application.Funciones
{
    public class Function
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CacheDatos _cache;
        public Function(CacheDatos cache, IHttpContextAccessor httpContext)
        {
            _cache = cache;
            _httpContextAccessor = httpContext;
        }


        public async Task AgregarDatosKeyInt(Dictionary<int, object> destino, Dictionary<int, object> datos)
        {
            await Task.Run(() =>
            {
                destino.AddRange(datos);
            });

        }
        public async Task AgregarDatosKeyString(Dictionary<string, object> destino, Dictionary<string, object> datos)
        {
            await Task.Run(() =>
            {
                destino.AddRange(datos);
            });

        }

        public async Task<Dictionary<string, CabecerasTabla>> ObtenerDatosCabecera()
        {
            Dictionary<string, CabecerasTabla> datoscabecera = new Dictionary<string, CabecerasTabla>();
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    string cabecera = _httpContextAccessor.HttpContext.Session.GetString("datoscabecera");
                   datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(cabecera);
                }
            }
            catch (Exception ex)
            {

            }

            return datoscabecera;
        }
        public async Task<DetalleOperario> ObtenerDatosOperario()
        {
            DetalleOperario user = new DetalleOperario();
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    string usuarioDetail = _httpContextAccessor.HttpContext.Session.GetString("usuarioDetail");
                    user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);
                }
                else
                {
                    var datosUsuarioRecuperados = _httpContextAccessor.HttpContext.Request.Cookies["usuario"];
                    var usuario = JsonConvert.DeserializeObject(datosUsuarioRecuperados);
                }
            }
            catch (Exception ex)
            {

            }

            return user;
        }

        public async Task<bool> ValidarModalAdvertencia()
        {
            bool respuesta = false;

            var datosOperario = await ObtenerDatosOperario();

            if (_httpContextAccessor.HttpContext.Request.Cookies["ModalAdvertenciaMostrado"] == null && datosOperario.IdSitio.Equals("LUREN"))
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddHours(3), 
                    IsEssential = true
                };

                _httpContextAccessor.HttpContext.Response.Cookies.Append("ModalAdvertenciaMostrado", "true", cookieOptions);
                respuesta = true;
            }
            return respuesta;
        }

        public async Task<Dictionary<int, Dictionary<string, ControlCambioAceite>>> ConvertirDiccionarioControlPorGEyTipo(List<ControlCambioAceite> datos)
        {
            Dictionary<int, Dictionary<string, ControlCambioAceite>> datosPorGEyTipo = new Dictionary<int, Dictionary<string, ControlCambioAceite>>();

            await Task.Run(() =>
            {
                foreach (var control in datos)
                {
                    if (!datosPorGEyTipo.ContainsKey(control.NumeroGenerador))
                    {
                        datosPorGEyTipo[control.NumeroGenerador] = new Dictionary<string, ControlCambioAceite>();
                    }
                    datosPorGEyTipo[control.NumeroGenerador][control.Tipo] = control;
                }
            }
            );
            return datosPorGEyTipo;
        }

    }

    public class ValidarPropiedadesNulasOVaciasBase
    {
        public string ValidarPropiedadesNulasOVacias()
        {
            var mensajesError = string.Empty;
            var properties = GetType().GetProperties();

            foreach (var property in properties)
            {
                var valor = property.GetValue(this);

                if (valor == null || string.IsNullOrWhiteSpace(valor.ToString()))
                {
                    var displayName = property.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                                               .Cast<DisplayNameAttribute>()
                                               .FirstOrDefault()?.DisplayName;

                    mensajesError = displayName;
                }
            }

            return mensajesError;
        }
    }
}
