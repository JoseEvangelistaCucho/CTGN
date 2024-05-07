namespace Generacion.Models.Mantenimiento
{
    public class ImagenModel
    {
        public string base64 { get; set; }
        public string casillaid { get; set; }
    }

    public class ImagenReporte
    {
        public string base64 { get; set; }
        public string TipoReporte { get; set; }
        public string NombreReporte { get; set; }
    }

}
