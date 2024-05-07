namespace Generacion.Models.ReporteProduccion
{
    public class EnergiaProducida
    {
        public string IdEnergyProduce { get; set; }
        public string TipoEnergia { get; set; }
        public string Fecha { get; set; }
        public decimal PmuEng_01 { get; set; }
        public decimal PmuEng_02 { get; set; }
        public decimal GrosEnergy { get; set; }
    }
}
