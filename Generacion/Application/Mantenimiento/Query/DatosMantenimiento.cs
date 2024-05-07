using Generacion.Application.DataBase;
using Generacion.Models.ION;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Generacion.Models.Mantenimiento;
using Oracle.ManagedDataAccess.Types;
using Generacion.Application.Funciones;

namespace Generacion.Application.Mantenimiento.Query
{
    public class DatosMantenimiento
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosMantenimiento(IConexionBD conexion, Function function)
        {
            _conexion = conexion;
            _function = function;
        }

        public async Task<Respuesta<ReporteDiarioMantenimiento>> ObtenerDatosMGD(string fecha)
        {
            Respuesta<ReporteDiarioMantenimiento> respuesta = new Respuesta<ReporteDiarioMantenimiento>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ConsultarReporteDiario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            using (OracleDataReader reader = command.ExecuteReader())
                            {
                                if (!command.Parameters["p_resultado"].Value.Equals(DBNull.Value))
                                {
                                    OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                                    respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                                }
                                if (respuesta.IdRespuesta == 0)
                                {
                                    respuesta.Detalle = new ReporteDiarioMantenimiento();
                                    while (reader.Read())
                                    {
                                        respuesta.Detalle.IdReporteDiario = reader["idreportediario"].ToString();
                                        respuesta.Detalle.Fecha = reader["fecha"].ToString();
                                        respuesta.Detalle.HorasMotor01 = int.Parse(reader["horasmotor01"].ToString());
                                        respuesta.Detalle.HorasMotor02 = int.Parse(reader["horasmotor02"].ToString());
                                        respuesta.Detalle.IdOperario = reader["idoperario"].ToString();
                                    }
                                }
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



        public async Task<Respuesta<List<CilindroAceiteCarter>>> ObtenerRegistroTKVesselPorFecha (string fecha,int numeroGenerador)
        {
            Respuesta<List<CilindroAceiteCarter>> respuesta = new Respuesta<List<CilindroAceiteCarter>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerTKVesselPorFecha", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_IdSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

                        command.Parameters.Add("p_Resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            using (OracleDataReader reader = command.ExecuteReader())
                            {
                                if (!command.Parameters["p_Resultado"].Value.Equals(DBNull.Value))
                                {
                                    OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_Resultado"].Value;

                                    respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                                }
                                if (respuesta.IdRespuesta == 0)
                                {
                                    respuesta.Detalle = new List<CilindroAceiteCarter>();
                                    while (reader.Read())
                                    {
                                        var datos = new CilindroAceiteCarter();
                                        datos.IdCarter = reader["IdCarter"].ToString();
                                        datos.NumeroGenerador = int.Parse(reader["NumeroGenerador"].ToString());
                                        datos.NivelCarterNuevo = decimal.Parse(reader["NivelCarterNuevo"].ToString());
                                        datos.NivelTKNuevo = decimal.Parse(reader["NivelTKNuevo"].ToString());
                                        datos.ContometroNuevo = decimal.Parse(reader["ContometroNuevo"].ToString());
                                        datos.IdReporteDiario = reader["IdReporteDiario"].ToString();
                                        datos.Fecha = DateTime.Parse(reader["Fecha"].ToString()).ToString("dd/MM/yyyy");

                                        respuesta.Detalle.Add(datos);
                                    }
                                }
                            }
                        }
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
