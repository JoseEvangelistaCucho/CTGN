namespace Generacion.Models.FotoVoltaica
{
    public class FotovoltaicaGenerada
    {
        public string IdVoltajeGenerado { get; set; }
        public decimal Detalle { get; set; }
        public string Fecha { get; set; }
        public string IdReporteFotoVol { get; set; }



        public int NumeroMes { get; set; }
        public int EstandarCarbono { get; set; }
        public int ReduccionCo2 { get; set; }
        public int PlantacionArboles { get; set; }
    }
}
