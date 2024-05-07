namespace Generacion.Models.InformePerturbacion
{
    public class SecuenciaCronologica
    {
        public string IdSecuenciaCronologica { get; set; }
        public int Posicion { get; set; }
        public string Hora { get; set; }
        public string Componente { get; set; }
        public string DescripcionEvento { get; set; }
        public string IdDetallePerturbacion { get; set; }
    }
}
