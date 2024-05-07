namespace Generacion.Models.DatosConsola
{
    public class OutGoingFeeder
    { 
        public string IdOutGoing { get; set; }
        public string IdTipoEngine { get; set; }
        public decimal KWh { get; set; }
        public decimal KVARh { get; set; }
        public string Hora { get; set; }
        public string Fecha { get; set; }
        public string IdFormatoConsola { get; set; }
        public int Fila { get; set; }
    }
}
