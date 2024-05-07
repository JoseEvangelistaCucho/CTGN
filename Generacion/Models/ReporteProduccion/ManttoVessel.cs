namespace Generacion.Models.ReporteProduccion
{
    public class ManttoVessel
    {

        public string IdTkManttoVessel { get; set; }
        public string Fecha { get; set; }
        public string TipoTk { get; set; }
        public decimal Eg1_Yesterday { get; set; }
        public decimal Eg1_Today { get; set; }
        public decimal Eg2_Yesterday { get; set; }
        public decimal Eg2_Today { get; set; }
    }
}
