using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.DashBoard;
using Generacion.Models.TurboCompresor;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.DashBoard.TurboCompresor.Command
{
    public class RegistroTurboCompresor : IRegistroTurboCompresor
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistroTurboCompresor(IConexionBD conexionBD, Function function)
        {
            _conexion = conexionBD;
            _function = function;
        }

        public async Task<Respuesta<string>> GuardarDatosTurboCompresor(DatosTurboCompresor datosTurboCompresor)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("InsertarTurboCompresor", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdDetalleFiltro", OracleDbType.Varchar2).Value = datosTurboCompresor.IdTurboCompresor;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datosTurboCompresor.Fecha;
                        command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = user.IdOperario;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;

                        command.Parameters.Add("p_Resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                        command.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_Resultado"].Value;
                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarDetalleTurboCompresor(DetalleTurboCompresor detalle)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("InsertarDetalleTurboCompresor", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdDetalleTurboCompresor", OracleDbType.Varchar2).Value = detalle.IdDetalleTurboCompresor;
                        command.Parameters.Add("p_Posicion", OracleDbType.Int32).Value = detalle.Posicion;
                        command.Parameters.Add("p_Lado", OracleDbType.Varchar2).Value = detalle.Lado;
                        command.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = detalle.Tipo;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = detalle.Fecha;
                        command.Parameters.Add("p_NumeroGenerador", OracleDbType.Int32).Value = detalle.NumeroGenerador;
                        command.Parameters.Add("p_Valor", OracleDbType.Decimal).Value = detalle.Valor;
                        command.Parameters.Add("p_IdTurboCompresor", OracleDbType.Varchar2).Value = detalle.IdTurboCompresor;
                        command.Parameters.Add("p_IdOperario", OracleDbType.Varchar2).Value = user.IdOperario;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;

                        command.Parameters.Add("p_Resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                        command.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_Resultado"].Value;
                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
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
