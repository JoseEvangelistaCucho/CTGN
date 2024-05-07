using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Bess;
using Generacion.Models.DataCoes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.BessLlipata.Query
{
    public class ObtenerDatosBessLlipata
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public ObtenerDatosBessLlipata(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }

        public async Task<Respuesta<List<DBDataBess>>> ObtenerDatosBEES(string fecha,string tipoConsulta)
        {
            Respuesta<List<DBDataBess>> respuesta = new Respuesta<List<DBDataBess>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerDatosBESSPorRangoFecha", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_tipoConsulta", OracleDbType.Varchar2).Value = tipoConsulta;
                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;//.ToString("dd/MM/yy HH:mm:ss");

                        command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["p_resultado"].Value).GetDataReader())
                        {
                            respuesta.Detalle = new List<DBDataBess>();
                            DBDataBess datos = new DBDataBess();

                            while (reader.Read())
                            {
                                datos = new DBDataBess()
                                {
                                    Fecha = DateTime.Parse(reader["fecha"].ToString()),
                                    Hora = reader["Hora"].ToString(),
                                    LLIXI625_PV = decimal.Parse(reader["LLIXI625_PV"].ToString()),
                                    LLIXI622_PV = decimal.Parse(reader["LLIXI622_PV"].ToString()),
                                    LLIXI633_PV = decimal.Parse(reader["LLIXI633_PV"].ToString()),
                                    LLIXI634_PV = decimal.Parse(reader["LLIXI634_PV"].ToString()),
                                    LLIXI634_PV2 = decimal.Parse(reader["LLIXI634_PV2"].ToString())
                                };

                                respuesta.Detalle.Add(datos);
                            }

                            respuesta.Detalle = respuesta.Detalle.OrderBy(item => TimeSpan.Parse(item.Hora)).ToList();

                            var primerElemento = respuesta.Detalle[0];
                            respuesta.Detalle.RemoveAt(0);
                            respuesta.Detalle.Add(primerElemento);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Respuesta<List<Demanda>>> ObtenerDatosCOES(DateTime primerDiaDelMes, DateTime ultimoDiaDelMes)
        {
            Respuesta<List<Demanda>> respuesta = new Respuesta<List<Demanda>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                
                /*
                    DateTime primerDiaDelMes = new DateTime(fecha.Year, fecha.Month, 1, 0, 30, 0);
                    DateTime ultimoDiaDelMes = primerDiaDelMes.AddMonths(1);
                */

                int cantidadDeDias = (ultimoDiaDelMes - primerDiaDelMes).Days + 1;


                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerDatosCOESPorRangoFecha", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_fecha_inicio", OracleDbType.Varchar2).Value = primerDiaDelMes.ToString("dd/MM/yyyy HH:mm:ss");
                        command.Parameters.Add("p_fecha_fin", OracleDbType.Varchar2).Value = ultimoDiaDelMes.ToString("dd/MM/yyyy HH:mm:ss");

                        command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["p_resultado"].Value).GetDataReader())
                        {
                            respuesta.Detalle = new List<Demanda>();
                            Demanda datos = new Demanda();

                            while (reader.Read())
                            {
                                datos = new Demanda()
                                {
                                    Fecha = DateTime.Parse(reader["fecha"].ToString()),
                                    Hora = reader["Hora"].ToString(),
                                    Ejecutado = decimal.Parse(reader["ejecutado"].ToString()),
                                    ProgramacionDiaria = decimal.Parse(reader["programacionDiaria"].ToString()),
                                    ProgramacionSemanal = decimal.Parse(reader["programacionSemanal"].ToString())
                                };

                                respuesta.Detalle.Add(datos);
                            }
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
