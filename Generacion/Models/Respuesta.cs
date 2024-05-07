namespace Generacion.Models
{
    public class Respuesta<T>
    {
        public int IdRespuesta { get; set; }
        public string Mensaje { get; set; }
        public T Detalle { get; set; } 
    }
}
