using Generacion.Application.DataBase;
using Generacion.Models.LecturasCampo;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using Generacion.Application.Funciones;

namespace Generacion.Application.LecturaCampo.Command
{
    public class DatosRegistroCampo : ILecturaCampo
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosRegistroCampo(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }

        public async Task<Respuesta<string>> GuardarDatoCampo(FormatoCampo registo)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_InsFormatoCampo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_IdFormatoCampo", OracleDbType.Varchar2).Value = registo.IdFormatoCampo;
                        command.Parameters.Add("p_Observaciones", OracleDbType.Varchar2).Value = registo.Observaciones;
                        command.Parameters.Add("p_Observaciones1", OracleDbType.Varchar2).Value = string.Empty;
                        command.Parameters.Add("p_Observaciones2", OracleDbType.Varchar2).Value = string.Empty;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = registo.Fecha;
                        command.Parameters.Add("p_IdSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

                        command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                        if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = "Ok";
                        }
                        else
                        {
                            respuesta.Mensaje = "No pudo consultar.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarDatosPrincipal(List<DatosFormatoCampo> datosFormatoCampo)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    foreach (var dato in datosFormatoCampo)
                    {
                        using (OracleCommand cmd = new OracleCommand("InsertOrUpdateDetalleCampo", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("p_IdDetalleCampo", OracleDbType.Varchar2).Value = dato.IdDetalleCampo;
                            cmd.Parameters.Add("p_Detalle", OracleDbType.Decimal).Value = dato.Detalle;
                            cmd.Parameters.Add("p_IdSubtituloCampo", OracleDbType.Varchar2).Value = dato.IdSubtituloCampo;
                            cmd.Parameters.Add("p_IdReporteCampo", OracleDbType.Varchar2).Value = dato.IdReporteCampo;
                            cmd.Parameters.Add("p_Hora", OracleDbType.Varchar2).Value = dato.Hora;
                            cmd.Parameters.Add("p_Fila", OracleDbType.Decimal).Value = dato.Fila;
                            cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = dato.Sitio;
                            cmd.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = dato.Fecha;
                            cmd.Parameters.Add("p_NumeroGenerador", OracleDbType.Decimal).Value = dato.NumeroGenerador;

                            OracleParameter p_Respuesta = new OracleParameter("p_resultado", OracleDbType.Decimal);
                            p_Respuesta.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(p_Respuesta);

                            cmd.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)cmd.Parameters["p_resultado"].Value;

                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }
    }
}
