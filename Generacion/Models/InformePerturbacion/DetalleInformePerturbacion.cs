namespace Generacion.Models.InformePerturbacion
{
    public class DetalleInformePerturbacion
    {
        public string IdDetallePerturbacion { get; set; }
        public string DescripcionEvento { get; set; }
        public string CondicionesPrevias { get; set; }
        public string? RutaImagenes { get; set; }
        public string Anexos { get; set; }
        public string IdReportePerturbacion { get; set; }
    }
}
