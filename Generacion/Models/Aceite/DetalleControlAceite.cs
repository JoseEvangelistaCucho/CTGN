using Generacion.Application.Funciones;
using System.ComponentModel;

namespace Generacion.Models.Aceite
{
    public class DetalleControlAceite : ValidarPropiedadesNulasOVaciasBase
    {
        public string? IdDetalleControlCambio { get; set; }
        public string? IdControlCambioAceite { get; set; }
        [DisplayName("Ingrese un valor en Horas de Operacion de cambio de aceite.")]
        public int HorasOperacion { get; set; }
        public int NumeroGenerador { get; set; }
        public string? Fecha { get; set; }
    }
}
