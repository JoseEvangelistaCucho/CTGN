using Generacion.Application.DataBase;
using Generacion.Models.DashBoard;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Generacion.Application.Funciones;
using Generacion.Models.Usuario;
using Generacion.Models.FiltroCentrifugo;

namespace Generacion.Application.FiltroCentrifugo.Query
{
    public class DatosFiltroCentrifugo
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosFiltroCentrifugo(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }
        public async Task<Respuesta<List<DetalleFiltro>>> ObtenerDatosFiltroPorSitio(string seleccion, string IdReporteFiltro)
        {
            Respuesta<List<DetalleFiltro>> respuesta = new Respuesta<List<DetalleFiltro>>();
            DetalleOperario user = await _function.ObtenerDatosOperario();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ObtenerDetalleFiltro", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        cmd.Parameters.Add("p_Seleccion", OracleDbType.Varchar2).Value = seleccion;
                        cmd.Parameters.Add("p_IdReporteFiltro", OracleDbType.Varchar2).Value = IdReporteFiltro;
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();

                        respuesta.Detalle = new List<DetalleFiltro>();
                        DetalleFiltro detalleFiltro = new DetalleFiltro();
                        while (reader.Read())
                        {
                            detalleFiltro = new DetalleFiltro()
                            {
                                HorasOperacionMantto = decimal.Parse(reader["HorasOperacionMANTTO"].ToString()),
                                IdDetalleFiltro = reader["idDetalleFiltro"].ToString(),
                                Fecha = reader["Fecha"].ToString(),
                                OperadorEjecutor = reader["OperadorEjecutor"].ToString(),
                                Espesor = decimal.Parse(reader["ESPESOR"].ToString()),
                                HorasOperacion = decimal.Parse(reader["HorasOperacion"].ToString()),
                                NumeroGenerador = decimal.Parse(reader["NumeroGenerador"].ToString()),
                                ProximaHoraCambio = decimal.Parse(reader["ProximaHoraCambio"].ToString()),
                                HorasOpUltimoMantto = decimal.Parse(reader["HorasOpUltimoMANTTO"].ToString()),
                                HorasTrabajadasFiltro = decimal.Parse(reader["HorasTrabajadasFiltro"].ToString()),
                                IdReporteFiltro = reader["idReporteFiltro"].ToString(),
                                Observaciones = reader["OBSERVACIONES"].ToString()
                            };

                            respuesta.Detalle.Add(detalleFiltro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Respuesta<List<DetalleFiltro>>> ObtenerDatosFiltroPorSitioyTipo(string tipo)
        {
            Respuesta<List<DetalleFiltro>> respuesta = new Respuesta<List<DetalleFiltro>>();
            DetalleOperario user = await _function.ObtenerDatosOperario();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ObtenerDetalleFiltroPorTipo", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        cmd.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = tipo;
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();

                        respuesta.Detalle = new List<DetalleFiltro>();
                        DetalleFiltro detalleFiltro = new DetalleFiltro();
                        while (reader.Read())
                        {
                            detalleFiltro = new DetalleFiltro()
                            {
                                HorasOperacionMantto = decimal.Parse(reader["HorasOperacionMANTTO"].ToString()),
                                IdDetalleFiltro = reader["idDetalleFiltro"].ToString(),
                                Fecha = reader["Fecha"].ToString(),
                                OperadorEjecutor = reader["OperadorEjecutor"].ToString(),
                                Espesor = decimal.Parse(reader["ESPESOR"].ToString()),
                                HorasOperacion = decimal.Parse(reader["HorasOperacion"].ToString()),
                                NumeroGenerador = decimal.Parse(reader["NumeroGenerador"].ToString()),
                                ProximaHoraCambio = decimal.Parse(reader["ProximaHoraCambio"].ToString()),
                                HorasOpUltimoMantto = decimal.Parse(reader["HorasOpUltimoMANTTO"].ToString()),
                                HorasTrabajadasFiltro = decimal.Parse(reader["HorasTrabajadasFiltro"].ToString()),
                                IdReporteFiltro = reader["idReporteFiltro"].ToString(),
                                Observaciones = reader["OBSERVACIONES"].ToString()
                            };

                            respuesta.Detalle.Add(detalleFiltro);
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
        public async Task<Respuesta<Dictionary<decimal, List<DetalleFiltro>>>> ObtenerDetallePorNumeroGE(string seleccion, string IdReporteFiltro)
        {
            Respuesta<List<DetalleFiltro>> datosFiltro = await ObtenerDatosFiltroPorSitio(seleccion,IdReporteFiltro);

            Respuesta<Dictionary<decimal, List<DetalleFiltro>>> respuesta = new Respuesta<Dictionary<decimal, List<DetalleFiltro>>>();

            datosFiltro.Detalle = datosFiltro.Detalle.OrderBy(x =>
            {
                DateTime fecha;
                return DateTime.TryParse(x.Fecha, out fecha) ? fecha : DateTime.MinValue;
            }).ToList();

            respuesta.Detalle = datosFiltro.Detalle
            .GroupBy(x => x.NumeroGenerador)
            .ToDictionary(
                group => group.Key,
                group => group.ToList()
            );

            return respuesta;
        }


        /// <summary>
        /// Se requiere el TIPO de componente
        /// </summary>
        /// <example> TIPO : FiltroCentrifugo o FiltroAutomatico</example>
        public async Task<Respuesta<ReporteFiltro>> ObtenerReporteFiltro(string tipoComponente)
        {
            Respuesta<ReporteFiltro> respuesta = new Respuesta<ReporteFiltro>();
            DetalleOperario user = await _function.ObtenerDatosOperario();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ObtenerReporteFiltro", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        cmd.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = tipoComponente;

                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();

                        while (reader.Read())
                        {
                            respuesta.Detalle = new ReporteFiltro()
                            {
                                IdReporteFiltro = reader["idReporteFiltro"].ToString(),
                                Tipo = reader["tipo"].ToString(),
                                Fecha = reader["fecha"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return respuesta;
        }



        public async Task<Respuesta<List<EspecificacionFiltro>>> ObtenerDatosEspecificacionFiltro(string idReporteFiltro)
        {
            Respuesta<List<EspecificacionFiltro>> respuesta = new Respuesta<List<EspecificacionFiltro>>();
            DetalleOperario user = await _function.ObtenerDatosOperario();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ObtenerEspecificacionesFiltro", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_IdReporteFiltro", OracleDbType.Varchar2).Value = idReporteFiltro;

                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();

                        respuesta.Detalle = new List<EspecificacionFiltro>();
                        EspecificacionFiltro detalleFiltro = new EspecificacionFiltro();
                        while (reader.Read())
                        {
                            detalleFiltro = new EspecificacionFiltro()
                            {
                                NumeroGenerador = decimal.Parse(reader["NumeroGenerador"].ToString()),
                                Fecha = reader["fecha"].ToString(),
                                Especificacion = reader["especificacion"].ToString(),
                                Tipo = reader["tipo"].ToString(),
                                IdReporteFiltro = reader["idReporteFiltro"].ToString(),
                                IdFiltro = reader["idFiltro"].ToString(),
                                Serie = reader["serie"].ToString(),
                                TipoMantenimiento = reader["tipoMantenimiento"].ToString()
                            };
                            respuesta.Detalle.Add(detalleFiltro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
        public async Task<Respuesta<Dictionary<decimal, EspecificacionFiltro>>> ObtenerEspecificacionesPorNumeroGE(string idReporteFiltro)
        {
            Respuesta<List<EspecificacionFiltro>> datosFiltro = await ObtenerDatosEspecificacionFiltro(idReporteFiltro);

            Respuesta<Dictionary<decimal, EspecificacionFiltro>> respuesta = new Respuesta<Dictionary<decimal, EspecificacionFiltro>>();

            respuesta.Detalle = datosFiltro.Detalle
            .ToDictionary(x => x.NumeroGenerador);

            return respuesta;
        }
    }
}
