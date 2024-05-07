namespace Generacion.Models.PruebasSemanales.BlackStart
{
    public class DetallePruebaSemanal
    {
        public string IdDetallePruebaSemanal { get; set; }
        public string IdSubtitulo { get; set; }
        public string ValorReferencia { get; set; }
        public decimal DetalleNumerico { get; set; }
        public string DetalleCadena { get; set; }
        public string Observaciones { get; set; }
        public string Fecha { get; set; }
       // public string IdOperario { get; set; }
        public string IdPruebaSemanal { get; set; }
    }
}
