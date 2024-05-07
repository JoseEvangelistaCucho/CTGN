using Generacion.Application.Funciones;
using System.ComponentModel;

namespace Generacion.Models.TurboCompresor
{
    public class DatoLavadoTurbo
    {
        public string IdLavadoTurbo { get; set; }
        public string Fecha { get; set; }
    }

    public class DetalleLavadoTurbo : ValidarPropiedadesNulasOVaciasBase
    {
        public string IdDetalleLavadoTurbo { get; set; }
        public string Tipo { get; set; }
        [DisplayName("Ingrese un valor en Lavado Turbo.")]
        public int Valor { get; set; }
        public int NumeroGenerador { get; set; }
        [DisplayName("Ingrese una fecha de inpeccion en Lavado Turbo.")]
        public string Fecha { get; set; }
        public string IdLavadoTurbo { get; set; }
    }

}
