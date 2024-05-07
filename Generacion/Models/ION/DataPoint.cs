namespace Generacion.Models.ION
{
    public class DataPoint
    {
        public int SourceID { get; set; }
        public int QuantityID { get; set; }
        public DateTime TimestampUTC { get; set; }
        public double Value { get; set; }
    }
}
