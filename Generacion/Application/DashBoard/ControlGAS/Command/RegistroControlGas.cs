using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.ControlGAS;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.DashBoard.ControlGAS.Command
{
    public class RegistroControlGas : IRegistroControlGas
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistroControlGas(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }
        public async Task<Respuesta<string>> RegistrarContratosControl(ContratoGas contratoGas)
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

                        using (OracleCommand command = new OracleCommand("InsertarContratoGas", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_idContratoGas", OracleDbType.Varchar2).Value = contratoGas.IdContratoGas;
                            command.Parameters.Add("p_ConsumoContrato", OracleDbType.Decimal).Value = contratoGas.ConsumoContrato;
                            command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = DateTime.Now.ToString("dd/MM/yyyy");
                            command.Parameters.Add("p_activo", OracleDbType.Int32).Value = contratoGas.Activo;
                            command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = user.IdOperario;
                            command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;

                            command.Parameters.Add("p_resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;
                            command.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;
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
                                string error = command.Parameters["p_resultado"].Value.ToString();
                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la solicitud.";
            }
            return respuesta;
        }

        public async Task<Respuesta<string>> RegistrarDatosControl(ConsumoGas consumoGas)
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

                        using (OracleCommand command = new OracleCommand("InsertarConsumoGas", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdConsumoGas", OracleDbType.Varchar2).Value = consumoGas.IdConsumoGas;
                            command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = DateTime.Now.ToString("dd/MM/yyyy");
                            command.Parameters.Add("p_diasMes", OracleDbType.Decimal).Value = consumoGas.DiasMes;
                            command.Parameters.Add("p_ConsumoDelMes", OracleDbType.Decimal).Value = consumoGas.ConsumoDelMes;
                            command.Parameters.Add("p_idContratoGas", OracleDbType.Varchar2).Value = consumoGas.IdContratoGas;
                            command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = user.IdOperario;
                            command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;

                            command.Parameters.Add("p_resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;
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
                respuesta.Mensaje = "Error al procesar la solicitud.";
            }
            return respuesta;
        }

        public async Task<Respuesta<string>> RegistrarDetalleControl(DetalleConsumoGas detalleGas)
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

                        using (OracleCommand command = new OracleCommand("InsertarDetalleConsumoGas", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdDetalleConsumoGas", OracleDbType.Varchar2).Value = detalleGas.IdDetalleConsumoGas;
                            command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = DateTime.Now.ToString("dd/MM/yyyy");
                            command.Parameters.Add("p_ConsumoTotalActual", OracleDbType.Decimal).Value = detalleGas.ConsumoTotalActual;
                            command.Parameters.Add("p_ConsumoDiario", OracleDbType.Decimal).Value = detalleGas.ConsumoDiario;
                            command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = user.IdOperario;
                            command.Parameters.Add("p_IdConsumoGas", OracleDbType.Varchar2).Value = detalleGas.IdConsumoGas;

                            command.Parameters.Add("p_resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;
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
                respuesta.Mensaje = "Error al procesar la solicitud.";
            }
            return respuesta;
        }
    }
}
