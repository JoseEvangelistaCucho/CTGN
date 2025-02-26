using Generacion.Application.DataBase;
using Generacion.Models;
using Generacion.Models.ION;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;

namespace Generacion.Application.ION.Query
{
    public class ConsultarION
    {
        private readonly IConexionBD _conexion;
        private readonly ILogger<ConsultarION> _logger;
        public ConsultarION(IConexionBD conexionBD, ILogger<ConsultarION> logger)
        {
            _conexion = conexionBD;
            _logger = logger;
        }

        public async Task<Respuesta<List<DatosFormatoMGD>>> ObtenerDatosION(string fechaInicio, string fechaFin)
        {
            Respuesta<List<DatosFormatoMGD>> respuesta = new Respuesta<List<DatosFormatoMGD>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_consultarIONPorFecha", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_fechaIni", OracleDbType.Varchar2).Value = string.Concat(fechaInicio + " 00:15");
                        command.Parameters.Add("p_fechaFin", OracleDbType.Varchar2).Value = string.Concat(fechaFin, " 00:00");
                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            List<DatosFormatoMGD> revenues = new List<DatosFormatoMGD>();
                            DatosFormatoMGD revenue = new DatosFormatoMGD();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                revenue = new DatosFormatoMGD();
                                revenue.Date_Time = row["Date_Time"].ToString();
                                revenue.kWhDelInt = decimal.Parse(row["kWh_del_int"].ToString());
                                revenue.kVARhDelInt = decimal.Parse(row["kVARh_del_int"].ToString());
                                revenue.kWhRecInt = decimal.Parse(row["kWh_rec_int"].ToString());
                                revenue.kVARhRecInt = decimal.Parse(row["kVARh_rec_int"].ToString());
                                revenue.VllAvg = decimal.Parse(row["Vll_avg"].ToString());
                                revenue.Freq = decimal.Parse(row["Freq"].ToString());


                                revenue.KWDelInt = revenue.kWhDelInt * 4;
                                revenue.KVARDelInt = revenue.kVARhDelInt * 4;
                                revenue.KWRecInt = revenue.kWhRecInt * 4;
                                revenue.KVARRecInt = (revenue.kVARhRecInt * revenue.kVARhDelInt) * 4;

                                var fechaHora = DateTime.Parse(revenue.Date_Time);

                                string hora = fechaHora.ToString("HH:mm");
                                revenue.Hora = hora;
                                revenues.Add(revenue);
                            }

                            respuesta.Detalle = revenues;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ObtenerDatosION  Error :  {ErrorDetails}", ex.ToString());
                throw;
            }
            return respuesta;
        }

        public async Task<Respuesta<List<DatosFormatoMGD>>> ObtenerDatosIONSQL(string sourceIDs, string quantityIDs, DateTime minTimestampUTC)
        {
            Respuesta<List<DatosFormatoMGD>> respuesta = new Respuesta<List<DatosFormatoMGD>>();
            try
            {
                using (SqlConnection connection = _conexion.ObtenerConexionSQL())
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetDatalogData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agrega parámetros
                        command.Parameters.AddWithValue("@SourceIDs", sourceIDs);  // Reemplaza con tus valores
                        command.Parameters.AddWithValue("@QuantityIDs", quantityIDs);  // Reemplaza con tus valores
                        command.Parameters.AddWithValue("@MinTimestampUTC", minTimestampUTC);  // Reemplaza con tus valores
                        command.Parameters.AddWithValue("@MaxTimestampUTC", minTimestampUTC.AddDays(+1));  // Reemplaza con tus valores

                        // Ejecuta el comando
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<DataPoint> dataPoint = new List<DataPoint>();

                            DataPoint data = new DataPoint();
                            while (reader.Read())
                            {
                                data = new DataPoint()
                                {
                                    SourceID = reader.GetInt32(0),
                                    QuantityID = reader.GetInt32(1),
                                    TimestampUTC = reader.GetDateTime(2).AddHours(-5),
                                    Value = Math.Round(reader.GetDouble(3),3)
                                 };
                                dataPoint.Add(data);
                            }
                            var datosAgrupados = dataPoint.OrderBy(x => x.TimestampUTC).GroupBy(x => x.TimestampUTC).ToList();

                            var datosAgrupadosDiccionario = datosAgrupados.ToDictionary(
                                grupo => grupo.Key, 
                                grupo => grupo.ToList() 
                            );

                            List<DatosFormatoMGD> revenues = new List<DatosFormatoMGD>();
                            DatosFormatoMGD revenue = new DatosFormatoMGD();
                            foreach (var item in datosAgrupadosDiccionario.Values)
                            {
                                revenue = new DatosFormatoMGD()
                                {
                                   Date_Time = item[0].TimestampUTC.ToString(),
                                    VllAvg  = decimal.Parse(item[0].Value.ToString()),
                                    Freq = decimal.Parse(item[1].Value.ToString()),
                                    kWhDelInt = decimal.Parse(item[2].Value.ToString()),
                                   kWhRecInt = decimal.Parse(item[3].Value.ToString()),
                                    kVARhDelInt = decimal.Parse(item[4].Value.ToString()),
                                    kVARhRecInt = decimal.Parse(item[5].Value.ToString()),
                                    KWDelInt = decimal.Parse(item[2].Value.ToString()) * 4,
                                    KVARDelInt = decimal.Parse(item[4].Value.ToString()) * 4,
                                    KWRecInt = decimal.Parse(item[3].Value.ToString()) * 4,
                                    KVARRecInt = (decimal.Parse(item[5].Value.ToString()) * decimal.Parse(item[4].Value.ToString())) * 4
                                };

                                var fechaHora = DateTime.Parse(revenue.Date_Time);

                                string hora = fechaHora.ToString("HH:mm");
                                revenue.Hora = hora;



                                revenues.Add(revenue);
                            }
                            respuesta.Detalle = revenues;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ObtenerDatosIONSQL Error: {ErrorDetails}", ex.ToString());
                throw;
            }
            return respuesta;
        }
    }
}
