using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;

namespace Generacion.Application.DataBase
{
    public class ConexionBD : IConexionBD
    {
        private readonly string _cadenaConexion;
        private readonly string _cadenaConexionSQL;

        public ConexionBD(string cadenaConexion,string cadenaConexionSQL)
        {
            this._cadenaConexionSQL = cadenaConexionSQL;
            this._cadenaConexion = cadenaConexion;
        }

        public OracleConnection ObtenerConexion()
        {
            return new OracleConnection(_cadenaConexion);
        }
        public SqlConnection ObtenerConexionSQL()
        {
            return new SqlConnection(_cadenaConexionSQL);
        }
    }
}

