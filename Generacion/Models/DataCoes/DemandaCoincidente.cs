namespace Generacion.Models.DataCoes
{
    public class DemandaCoincidente
    {
        public string Hora { get; set; }
        public string InicioMes { get; set; }
        public decimal PotenciaMW { get; set; }
        public string FechaCoincidente { get; set; }
        public decimal promedioDescarga { get; set; }
    }
}
