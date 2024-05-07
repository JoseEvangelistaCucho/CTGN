using Dapper;
using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Aceite;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Generacion.Application.DashBoard.CambioAceite.Query
{
    public class DatosControlRegistroAceite
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosControlRegistroAceite(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }
        public async Task<Respuesta<List<ControlCambioAceite>>> ObtenerDatosControlAceite()
        {
            Respuesta<List<ControlCambioAceite>> respuesta = new Respuesta<List<ControlCambioAceite>>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("ObtenerDatosCambioAceite", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            respuesta.Detalle = new List<ControlCambioAceite>();

                            while (reader.Read())
                            {
                                respuesta.Detalle.Add(new ControlCambioAceite()
                                {
                                    IdControlCambioAceite = reader["idControlCambioAceite"].ToString(),
                                    HorasCambio = int.Parse(reader["HorasCambio"].ToString()),
                                    Tipo = reader["Tipo"].ToString(),
                                    NumeroGenerador = int.Parse(reader["Numerogenerador"].ToString()),
                                    FechaCambio = reader["fechacambio"].ToString(),
                                    Activo = int.Parse(reader["Activo"].ToString())
                                });
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

        public async Task<Respuesta<DetalleControlAceite>> ObtenerDetalleControlAceite(int numeroGenerador)
        {
            Respuesta<DetalleControlAceite> respuesta = new Respuesta<DetalleControlAceite>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("ObtenerDetalleCambioAceite", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_NumeroGenerador", OracleDbType.Varchar2).Value = numeroGenerador;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                respuesta.Detalle = new DetalleControlAceite()
                                {
                                    IdDetalleControlCambio = reader["IdDetalleControlCambio"].ToString(),
                                    IdControlCambioAceite = reader["idControlCambioAceite"].ToString(),
                                    HorasOperacion = int.Parse(reader["HorasOperacion"].ToString()),
                                    NumeroGenerador = int.Parse(reader["numeroGenerador"].ToString()),
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
    }
}
