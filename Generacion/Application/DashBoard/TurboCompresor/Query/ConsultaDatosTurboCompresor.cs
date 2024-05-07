using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.DashBoard;
using Generacion.Models.TurboCompresor;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Generacion.Application.DashBoard.TurboCompresor.Query
{
    public class ConsultaDatosTurboCompresor
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public ConsultaDatosTurboCompresor(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }

        public async Task<Respuesta<List<DetalleTurboCompresor>>> ObtenerDetalleTurboCompresor(string id)
        {
            Respuesta<List<DetalleTurboCompresor>> respuesta = new Respuesta<List<DetalleTurboCompresor>>();

            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ObtenerDetalleTurboCompresor", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_IdTurboCompresor", OracleDbType.Varchar2).Value = id;
                        cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        cmd.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;
                        
                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();

                        respuesta.Detalle = new List<DetalleTurboCompresor>();
                        DetalleTurboCompresor dashboardDetalleFiltro = new DetalleTurboCompresor();
                        while (reader.Read())
                        {
                            dashboardDetalleFiltro = new DetalleTurboCompresor()
                            {
                                IdDetalleTurboCompresor = reader["IdDetalleTurboCompresor"].ToString(),
                                Posicion = int.Parse(reader["Posicion"].ToString()),
                                Lado = reader["Lado"].ToString(),
                                Tipo = reader["Tipo"].ToString(),
                                NumeroGenerador = int.Parse(reader["NumeroGenerador"].ToString()),
                                Valor = decimal.Parse(reader["Valor"].ToString()),
                                Fecha = reader["Fecha"].ToString(),
                                IdTurboCompresor = reader["IdTurboCompresor"].ToString()
                            };

                            respuesta.Detalle.Add(dashboardDetalleFiltro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }


            return respuesta;
        }


        public async Task<Respuesta<DatosTurboCompresor>> ObtenerDatosTurboCompresor()
        {
            Respuesta<DatosTurboCompresor> respuesta = new Respuesta<DatosTurboCompresor>();

            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ObtenerDatosTurboCompresor", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        cmd.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();
                        
                        while (reader.Read())
                        {
                            respuesta.Detalle = new DatosTurboCompresor()
                            {
                                Fecha = reader["Fecha"].ToString(),
                                IdTurboCompresor = reader["IdTurboCompresor"].ToString()
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

    }
}
