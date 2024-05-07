using Generacion.Application.DataBase;
using Generacion.Models.Almacen.Bujias;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Generacion.Application.Funciones;
using Generacion.Models.Usuario;
using Generacion.Models.ControlGAS;

namespace Generacion.Application.DashBoard.ControlGAS.Query
{
    public class ObtenerDetalleConsumoGas
    {
        private readonly IConexionBD _conexion; 
        private readonly Function _function; 
        public ObtenerDetalleConsumoGas(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }
        public async Task<Respuesta<ContratoGas>> ObtenerContratoGas()
        {
            Respuesta<ContratoGas> respuesta = new Respuesta<ContratoGas>();
            DetalleOperario user = await _function.ObtenerDatosOperario();

            try
            {
                await Task.Run(() =>
                {

                    using (OracleConnection connection = _conexion.ObtenerConexion())
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("ObtenerContratosGas", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdSitio", OracleDbType.Varchar2).Value = user.IdSitio;
                            command.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                            using (OracleDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    respuesta.Detalle = new ContratoGas()
                                    {
                                        Activo = int.Parse(reader["Activo"].ToString()),
                                        ConsumoContrato = decimal.Parse(reader["ConsumoContrato"].ToString()),
                                       // Fecha = reader["Fecha"].ToString(),
                                      //  IdOperario = reader["IdOperario"].ToString(),
                                        IdContratoGas = reader["IdContratoGas"].ToString()
                                    };
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }
        public async Task<Respuesta<ConsumoGas>> ObtenerDatosConsumoGas()
        {
            Respuesta<ConsumoGas> respuesta = new Respuesta<ConsumoGas>();
            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();

                await Task.Run(() =>
                {
                    using (OracleConnection connection = _conexion.ObtenerConexion())
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("ObtenerConsumosGas", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdSitio", OracleDbType.Varchar2).Value = user.IdSitio;
                            command.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                            using (OracleDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    respuesta.Detalle = new ConsumoGas()
                                    {
                                        ConsumoDelMes = decimal.Parse(reader["ConsumoDelMes"].ToString()),
                                        DiasMes = int.Parse(reader["DiasMes"].ToString()),
                                        //Fecha = reader["Fecha"].ToString(),
                                        //IdOperario = reader["IdOperario"].ToString(),
                                        IdConsumoGas = reader["IdConsumoGas"].ToString(),
                                        IdContratoGas = reader["IdContratoGas"].ToString()
                                    };
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }
        public async Task<Respuesta<DetalleConsumoGas>> ObtenerDetalleDiarioConsumoGas(string idConsumo)
        {
            Respuesta<DetalleConsumoGas> respuesta = new Respuesta<DetalleConsumoGas>();
            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();

                await Task.Run(() =>
                {
                    using (OracleConnection connection = _conexion.ObtenerConexion())
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("ObtenerDetalleConsumosGas", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdConsumoGas", OracleDbType.Varchar2).Value = idConsumo;
                            command.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                            using (OracleDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    respuesta.Detalle = new DetalleConsumoGas()
                                    {
                                        Fecha = reader["Fecha"].ToString(),
                                        IdConsumoGas = reader["IdConsumoGas"].ToString(),
                                        ConsumoDiario = decimal.Parse(reader["ConsumoDiario"].ToString()),
                                        ConsumoTotalActual = decimal.Parse(reader["ConsumoTotalActual"].ToString()),
                                        IdDetalleConsumoGas = reader["IdDetalleConsumoGas"].ToString()
                                    };
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }
    }
}
