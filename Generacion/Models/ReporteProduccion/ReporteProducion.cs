using Newtonsoft.Json;

namespace Generacion.Models.ReporteProduccion
{
    public class ReporteProducion
    {
        public string IdReporteProduccion { get; set; }
        public string Operadores { get; set; }
        public decimal TotalPlantGen { get; set; }
        public decimal TotalPlantExportION { get; set; }
        public decimal TotalGasConsumption { get; set; }
        public decimal Efficiency { get; set; }
        public decimal ConsumoServiciosAuxiliares { get; set; }
        public string Fecha { get; set; }
    }

    public class ArranqueSincronizacionJson
    {
        [JsonProperty("Anual")]
        public int Anual { get; set; }

        [JsonProperty("Mensual")]
        public int Mensual { get; set; }

        [JsonProperty("Diario")]
        public int Diario { get; set; }

        [JsonProperty("NumeroGenerador")]
        public int NumeroGenerador { get; set; }

        [JsonProperty("Tipo")]
        public string Tipo { get; set; }

        [JsonProperty("IdReporteProduccion")]
        public string IdReporteProduccion { get; set; }

        [JsonProperty("Fecha")]
        public string? Fecha { get; set; }
    }
    public class ArranqueSincronizacion1Arranque : ArranqueSincronizacionJson { }
    public class ArranqueSincronizacion2Arranque : ArranqueSincronizacionJson { }
    public class ArranqueSincronizacion1Sincronizacion : ArranqueSincronizacionJson { }
    public class ArranqueSincronizacion2Sincronizacion : ArranqueSincronizacionJson { }
}
