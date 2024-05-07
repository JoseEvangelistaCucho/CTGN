using Generacion.Application.DataBase;
using Generacion.Models;
using Generacion.Models.DashBoard;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.DashBoard.Filtro.Command
{
    public class RegistroDashBoard : IDashBoard
    {
        private readonly IConexionBD _conexion;
        public RegistroDashBoard(IConexionBD conexionBD)
        {
            _conexion = conexionBD;
        }
        public async Task<Respuesta<string>> GuardarDatosFiltro(List<DashboardDetalleFiltro> detalleFiltro, string idSitio)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();
                    foreach (var item in detalleFiltro)
                    {
                        using (OracleCommand command = new OracleCommand("proc_GuardarDetalleFiltro", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdDetalleFiltro", OracleDbType.Varchar2).Value = item.IdDetalleFiltro;
                            command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = item.Fecha;
                            command.Parameters.Add("p_OperadorEjecutor", OracleDbType.Varchar2).Value = item.OperadorEjecutor;
                            command.Parameters.Add("p_ProximaHoraCambio", OracleDbType.Decimal).Value = item.ProximaHoraCambio;
                            command.Parameters.Add("p_HorasOperacion", OracleDbType.Decimal).Value = item.HorasOperacion;
                            command.Parameters.Add("p_NumeroGenerador", OracleDbType.Decimal).Value = item.NumeroGenerador;
                            command.Parameters.Add("p_HorasTrabajadasFiltro", OracleDbType.Decimal).Value = 0;//em.HorasTrabajadasFiltro;
                            command.Parameters.Add("p_Espesor", OracleDbType.Decimal).Value = item.Espesor;
                            command.Parameters.Add("p_HorasOperacionMANTTO", OracleDbType.Decimal).Value = 0;// item.HorasOperacionMantto;
                            command.Parameters.Add("p_HorasOpUltimoMANTTO", OracleDbType.Decimal).Value = 0;
                            command.Parameters.Add("p_Observaciones", OracleDbType.Varchar2).Value = "";//item.Observaciones;
                            command.Parameters.Add("p_IdReporteFiltro", OracleDbType.Varchar2).Value = item.IdReporteFiltro;
                            command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = idSitio;

                            command.Parameters.Add("p_Resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_Resultado"].Value;
                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
    }
}
