namespace Generacion.Models.Mantenimiento
{
    public class ReporteDiarioMantenimiento
    {
        public string IdReporteDiario { get; set; }
        public string Fecha { get; set; }
        public int HorasMotor01 { get; set; }
        public int HorasMotor02 { get; set; }
        public string IdOperario { get; set; }
    }
}
