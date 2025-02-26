using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.CalibracionValvula;
using Generacion.Models.Usuario;
using MediatR;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.DashBoard.CalibracionValvula.Command
{
    public class RegistroCalibracionValvula : IRegistroCalibracionValvula
    {
        private readonly Logger _logger;
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistroCalibracionValvula(IConexionBD conexionBD, Function function, Logger logger)
        {
            _conexion = conexionBD;
            _function = function;
        }
        public async Task<Respuesta<string>> GuardarDatosCalibracionValvula(DetalleCalibracionValvula datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();
                await Task.Run(() =>
                {
                    using (OracleConnection connection = _conexion.ObtenerConexion())
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("InsertarDetCalibracionMotor", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdDetalleCalibracionMotor", OracleDbType.Varchar2).Value = datos.IdDetalleCalibracionMotor;
                            command.Parameters.Add("p_Valor", OracleDbType.Int32).Value = datos.valor;
                            command.Parameters.Add("p_NumeroGenerador", OracleDbType.Int32).Value = datos.NumeroGenerador;
                            command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datos.Fecha;
                            command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                            command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = user.IdOperario;

                            command.Parameters.Add("p_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;
                            command.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;
                            var mensaje = command.Parameters["p_mensaje"].Value.ToString();
                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                            if (respuesta.IdRespuesta == 0)
                            {
                                respuesta.Mensaje = "Insertado correctamente";
                            }
                            else if (respuesta.IdRespuesta == 1)
                            {
                                respuesta.Mensaje = "Actualizado correctamente";
                            }
                            else if (respuesta.IdRespuesta == 99)
                            {
                                respuesta.Mensaje = "Error al ejecutar el procedimiento almacenado";
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al Procesar solicitud.";
                _logger.LogError("Error GuardarDatosCalibracionValvula : " + ex.Message.ToString());
            }

            return respuesta;
        }
    }
}
