namespace Generacion.Models.FiltroCentrifugo
{
    public class EspecificacionFiltro
    {
        public string IdFiltro { get; set; }
        public string Fecha { get; set; }
        public string Tipo { get; set; }
        public string Serie { get; set; }
        public string Especificacion { get; set; }
        public string TipoMantenimiento { get; set; }
        public decimal NumeroGenerador { get; set; }
        public string IdReporteFiltro { get; set; }
    }
}
