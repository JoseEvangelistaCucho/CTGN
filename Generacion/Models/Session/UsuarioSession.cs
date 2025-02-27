using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Generacion.Models.Session
{
    public class UsuarioSession
    {
        [DisplayName("Ingrese un usuario.")]
        public string UsuarioRed
        {
            get; set;
        }
        
        [DisplayName("Ingrese una clave.")]
        public string Clave
        {
            get; set;
        }
        
        [DisplayName("Seleccione un sitio.")]
        public string Sitio
        {
            get; set;
        }

        [JsonIgnore]
        [DisplayName("Seleccione la red.")]
        public string Company { get; set; }


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





