using Generacion.Application.Funciones;
using System.ComponentModel;

namespace Generacion.Models.Mantenimiento
{
    public class CilindroAceiteCarter : ValidarPropiedadesNulasOVaciasBase
    {
        public string IdCarter { get; set; }
        public int NumeroGenerador { get; set; }
        public decimal NivelCarterNuevo { get; set; }
        [DisplayName("Ingrese una valor a nivel tk vessel nuevo.")]
        public decimal NivelTKNuevo { get; set; }
        public decimal ContometroNuevo { get; set; }
        public string IdReporteDiario { get; set; }
        public string Fecha { get; set; }
    }
}
