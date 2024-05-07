using System.Numerics;

namespace Generacion.Models.ReporteProduccion
{
    public class ReporteProduccionStatus
    {
        public string IdReporteProdStatus { get; set; }
        public int NumeroGenerador { get; set; }
        public decimal ServiceAccumulated { get; set; }
        public decimal PlannedMainten { get; set; }
        public decimal ForcedMaintMech { get; set; }
        public decimal ForcedMaintElec { get; set; }
        public decimal ForcedAuxiliaries { get; set; }
        public decimal ExternalTrips { get; set; }
        public decimal StandBy { get; set; }
        public decimal RunningHours { get; set; }
        public decimal HoursAvailable { get; set; }
        public string Fecha { get; set; }
        public string IdReporteProduccion { get; set; }
    }
}
