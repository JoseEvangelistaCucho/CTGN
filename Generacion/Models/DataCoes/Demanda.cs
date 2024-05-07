namespace Generacion.Models.DataCoes
{
    public class Demanda
    {
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public decimal Ejecutado { get; set; }
        public decimal ProgramacionDiaria { get; set; }
        public decimal ProgramacionSemanal { get; set; }
    }
}
