namespace Generacion.Models.DatosConsola
{
    public class EnergiaGenerada
    {
        public string Hora { get; set; }
        /// <summary>
        /// Codigo : P
        /// </summary>
        public decimal PotenciaActiva { get; set; }
        /// <summary>
        /// Codigo : Q
        /// </summary>
        public decimal PotenciaReactiva { get; set; }
        /// <summary>
        /// Codigo : E+
        /// </summary>
        public decimal EnergiaActiva { get; set; }
        /// <summary>
        /// Codigo : Eq+
        /// </summary>
        public decimal EnergiaReactiva { get; set; }
        /// <summary>
        /// Codigo : lL1
        /// </summary>
        public decimal CorrienteLinea1 { get; set; }

        /// <summary>
        /// Codigo : lL2
        /// </summary>
        public decimal CorrienteLinea2 { get; set; }

        /// <summary>
        /// Codigo : lL3
        /// </summary>
        public decimal CorrienteLinea3 { get; set; }

        /// <summary>
        /// Codigo : U12
        /// </summary>
        public decimal Voltaje { get; set; }

        /// <summary>
        /// Codigo : U23
        /// </summary>
        public decimal Voltaje23 { get; set; }

        /// <summary>
        /// Codigo : U31
        /// </summary>
        public decimal Voltaje31 { get; set; }
        public int Fila { get; set; }

    }

    public class DatosFormatoConsola : EnergiaGenerada
    {

        public string IdDetalleConsola { get; set; }
        public string Fecha { get; set; }       
        public string IdRegistroConsola { get; set; }
        public string IdOperario { get; set; }
        public string IdformatoConsola { get; set; }
    }
}
