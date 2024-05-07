using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.TurboCompresor;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.DashBoard.LavadoTurbo.Command
{
    public class RegistroLavadoTurbo : IRegistroLavadoTurbo
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistroLavadoTurbo(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }

        public async Task<Respuesta<string>> GuardarDatosTurboLavado(DatoLavadoTurbo request)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("GuardarLavadoTurbo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdLavadoTurbo", OracleDbType.Varchar2).Value = request.IdLavadoTurbo;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = request.Fecha;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        command.Parameters.Add("p_IdOperario", OracleDbType.Varchar2).Value = user.IdOperario;

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
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la solicitud.";
            }
            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarDetalleTurboLavado(DetalleLavadoTurbo request)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("GuardarDetalleLavadoTurbo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdDetalleLavadoTurbo", OracleDbType.Varchar2).Value = request.IdDetalleLavadoTurbo;
                        command.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = request.Tipo;
                        command.Parameters.Add("p_Valor", OracleDbType.Int32).Value = request.Valor;
                        command.Parameters.Add("p_NumeroGenerador", OracleDbType.Int32).Value = request.NumeroGenerador;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = request.Fecha;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        command.Parameters.Add("p_IdOperario", OracleDbType.Varchar2).Value = user.IdOperario;
                        command.Parameters.Add("p_IdLavadoTurbo", OracleDbType.Varchar2).Value = request.IdLavadoTurbo;

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
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la solicitud.";
            }
            return respuesta;
        }
    }
}
