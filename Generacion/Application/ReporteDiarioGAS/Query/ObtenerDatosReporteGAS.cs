using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.ReporteDiarioGAS;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using MReporteGAS =Generacion.Models.ReporteDiarioGAS.ReporteGAS;

namespace Generacion.Application.ReporteDiarioGAS.Query
{
    public class ObtenerDatosReporteGAS
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public ObtenerDatosReporteGAS(IConexionBD conexion, Function function)
        {
            _conexion = conexion;
            _function = function;
        }
        public async Task<Respuesta<List<DetalleReporteGas>>> ObtenerDetallesReportePorAño(string año)
        {
            Respuesta<List<DetalleReporteGas>> respuesta = new Respuesta<List<DetalleReporteGas>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerDetalleGASPorAño", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_año", OracleDbType.Varchar2, año.ToUpper(), System.Data.ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("p_sitio", OracleDbType.Varchar2, datosOperario.IdSitio, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            respuesta.Detalle = new List<DetalleReporteGas>();
                            DetalleReporteGas detalle = new DetalleReporteGas();

                            while (reader.Read())
                            {
                                detalle = new DetalleReporteGas();
                                detalle.IdDetalleReporte = reader["IdDetalleReporte"].ToString();
                                detalle.Fecha = reader["Fecha"].ToString();
                                detalle.Hora = reader["Hora"].ToString();
                                detalle.VC = decimal.Parse(reader["VC"].ToString());
                                detalle.VN = decimal.Parse(reader["VN"].ToString());
                                detalle.PL = decimal.Parse(reader["PL"].ToString());
                                detalle.FL = decimal.Parse(reader["FL"].ToString());
                                detalle.SV = decimal.Parse(reader["SV"].ToString());
                                detalle.TL = decimal.Parse(reader["TL"].ToString());
                                detalle.FC = decimal.Parse(reader["FC"].ToString());
                                detalle.FS = decimal.Parse(reader["FS"].ToString());
                                detalle.DA = decimal.Parse(reader["DA"].ToString());
                                detalle.DP = decimal.Parse(reader["DP"].ToString());
                                detalle.PR = decimal.Parse(reader["PR"].ToString());
                                detalle.D1 = decimal.Parse(reader["D1"].ToString());
                                detalle.D2 = decimal.Parse(reader["D2"].ToString());
                                // detalle.L = decimal.Parse(reader["L"].ToString());
                                detalle.Caudalimetro = decimal.Parse(reader["Caudalimetro"].ToString());
                                detalle.PresionIngreso = decimal.Parse(reader["PresionIngreso"].ToString());
                                detalle.IdReporteGas = reader["IdReporteGas"].ToString();

                                respuesta.Detalle.Add(detalle);
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

        public async Task<Respuesta<Dictionary<string, List<DetalleReporteGas>>>> ObtenerDetallesReporte(string id)
        {
            Respuesta<Dictionary<string, List<DetalleReporteGas>>> respuesta = new Respuesta<Dictionary<string, List<DetalleReporteGas>>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerDetalleGAS", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_IdReporteGas", OracleDbType.Varchar2, id.ToUpper(), System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<DetalleReporteGas> datos = new List<DetalleReporteGas>();
                            DetalleReporteGas detalle = new DetalleReporteGas();

                            while (reader.Read())
                            {
                                detalle = new DetalleReporteGas();
                                detalle.IdDetalleReporte = reader["IdDetalleReporte"].ToString();
                                detalle.Fecha = reader["Fecha"].ToString();
                                detalle.Hora = reader["Hora"].ToString();
                                detalle.VC = decimal.Parse(reader["VC"].ToString());
                                detalle.VN = decimal.Parse(reader["VN"].ToString());
                                detalle.PL = decimal.Parse(reader["PL"].ToString());
                                detalle.FL = decimal.Parse(reader["FL"].ToString());
                                detalle.SV = decimal.Parse(reader["SV"].ToString());
                                detalle.TL = decimal.Parse(reader["TL"].ToString());
                                detalle.FC = decimal.Parse(reader["FC"].ToString());
                                detalle.FS = decimal.Parse(reader["FS"].ToString());
                                detalle.DA = decimal.Parse(reader["DA"].ToString());
                                detalle.DP = decimal.Parse(reader["DP"].ToString());
                                detalle.PR = decimal.Parse(reader["PR"].ToString());
                                detalle.D1 = decimal.Parse(reader["D1"].ToString());
                                detalle.D2 = decimal.Parse(reader["D2"].ToString());
                                // detalle.L = decimal.Parse(reader["L"].ToString());
                                detalle.Caudalimetro = decimal.Parse(reader["Caudalimetro"].ToString());
                                detalle.PresionIngreso = decimal.Parse(reader["PresionIngreso"].ToString());
                                detalle.IdReporteGas = reader["IdReporteGas"].ToString();

                                datos.Add(detalle);
                            }
                            var lineaUno = datos
                                .Where(x => x.Hora.StartsWith("18"))
                                .ToList();
                            var lineaDos = datos
                                .Where(x => x.Hora.StartsWith("6"))
                                .ToList();

                            lineaUno = lineaUno.OrderBy(x => DateTime.Parse(x.Fecha)).ToList();
                            lineaDos = lineaDos.OrderBy(x => DateTime.Parse(x.Fecha)).ToList();

                            respuesta.Detalle = new Dictionary<string, List<DetalleReporteGas>>();
                            respuesta.Detalle.Add("18", lineaUno);
                            respuesta.Detalle.Add("6", lineaDos);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return respuesta;
        }

        public async Task<Respuesta<MReporteGAS>> ObtenerIdReporteGAS()
        {
            Respuesta<MReporteGAS> respuesta = new Respuesta<MReporteGAS>();
            DetalleOperario user = await _function.ObtenerDatosOperario();

            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerIdReporteGAS", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            MReporteGAS formatoConsola = new MReporteGAS();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                formatoConsola = new MReporteGAS();
                                formatoConsola.IdReporteGas = row["IdReporteGas"].ToString();
                                formatoConsola.Fecha = row["Fecha"].ToString();
                                formatoConsola.Activo = int.Parse(row["Activo"].ToString());
                            }

                            //crear una lista de los idReportes validar con el idSitio y obtener el que corresponde
                            //formatoConsola = listaFormato.where(x => x.idReporteGas.starWith(idSitio)).select(x => x).FistOrDefault();
                            respuesta.Detalle = formatoConsola;
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
