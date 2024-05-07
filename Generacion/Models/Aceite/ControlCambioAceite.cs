using Generacion.Application.Funciones;
using MediatR;
using System.ComponentModel;

namespace Generacion.Models.Aceite
{
    public class ControlCambioAceite : ValidarPropiedadesNulasOVaciasBase
    {
        public string IdControlCambioAceite { get; set; }
        [DisplayName("Ingrese un valor en Horas de cambio de aceite.")] 
        public int HorasCambio { get; set; }
        public int NumeroGenerador { get; set; }
        public string Tipo { get; set; }
        [DisplayName("Ingrese un valor en fecha de cambio de aceite.")]
        public string FechaCambio { get; set; }
        public int Activo { get; set; }
    }

}
