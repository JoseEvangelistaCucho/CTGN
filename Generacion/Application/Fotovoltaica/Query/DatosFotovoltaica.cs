using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.FotoVoltaica;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.Fotovoltaica.Query
{
    public class DatosFotovoltaica
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosFotovoltaica(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }

        public async Task<Respuesta<ReporteFotovoltaica>> ObtenerDatosFotovoltaica(string fecha)
        {
            Respuesta<ReporteFotovoltaica> respuesta = new Respuesta<ReporteFotovoltaica>();
            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                try
                {
                    connection.Open();

                    OracleCommand cmd = new OracleCommand("ConsultarReporteFotovoltaica", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = fecha;
                    cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                    
                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_Cursor"].Value).GetDataReader();

                    while (reader.Read())
                    {
                        respuesta.Detalle = new ReporteFotovoltaica()
                        {
                           IdReporteFotoVol = reader["IdReporteFotoVol"].ToString(),
                            /* EstandarCarbono = int.Parse(reader["EstandarCarbono"].ToString()),
                            ReduccionCo2 = int.Parse(reader["ReduccionCo2"].ToString()),
                            PlantacionArboles = int.Parse(reader["PlantacionArboles"].ToString()),*/
                            ImagenReporte = reader["ImagenReporte"].ToString(),
                            Manager = reader["Manager"].ToString(),
                            Safety = reader["Safety"].ToString(),
                            HumanResources = reader["HumanResource"].ToString(),
                            Nota = reader["Nota"].ToString(),
                            Fecha = reader["Fecha"].ToString()
                        };
                    }

                }
                catch (Exception ex)
                {

                }
            }
            return respuesta;
        }


        public async Task<Respuesta<List<FotovoltaicaGenerada>>> ObtenerDetalleGenerado(string fecha,int numeroDia)
        {
            Respuesta<List<FotovoltaicaGenerada>> respuesta = new Respuesta<List<FotovoltaicaGenerada>>();
            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                try
                {
                    connection.Open();

                    OracleCommand cmd = new OracleCommand("BuscarDetalleFotoAnio", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = fecha;
                    cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                    cmd.Parameters.Add("p_NumeroMes", OracleDbType.Int32).Value = numeroDia;

                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_Cursor"].Value).GetDataReader();

                    respuesta.Detalle = new List<FotovoltaicaGenerada>();
                    FotovoltaicaGenerada fotovoltaica = new FotovoltaicaGenerada();

                    while (reader.Read())
                    {
                        fotovoltaica = new FotovoltaicaGenerada()
                        {
                            IdReporteFotoVol = reader["IdReporteFotoVol"].ToString(),
                            Detalle = decimal.Parse(reader["detalle"].ToString()),
                            NumeroMes = int.Parse(reader["NumeroMes"].ToString()),
                            Fecha = reader["fecha"].ToString(),
                            EstandarCarbono = int.Parse(reader["estandarCarbono"].ToString()),
                            ReduccionCo2 = int.Parse(reader["reduccionCo2"].ToString()),
                            PlantacionArboles = int.Parse(reader["plantacionArboles"].ToString()),
                            IdVoltajeGenerado = reader["IdReporteFotoVol"].ToString()
                        };

                        respuesta.Detalle.Add(fotovoltaica);
                    }

                }
                catch (Exception ex)
                {

                }
            }
            return respuesta;
        }

    }
}
