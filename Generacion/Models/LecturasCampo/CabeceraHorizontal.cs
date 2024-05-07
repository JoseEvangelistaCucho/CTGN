using Generacion.Models.DatosConsola;
using Generacion.Models.Usuario;
using System.Security.Principal;

namespace Generacion.Models.LecturasCampo
{

    public class DatosFormatoCampo
    {
        public string IdDetalleCampo { get; set; }
        public decimal Detalle { get; set; }
        public string IdSubtituloCampo { get; set; }
        public string IdReporteCampo { get; set; }
        public string Hora { get; set; }
        public string Sitio { get; set; }
        public int Fila { get; set; }
        public int NumeroGenerador { get; set; }
        public string Fecha { get; set; }
    }
}
