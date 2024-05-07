namespace Generacion.Models.ReporteProduccion
{
    public class ArranqueSincronizacion
    {
        public int Anual { get; set; }
        public int Mensual { get; set; }
        public int Diario { get; set; }
        public int NumeroGenerador { get; set; }
        public string Tipo { get; set; }
        public string IdReporteProduccion { get; set; }
        public string? Fecha { get; set; }
    }
}
