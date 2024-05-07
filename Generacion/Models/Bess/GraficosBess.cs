namespace Generacion.Models.Bess
{
    public class PortenciaMarconaVSBess
    {
        public decimal[] LLIXI625_PV { get; set; }
        public decimal[] LLIXI633_PV { get; set; }
        public string[] Hora { get; set; }
    }
    public class DemandaCoesVSBess
    {
        public decimal[] Ejecutado { get; set; }
        public decimal[] UsoBaterias { get; set; }
        public string[] Hora { get; set; }
    }
    public class DemandavsCoincidente
    {
        public decimal[] ValorHPCoincidente { get; set; }
        public decimal[] ValorCoincidente { get; set; }
        public string[] Fecha { get; set; }
    }
}
