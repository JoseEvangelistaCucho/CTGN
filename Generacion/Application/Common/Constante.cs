namespace Generacion.Application.Common
{
    /// <summary>
    /// Valores constantes para todo el proyecto
    /// </summary>
    public class Constante
    {
        public const int posicionInicioOutTage = 5;
        public const int posicionLetraA = 65;
        public const string formatoExcelHora = "[h]:mm:ss";
        public const string idContextExcel = "excelPackage";
         
        public const int idVistaReporteMantenimiento = 1;
        public const string seleccionarTodo = "ALL";

        public const string codigoIONLuren = "441";
        public const string codigoIONPedregal = "";
        public const string codigoCentralLuren = "LUREN";
        public const string codigoCentralPedregal = "PEDREGAL";



        public const string codigosION = "12889,12890,12891,12892,12893,12894,537,540";
        public const string codigosIONPedregal = "12889,12890,12891,12892,12893,12894,537,540";

       


        /// <summary>
        /// valores para CSV
        /// </summary>
        public const string keyCSV = "DatosCSV";
        public const string seleccionCostosMarginales = "1";
        public const string facturaModulacion = "3";
        public const string dataCoes = "4";
        public const string dataBess = "5";


    }

    public class ReporteFormatoRAM
    {
        public const int espacioCabeceraVistaData = 2;
        public const int espacioTablaVistaData = 3;
        public const string tituloOutage = "Registro de Eventos C.G.D.";

        

    }
    /// <summary>
    /// Tipos de componente
    /// </summary>
    public class TipoComponente
    {
        public const string filtroCentrifugo = "FiltroCentrifugo";
        public const string filtroAutomatico = "FiltroAutomatico";

    }

    /// <summary>
    /// Mensajes constantes para las ejecuciones de procedimientos y handlers
    /// </summary>
    public class Mensajes
    {
        public const string sucess = "Insertado correctamente";
        public const string update = "Actualizado correctamente";
        public const string error = "Error al ejecutar el procedimiento almacenado";
    }
}
