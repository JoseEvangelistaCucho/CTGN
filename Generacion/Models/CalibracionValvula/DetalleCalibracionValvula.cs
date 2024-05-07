using Generacion.Application.Funciones;
using System.ComponentModel;

namespace Generacion.Models.CalibracionValvula
{
    public class DetalleCalibracionValvula : ValidarPropiedadesNulasOVaciasBase
    {
        public string IdDetalleCalibracionMotor { get; set; }
        [DisplayName("Ingrese una valor a la U.CaL de Calibracion Valvula")]
        public int valor { get; set; }
        public int NumeroGenerador { get; set; }
        [DisplayName("Ingrese una fecha a la U.CaL de Calibracion Valvula")]
        public string Fecha { get; set; }
    }
}
