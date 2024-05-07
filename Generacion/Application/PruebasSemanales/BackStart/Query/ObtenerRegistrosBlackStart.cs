using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.PruebasSemanales.BlackStart;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Generacion.Application.PruebasSemanales.BackStart.Query
{
    public class ObtenerRegistrosPruebaSemanal
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public ObtenerRegistrosPruebaSemanal(IConexionBD conexionBD, Function function)
        {
            _conexion = conexionBD;
            _function = function;
        }


        public async Task<Respuesta<PruebaSemanal>> ObtenerDatosPruebaSemanal(string tipo)
        {
            Respuesta<PruebaSemanal> respuesta = new Respuesta<PruebaSemanal>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerPruebaSemanal", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_tipoPrueba", OracleDbType.Varchar2).Value = tipo;

                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            respuesta.Detalle = new PruebaSemanal();
                            while (reader.Read())
                            {
                                respuesta.Detalle = new PruebaSemanal()
                                {
                                    Activo = Convert.ToInt32(reader["Activo"]),
                                    IdPruebaSemanal = reader["idPruebaSemanal"].ToString(),
                                    Fecha = reader["fecha"].ToString()
                                };
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
        public async Task<Respuesta<List<DetallePruebaSemanal>>> ObtenerDetallePruebaSemanal(string IdPruebaSemanal)
        {
            Respuesta<List<DetallePruebaSemanal>> respuesta = new Respuesta<List<DetallePruebaSemanal>>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerDetallesBSPorID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_idPruebaSemanal ", OracleDbType.Varchar2).Value = IdPruebaSemanal;

                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            respuesta.Detalle = new List<DetallePruebaSemanal>();
                            DetallePruebaSemanal detalle = new DetallePruebaSemanal();

                            while (reader.Read())
                            {
                                detalle = new DetallePruebaSemanal()
                                {
                                    IdDetallePruebaSemanal = reader["idDetallePruebaSemanal"].ToString(),
                                    IdSubtitulo = reader["idSubtitulo"].ToString(),
                                    ValorReferencia = reader["valorReferencia"].ToString(),
                                    DetalleNumerico = Convert.ToDecimal(reader["detalleNumerico"]),
                                    DetalleCadena = reader["detalleCadena"].ToString(),
                                    Observaciones = reader["observaciones"].ToString(),
                                    Fecha = reader["fecha"].ToString(),
                                    IdPruebaSemanal = reader["idPruebaSemanal"].ToString()
                                };
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

    }
}
