namespace Generacion.Models.Bess
{
    public class DBBessLlipata
    {
        public string IdBessLlipata { get; set; }
        public string Fecha { get; set; }
        public decimal TiempoDescarga { get; set; }
        public decimal EnergiaDescarga { get; set; }
        public decimal RatioDescargaHora { get; set; }
        public string HoraDescargaDelSistema { get; set; }
        public decimal ContadorDescargasMes { get; set; }
        public decimal TiempoRecarga { get; set; }
        public decimal EnergiaCargada { get; set; }
        public decimal RatioCargaHora { get; set; }
        public string HoraCargaSistema { get; set; }
        public decimal ContadorRecargasMes { get; set; }
        public string RecortePotenciaMarconaFP { get; set; }
        public string TiempoDescargaFP { get; set; }
        public string HoraCargaFP { get; set; }
        public decimal BessLlipataValue { get; set; }
        public string Observacion { get; set; }
        public string Nota { get; set; }
    }

}
