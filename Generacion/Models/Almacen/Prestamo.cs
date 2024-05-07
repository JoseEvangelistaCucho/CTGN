namespace Generacion.Models.Almacen
{
    public class Prestamo
    {
        public string ComponentePrestadoID { get; set; }
        public string ComponenteID { get; set; }
        public string NombreComponente { get; set; }
        public string PrestamoDesde { get; set; }
        public string PrestamoHacia { get; set; }
        public int CantidadPrestamo { get; set; }
    }
    public class PrestamoComponente : Prestamo
    {
        public DateTime FechaPrestamos { get; set; }
    }
}
