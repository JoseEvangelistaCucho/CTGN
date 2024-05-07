using Generacion.Models.ION;

namespace Generacion.Models.MGD
{
    public class DatosMGD 
    {
        public string IdReporteMGD { get; set; }
        public string Idsitio { get; set; }
        public string IdOperario { get; set; }
        public string Fecha { get; set; }
        public List<DatosFormatoMGD> Revenue { get; set; }
    }
}
