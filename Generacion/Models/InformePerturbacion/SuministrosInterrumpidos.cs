namespace Generacion.Models.InformePerturbacion
{
    public class SuministrosInterrumpidos
    {
        public string IdSuministrosInterrumpidos { get; set; }
        public string Equipo { get; set; }
        public int Potencia { get; set; }
        public string TiempoInicio { get; set; }
        public string Tiempofinal { get; set; }
        public string TiempoDuracion { get; set; }
        public string IdDetallePerturbacion { get; set; }
    }
}
