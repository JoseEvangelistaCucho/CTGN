using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;

namespace Generacion.Application.DataBase
{
    public interface IConexionBD
    {
        OracleConnection ObtenerConexion();
        SqlConnection ObtenerConexionSQL();
    }
}
