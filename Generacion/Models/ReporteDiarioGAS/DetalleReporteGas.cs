namespace Generacion.Models.ReporteDiarioGAS
{
    public class DetalleReporteGas
    {
        public string IdDetalleReporte { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public decimal VC { get; set; }
        public decimal VN { get; set; }
        public decimal PL { get; set; }
        public decimal FL { get; set; }
        public decimal SV { get; set; }
        public decimal TL { get; set; }
        public decimal FC { get; set; }
        public decimal FS { get; set; }
        public decimal DA { get; set; }
        public decimal DP { get; set; }
        public decimal PR { get; set; }
        public decimal D1 { get; set; }
        public decimal D2 { get; set; }
        //public decimal L { get; set; }
        public decimal Caudalimetro { get; set; }
        public decimal PresionIngreso { get; set; }
        public string IdReporteGas { get; set; }
    }
}
