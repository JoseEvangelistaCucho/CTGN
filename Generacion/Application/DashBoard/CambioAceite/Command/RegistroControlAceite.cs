using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Aceite;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.DashBoard.CambioAceite.Command
{
    public class RegistroControlAceite : IRegistroControlAceite
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistroControlAceite(IConexionBD conexionBD, Function function)
        {
            _conexion = conexionBD;
            _function = function;
        }

        public async Task<Respuesta<string>> GuardarDatosControlAceite(ControlCambioAceite request)
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

                        using (OracleCommand command = new OracleCommand("InsertarControlCambioAceite", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_idControlCambioAceite", OracleDbType.Varchar2).Value = request.IdControlCambioAceite;
                            command.Parameters.Add("p_numeroGenerador", OracleDbType.Decimal).Value = request.NumeroGenerador;
                            command.Parameters.Add("p_HorasCambio", OracleDbType.Decimal).Value = request.HorasCambio;
                            command.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = request.Tipo;
                            command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = DateTime.Now.ToString("dd/MM/yyyy");
                            command.Parameters.Add("p_fechaCambio", OracleDbType.Varchar2).Value = request.FechaCambio;
                            command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = user.IdOperario;
                            command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                            command.Parameters.Add("p_usoActual", OracleDbType.Int32).Value = request.Activo;

                            command.Parameters.Add("p_resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;
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
                respuesta.Mensaje = "Error al procesar la solicitud.";
            }

            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarDetalleControlAceite(DetalleControlAceite request)
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

                        using (OracleCommand command = new OracleCommand("InsertarDetalleControlAceite", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_idDetalleControlCambio", OracleDbType.Varchar2).Value = request.IdDetalleControlCambio;
                            command.Parameters.Add("p_idControlCambioAceite", OracleDbType.Varchar2).Value = request.IdControlCambioAceite;
                            command.Parameters.Add("p_HorasOperacion", OracleDbType.Decimal).Value = request.HorasOperacion;
                            command.Parameters.Add("p_numeroGenerador", OracleDbType.Decimal).Value = request.NumeroGenerador;
                            command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = DateTime.Now.ToString("dd/MM/yyyy");
                            command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = user.IdOperario;
                            command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;

                            command.Parameters.Add("p_resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;
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
                respuesta.Mensaje = "Error al procesar la solicitud.";
            }
            return respuesta;
        }
    }
}
