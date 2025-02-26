using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Almacen.Bujias;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.Bujias.Query
{
    public class ConsultaBujias
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        private readonly Logger _logger;
        public ConsultaBujias(IConexionBD conexion, Function function, Logger logger)
        {
            _function = function;
            _conexion = conexion;
            _logger = logger;
        }

        public async Task<Respuesta<List<RegistroBujias>>> ObtenerdetalleControlCambio(string lado, int fila, int GE, string sitio)
        {
            Respuesta<List<RegistroBujias>> respuesta = new Respuesta<List<RegistroBujias>>();
            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                try
                {
                    connection.Open();

                    OracleCommand cmd = new OracleCommand("GetDetalleControlCambio", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_Lado", OracleDbType.Varchar2).Value = lado;
                    cmd.Parameters.Add("p_Fila", OracleDbType.Int32).Value = fila;
                    cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = sitio;
                    cmd.Parameters.Add("p_NumeroGE", OracleDbType.Int32).Value = GE;

                    cmd.Parameters.Add("p_item", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_Cursor"].Value).GetDataReader();

                    Dictionary<string, List<RegistroBujias>> registros = new Dictionary<string, List<RegistroBujias>>();
                    List<RegistroBujias> listaRegistro = new List<RegistroBujias>();
                    RegistroBujias registro = new RegistroBujias();
                    while (reader.Read())
                    {
                        registro = new RegistroBujias()
                        {
                            IdDetalleControlCambio = reader["IdDetalleControlCambio"].ToString(),
                            Detalle = int.Parse(reader["area_nombre"].ToString()),
                            Fecha = reader["Fecha"].ToString(),
                            Lado = reader["Lado"].ToString(),
                            IdSubtituloCampo = reader["IdSubtituloCampo"].ToString(),
                            Fila = int.Parse(reader["Fila"].ToString()),
                            Item = int.Parse(reader["Item"].ToString()),
                            IdControlCambio = reader["IdControlCambio"].ToString(),
                            Nota = reader["nota"].ToString()
                        };
                        listaRegistro.Add(registro);
                    }
                    respuesta.Detalle = new List<RegistroBujias>();
                    respuesta.Detalle = listaRegistro;

                    OracleDecimal oracleDecimalValue = (OracleDecimal)cmd.Parameters["p_item"].Value;
                    respuesta.Mensaje = oracleDecimalValue.Value.ToString();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error ObtenerdetalleControlCambio : " + ex.Message.ToString());
                }
            }
            return respuesta;
        }
        
        public async Task<Respuesta<List<DetalleRegistroBujias>>> ObtenerControlCambio(string sitio)
        {
            Respuesta<List<DetalleRegistroBujias>> respuesta = new Respuesta<List<DetalleRegistroBujias>>();
            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                try
                {
                    connection.Open();

                    OracleCommand cmd = new OracleCommand("GETCONTROLCAMBIO", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = sitio;
                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_Cursor"].Value).GetDataReader();

                    respuesta.IdRespuesta = 0;
                    respuesta.Detalle = new List<DetalleRegistroBujias>();
                    DetalleRegistroBujias registro = new DetalleRegistroBujias();
                    while (reader.Read())
                    {
                        registro = new DetalleRegistroBujias()
                        {
                            Horasoperacion = int.Parse(reader["horasoperacion"].ToString()),
                            Numerogenerador = int.Parse(reader["numerogenerador"].ToString()),
                            Bujiasgastadas = int.Parse(reader["bujiasgastadas"].ToString()),
                        };
                        respuesta.Detalle.Add(registro);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error ObtenerControlCambio : " + ex.Message.ToString());
                }
            }
            return respuesta;
        }

        public async Task<Respuesta<RegistroBujias>> ObtenerRegistrosPorGE(int numeroGenerador)
        {
            Respuesta<RegistroBujias> respuesta = new Respuesta<RegistroBujias>();
            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                try
                {
                    connection.Open();

                    OracleCommand cmd = new OracleCommand("GetControlCambioPorGE", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                    cmd.Parameters.Add("p_NumeroGE", OracleDbType.Int32).Value = numeroGenerador;

                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_Cursor"].Value).GetDataReader();

                    while (reader.Read())
                    {
                        respuesta.Detalle = new RegistroBujias()
                        {
                            Detalle = int.Parse(reader["Detalle"].ToString()),
                            Fecha = DateTime.Parse(reader["Fecha"].ToString()).ToString("dd/MM/yyyy"),
                            Lado = reader["Lado"].ToString(),
                            Fila
                            = int.Parse(reader["Fila"].ToString()),
                            Item = int.Parse(reader["Item"].ToString()),
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error ObtenerRegistrosPorGE : " + ex.Message.ToString());
                }
            }
            return respuesta;
        }


        public async Task<Respuesta<int>> ObtenerTotalBujiaUsado(int numeroGe)
        {
            Respuesta<int> respuesta = new Respuesta<int>();
            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                try
                {
                    connection.Open();

                    OracleCommand cmd = new OracleCommand("GetDetalleDeBujiasUso", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                    cmd.Parameters.Add("p_NumeroGE", OracleDbType.Int32).Value = numeroGe;

                    cmd.Parameters.Add("p_respuesta", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    respuesta.Detalle = new int();

                    OracleDecimal oracleDecimalValue = (OracleDecimal)cmd.Parameters["p_respuesta"].Value;
                    respuesta.Detalle = (int)oracleDecimalValue.Value;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error ObtenerTotalBujiaUsado : " + ex.Message.ToString());
                }
            }
            return respuesta;
        }
    }
}
