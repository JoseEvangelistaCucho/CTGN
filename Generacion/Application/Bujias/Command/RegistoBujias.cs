using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Almacen.Bujias;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.Bujias.Command
{
    public class RegistoBujias : IBujias
    {
        private readonly IConexionBD _conexion;
        private readonly Logger _logger;
        public RegistoBujias(IConexionBD conexion, Logger logger)
        {
            _conexion = conexion;
            _logger = logger;
        }

        public async Task<Respuesta<string>> GuardarOActualizarRegisto(List<RegistroBujias> datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            respuesta.IdRespuesta = -1;
            respuesta.Mensaje = "No Actualizado";
            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                try
                {
                    connection.Open();

                    foreach (var registro in datos)
                    {
                        using (OracleCommand cmd = new OracleCommand("InsertOrUpdateControlCambio", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("p_IdDetalleControlCambio", OracleDbType.Varchar2).Value = registro.IdDetalleControlCambio;
                            cmd.Parameters.Add("p_Detalle", OracleDbType.Int32).Value = registro.Detalle;
                            cmd.Parameters.Add("p_Nota", OracleDbType.Varchar2).Value = registro.Nota;
                            cmd.Parameters.Add("p_NumeroEG", OracleDbType.Int32).Value = registro.NumeroGenerador;
                            cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = registro.Sitio;
                            cmd.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = registro.Fecha;
                            cmd.Parameters.Add("p_Lado", OracleDbType.Varchar2).Value = registro.Lado;
                            cmd.Parameters.Add("p_IdSubtituloCampo", OracleDbType.Varchar2).Value = registro.IdSubtituloCampo;
                            cmd.Parameters.Add("p_Fila", OracleDbType.Int32).Value = registro.Fila;
                            cmd.Parameters.Add("p_Item", OracleDbType.Int32).Value = registro.Item;
                            cmd.Parameters.Add("p_IdControlCambio", OracleDbType.Varchar2).Value = registro.IdControlCambio;

                            OracleParameter outParam = new OracleParameter("p_Resultado", OracleDbType.Int32);
                            outParam.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(outParam);

                            cmd.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)cmd.Parameters["p_resultado"].Value;
                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                        }
                        if (respuesta.IdRespuesta == 0)
                        {
                            respuesta.Mensaje = "";
                        }
                        else if (respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = "";
                        }
                        else
                        {
                            respuesta.Mensaje = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    respuesta.IdRespuesta = 99;
                    respuesta.Mensaje = "";
                _logger.LogError("Error GuardarOActualizarRegisto : " + ex.Message.ToString());
                }
            }
            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarOActualizarControlCambio(ControlCambioData controlCambioData)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("SaveOrUpdateControlCambio", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdControlCambio", OracleDbType.Varchar2).Value = controlCambioData.IdControlCambio;
                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = controlCambioData.Sitio;
                        command.Parameters.Add("p_HorasOperacion", OracleDbType.Decimal).Value = controlCambioData.HorasOperacion;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = controlCambioData.Fecha;
                        command.Parameters.Add("p_NumeroGenerador", OracleDbType.Decimal).Value = controlCambioData.NumeroGenerador;
                        command.Parameters.Add("p_IdOperario", OracleDbType.Varchar2).Value = controlCambioData.IdOperario;

                        OracleParameter resultadoParam = new OracleParameter("p_Resultado", OracleDbType.Decimal);
                        resultadoParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultadoParam);

                        command.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;
                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;

                    }
                }
            }
            catch (Exception ex)
            { 
                _logger.LogError("Error GuardarOActualizarControlCambio : " + ex.Message.ToString());
            }
            return respuesta;
        }
    }
}
