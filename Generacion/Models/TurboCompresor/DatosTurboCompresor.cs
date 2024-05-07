using Generacion.Application.Funciones;
using MediatR;
using System.ComponentModel;

namespace Generacion.Models.TurboCompresor
{
    public class DatosTurboCompresor : ValidarPropiedadesNulasOVaciasBase
    {
        public string IdTurboCompresor { get; set; }

        [DisplayName("Ingrese una fecha de inpeccion en el Turbo Compresor.")]
        public string Fecha { get; set; }
    }

    public class DetalleTurboCompresor : ValidarPropiedadesNulasOVaciasBase
    {
        public string IdDetalleTurboCompresor { get; set; }
        public int Posicion { get; set; }
        public string Lado { get; set; }
        public string Tipo { get; set; }
        public int NumeroGenerador { get; set; }
        public string Fecha { get; set; }
        [DisplayName("Ingrese un valor en el <<>> Turbo Compresor.")]
        public decimal Valor { get; set; }
        public string IdTurboCompresor { get; set; }
    }
}
