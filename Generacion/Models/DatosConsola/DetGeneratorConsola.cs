namespace Generacion.Models.DatosConsola
{
    public class DetGeneratorConsola
    {
        /// <summary>
        /// id de tabla 
        /// </summary>
        public string IdDetGeneratorConsola { get; set; }
        /// <summary>
        /// hora de registro
        /// </summary>
        public string Hora { get; set; }
        /// <summary>
        /// fecha de registro
        /// </summary>
        public string Fecha { get; set; }
       /// <summary>
       /// Temperatura de linea L1 del generador
       /// </summary>
        public int L1 { get; set; }
        /// <summary>
        /// Temperatura de linea L2 del generador
        /// </summary>
        public int L2 { get; set; }
        /// <summary>
        /// Temperatura de linea L3 del generador
        /// </summary>
        public int L3 { get; set; }
        /// <summary>
        /// temperatura del cojinete lado acople "D"
        /// </summary>
        public int D { get; set; }
        /// <summary>
        /// Temperatura del cojinete lado libre "ND"
        /// </summary>
        public int ND { get; set; }
        /// <summary>
        /// torsion vibrasional del generador con respecto al motor
        /// </summary>
        public double TersionalVibration { get; set; }
        /// <summary>
        /// id de cabecera
        /// </summary>
        ///<example>"TCA" o "TCB" </example>
        public string IdFormatoConsola { get; set; }
    }

    public class DetGeneratorTC
    {
        /// <summary>
        /// id de tabla
        /// </summary>
        public string IdDetGenerator { get; set; }
        /// <summary>
        /// fecha de tabla
        /// </summary>
        public string Fecha { get; set; }
        /// <summary>
        /// id de cabeceraTabla
        /// </summary>
        public string IdTipoEngine { get; set; }
        /// <summary>
        /// Temperatura a salida del turbocompresor
        /// </summary>
        public int TempOut { get; set; }
        /// <summary>
        /// Velocidad de salida del turbocompresor
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// id de tabla DetGeneratorConsola
        /// </summary>
        public string IdDetGeneratorConsola { get; set; }
    }

    public class RegistroDatosGenerator : DetGeneratorConsola
    {
        public List<DetGeneratorTC> detalleGeneradores { get; set; }
    }

    public class RegistrosDatosGenerator : DetGeneratorConsola
    {
        public Dictionary<string, DetGeneratorTC> detalleGeneradores { get; set; }
    }

}
