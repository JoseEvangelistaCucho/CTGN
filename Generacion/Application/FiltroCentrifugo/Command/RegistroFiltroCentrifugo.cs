using Generacion.Models.DashBoard;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using Generacion.Application.DataBase;
using Generacion.Models.FiltroCentrifugo;
using Generacion.Application.Funciones;
using Generacion.Models.Usuario;

namespace Generacion.Application.FiltroCentrifugo.Command
{
    public class RegistroFiltroCentrifugo : IRegistroFiltroCentrifugo
    {
        private readonly Function _function;
        private readonly IConexionBD _conexion;
        public RegistroFiltroCentrifugo(IConexionBD conexionBD, Function function)
        {
            _conexion = conexionBD;
            _function = function;
        }

        public async Task<Respuesta<string>> GuardarDatosFiltro(List<DetalleFiltro> detalleFiltro)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            DetalleOperario user = await _function.ObtenerDatosOperario();

            if (detalleFiltro.Count !=0)
            {
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
                                command.Parameters.Add("p_HorasTrabajadasFiltro", OracleDbType.Decimal).Value = item.HorasTrabajadasFiltro;
                                command.Parameters.Add("p_Espesor", OracleDbType.Decimal).Value = item.Espesor;
                                command.Parameters.Add("p_HorasOperacionMANTTO", OracleDbType.Decimal).Value = item.HorasOperacionMantto;
                                command.Parameters.Add("p_HorasOpUltimoMANTTO", OracleDbType.Decimal).Value = 0;
                                command.Parameters.Add("p_Observaciones", OracleDbType.Varchar2).Value = item.Observaciones;
                                command.Parameters.Add("p_IdReporteFiltro", OracleDbType.Varchar2).Value = item.IdReporteFiltro;
                                command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;

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
                    respuesta.IdRespuesta = 99;
                    respuesta.Mensaje = "Error desconocido.";

                }
            }
            else
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "No existen datos.";

            }

            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarDetalleFiltro(List<EspecificacionFiltro> detalleFiltro)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            DetalleOperario user = await _function.ObtenerDatosOperario();

            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    foreach (var item in detalleFiltro)
                    {
                        using (OracleCommand command = new OracleCommand("GuardarEspecificacionFiltro", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdFiltro", OracleDbType.Varchar2).Value = item.IdFiltro;
                            command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = item.Fecha;
                            command.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = item.Tipo;
                            command.Parameters.Add("p_Serie", OracleDbType.Varchar2).Value = item.Serie;
                            command.Parameters.Add("p_Especificacion", OracleDbType.Varchar2).Value = item.Especificacion;
                            command.Parameters.Add("p_TipoMantenimiento", OracleDbType.Varchar2).Value = item.TipoMantenimiento;
                            command.Parameters.Add("p_NumeroGenerador", OracleDbType.Decimal).Value = item.NumeroGenerador;
                            command.Parameters.Add("p_IdReporteFiltro", OracleDbType.Varchar2).Value = item.IdReporteFiltro;
                            command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;

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

        public async Task<Respuesta<string>> GuardarReporteFiltro(ReporteFiltro reporte)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            DetalleOperario user = await _function.ObtenerDatosOperario();
            try
            {

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    using (OracleCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GuardarReporteFiltro";

                        command.Parameters.Add("p_IdReporteFiltro", OracleDbType.Varchar2).Value = reporte.IdReporteFiltro;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = reporte.Fecha;
                        command.Parameters.Add("p_IdOperador", OracleDbType.Varchar2).Value = user.IdOperario;
                        command.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = reporte.Tipo;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;

                        command.Parameters.Add("p_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        connection.Open();
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
