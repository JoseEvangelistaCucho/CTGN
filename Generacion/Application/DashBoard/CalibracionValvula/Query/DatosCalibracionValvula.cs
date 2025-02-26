using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.CalibracionValvula;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Generacion.Application.DashBoard.CalibracionValvula.Query
{
    public class DatosCalibracionValvula
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        private readonly Logger _logger;
        public DatosCalibracionValvula(IConexionBD conexion, Function function, Logger logger)
        {
            _function = function;
            _conexion = conexion;
             _logger = logger;
        }

        public async Task<Respuesta<DetalleCalibracionValvula>> ObtenerDetalleCalibracionPorNumeroGE(int numeroGenerador)
        {
            Respuesta<DetalleCalibracionValvula> respuesta = new Respuesta<DetalleCalibracionValvula>();

            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ConsultarCalibracionPorGE", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        cmd.Parameters.Add("p_NumeroGenerador", OracleDbType.Int32).Value = numeroGenerador;
                        cmd.Parameters.Add("p_mensaje", OracleDbType.Varchar2).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();

                        while (reader.Read())
                        {
                            respuesta.Detalle = new DetalleCalibracionValvula()
                            {
                                IdDetalleCalibracionMotor = reader["IdDetalleCalibracionMotor"].ToString(),
                                valor = int.Parse(reader["Valor"].ToString()),
                                NumeroGenerador = int.Parse(reader["NumeroGenerador"].ToString()),
                                Fecha = reader["Fecha"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error ObtenerDetalleCalibracionPorNumeroGE : " + ex.Message.ToString());
            }
            return respuesta;
        }
    }
}
