namespace Generacion.Models.DatosConsola
{
    public class DatosEngine
    {
        public string IdDetEngineConsola { get; set; }
        public string Hora { get; set; }
        public double RunHours { get; set; }
        public double CATemp { get; set; }
        public double DiffPressJetPulse { get; set; }
        public double CAPress { get; set; }
        public double ExhGasAvgTemp { get; set; }
        public double CylPressAvg { get; set; }
        public string Fecha { get; set; }
        public string IdFormatoConsola { get; set; }
    }

    public class DetalleEngine
    {
        public string IdDetEngine { get; set; }
        public string IdTipoEngine { get; set; }
        public string Fecha { get; set; }
        public double Presion { get; set; }
        public double Temp { get; set; }
        public string IdDetEngineConsola { get; set; }

    }
    public class RegistroDetalleEngine : DatosEngine
    {
        public List<DetalleEngine> detalleEngines { get; set; }
    }
    public class RegistrosDatosEngine : DatosEngine
    {
        public Dictionary<string, DetalleEngine> detalleEngine { get; set; }
    }
}
