using MediatR;

namespace Generacion.Models.ReporteRAM
{
    public class DBRAM 
    {
        public string IdReporteRam { get; set; }
        public decimal ConsumoServiciosAuxiliares { get; set; }
        public decimal LHV_kJkgGE1 { get; set; }
        public decimal LHV_kJkgGE2 { get; set; }
        public string HorasDerateoEquivalenteGE1 { get; set; }
        public string HorasDerateoEquivalenteGE2 { get; set; }
        public decimal CapacidadMaximaNetaGE1 { get; set; }
        public decimal CapacidadMaximaNetaGE2 { get; set; }
        public string Fecha { get; set; }
    }
}
