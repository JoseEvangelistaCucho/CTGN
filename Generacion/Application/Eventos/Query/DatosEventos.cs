using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.ReporteProduccion;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Generacion.Application.Eventos.Query
{
    public class DatosEventos
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosEventos(IConexionBD conexion, Function function)
        {
            _conexion = conexion;
            _function = function;
        }


        public async Task<Respuesta<List<RegistroEventos>>> obtenerDatosEventoPorFecha(string fecha, string tipoBusqueda, string nombreReporte)
        {
            Respuesta<List<RegistroEventos>> respuesta = new Respuesta<List<RegistroEventos>>();
            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ConsultarEventosPorFecha", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_tipoBusqueda", OracleDbType.Varchar2).Value = tipoBusqueda;
                        cmd.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                        cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        cmd.Parameters.Add("p_NombreReporte", OracleDbType.Varchar2).Value = nombreReporte;
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();

                        respuesta.Detalle = new List<RegistroEventos>();
                        RegistroEventos  registro = new RegistroEventos();
                        while (reader.Read())
                        {
                            registro = new RegistroEventos()
                            {
                                IdRegEventos = reader["idregeventos"].ToString(),
                                NumeroGenerador = int.Parse(reader["numerogenerador"].ToString()),
                                FechaParada = reader["fechaparada"].ToString(),
                                FechaArranque = reader["fechaarranque"].ToString(),
                                Sistema = reader["sistema"].ToString(),
                                UnidadFuncional = reader["unidadfuncional"].ToString(),
                                ExternalTrips = reader["externaltrips"].ToString(),
                                ForcedMaint = reader["forcedmaint"].ToString(),
                                PlannedMaint = reader["plannedmaint"].ToString(),
                                StandBy = reader["standby"].ToString(),
                                DescripcionEvento = reader["descripcionevento"].ToString(),
                                IdReporte = reader["idreporte"].ToString(),
                                nombreReporte = reader["nombrereporte"].ToString()
                            };

                            respuesta.Detalle.Add(registro);
                        }
                        respuesta.Detalle = respuesta.Detalle.OrderBy(x => DateTime.Parse(x.FechaParada)).ToList();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
    }
}
