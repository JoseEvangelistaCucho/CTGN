using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.TurboCompresor;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Generacion.Application.DashBoard.LavadoTurbo.Query
{
    public class DatosLavadoTurbo
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosLavadoTurbo(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }

        public async Task<Respuesta<DatoLavadoTurbo>> ObtenerDatosLavadoTurbo()
        {
            Respuesta<DatoLavadoTurbo> respuesta = new Respuesta<DatoLavadoTurbo>();

            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ConsultarLavadosTurbo", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        cmd.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();

                        while (reader.Read())
                        {
                            respuesta.Detalle = new DatoLavadoTurbo()
                            {
                                IdLavadoTurbo = reader["IdLavadoTurbo"].ToString(),
                                Fecha = reader["Fecha"].ToString()
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


        public async Task<Respuesta<List<DetalleLavadoTurbo>>> ObtenerDetalleLavadoTurbo(string id)
        {
            Respuesta<List<DetalleLavadoTurbo>> respuesta = new Respuesta<List<DetalleLavadoTurbo>>();

            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ConsultarDetalleLavadoTurbo", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_IdLavadoTurbo", OracleDbType.Varchar2).Value = id;
                        cmd.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();
                        respuesta.Detalle = new List<DetalleLavadoTurbo>();

                        while (reader.Read())
                        {

                            var dato= new DetalleLavadoTurbo()
                            {
                                IdDetalleLavadoTurbo = reader["IdDetalleLavadoTurbo"].ToString(),
                                Tipo = reader["Tipo"].ToString(),
                                Valor = int.Parse(reader["Valor"].ToString()),
                                NumeroGenerador = int.Parse(reader["NumeroGenerador"].ToString()),
                                Fecha = reader["Fecha"].ToString(),
                                IdLavadoTurbo = reader["IdLavadoTurbo"].ToString()
                            };
                            respuesta.Detalle.Add(dato);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
            }

            return respuesta;
        }
    }
}
