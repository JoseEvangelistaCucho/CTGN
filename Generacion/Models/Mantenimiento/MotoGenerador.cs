namespace Generacion.Models.Mantenimiento
{
    public class MotoGenerador
    {
        public string? IdDescripDiario { get; set; }
        public string? IdReporteDiario { get; set; }
        public int NumeroGenerador { get; set; }
        public string? Fecha { get; set; }
        public string? Hora { get; set; }
        public string? Descripcion { get; set; }
        public detalleContenido? detalleContenido { get; set; }

        
    }
    public class detalleContenido
    {
        public string? Casilladiv { get; set; }
        public string? Casillaid { get; set; }
        public string? Posicion { get; set; }
    }
}
