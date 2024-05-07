namespace Generacion.Models.Mantenimiento
{
    public class MantenimientoServicios
    {
        public string  IdServiciosAux { get; set; }
        public string Descripcion { get; set; }
        public string Fecha { get; set; }
        public string IdReporteDiario { get; set; }
        public detalleContenido? detalleContenido { get; set; }

    }
}
