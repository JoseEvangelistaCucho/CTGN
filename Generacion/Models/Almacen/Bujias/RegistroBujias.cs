namespace Generacion.Models.Almacen.Bujias
{
    public class RegistroBujias
    {
        public string IdDetalleControlCambio { get; set; }
        public int Detalle { get; set; }
        public string Fecha { get; set; }
        public string Lado { get; set; }
        public string IdSubtituloCampo { get; set; }
        public int Fila { get; set; }
        public int Item { get; set; }
        public string IdControlCambio { get; set; }
        public string Sitio { get; set; }
        public string Nota { get; set; }
        public int NumeroGenerador { get; set; }
    }
    public class DetalleRegistroBujias
    {
        public int Horasoperacion { get; set; }
        public int Numerogenerador { get; set; }
        public int Bujiasgastadas { get; set; }
    }
}
