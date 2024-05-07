using Generacion.Application.Funciones;
using System.ComponentModel;

namespace Generacion.Models.DashBoard
{
    public class DetalleFiltro : DashboardDetalleFiltro
    {
        public decimal HorasTrabajadasFiltro { get; set; }
        public decimal HorasOperacionMantto { get; set; }
        public decimal HorasOpUltimoMantto { get; set; }
        public string Observaciones { get; set; }
    }

    public class DashboardDetalleFiltro : ValidarPropiedadesNulasOVaciasBase
    {
        public string IdDetalleFiltro { get; set; }
        [DisplayName("Ingrese las horas de operacion.")]
        public decimal ProximaHoraCambio { get; set; }

        [DisplayName("Ingrese un número de generador del 0 al 2.")]
        public decimal NumeroGenerador { get; set; }
        [DisplayName("Ingrese una fecha.")]
        public string Fecha { get; set; }
        [DisplayName("Ingrese las horas de operacion.")]
        public decimal HorasOperacion { get; set; }
        [DisplayName("Ingrese el espesor.")]
        public decimal Espesor { get; set; }
        public string IdReporteFiltro { get; set; }
        [DisplayName("Seleccione un ejecutor.")]
        public string OperadorEjecutor { get; set; }
    }
}
