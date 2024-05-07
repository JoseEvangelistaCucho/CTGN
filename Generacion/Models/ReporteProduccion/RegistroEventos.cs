namespace Generacion.Models.ReporteProduccion
{
    public class RegistroEventos
    {
        public string IdRegEventos { get; set; }
        public int NumeroGenerador { get; set; }
        public string FechaParada { get; set; }
        public string FechaArranque { get; set; }
        public string ExternalTrips { get; set; }
        public string ForcedMaint { get; set; }
        public string PlannedMaint { get; set; }
        public string StandBy { get; set; }
        public string Sistema { get; set; }
        public string UnidadFuncional { get; set; }
        public string DescripcionEvento { get; set; }
        public string IdReporte { get; set; }
        public string nombreReporte { get; set; }
    }
}
