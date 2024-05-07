using Generacion.Application.DataBase;
using Generacion.Models;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Generacion.Application.Usuario.Command
{
    public class DatosUsuario : IUsuario
    {
        private readonly IConexionBD _conexion;
        public DatosUsuario(IConexionBD conexion)
        {
            _conexion = conexion;
        }
        public async Task<Respuesta<List<HistorialUsuario>>> GuardarHistorial(List<HistorialUsuario> historialUsuario,string idOperario)
        {
            Respuesta<List<HistorialUsuario>> respuesta = new Respuesta<List<HistorialUsuario>>();

            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("PROC_InsertarHistorial", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        OracleParameter idHistorialOperario = new OracleParameter("p_idHistorialOperario", OracleDbType.Varchar2);
                        OracleParameter inputFecha = new OracleParameter("p_fecha", OracleDbType.Varchar2);
                        OracleParameter inputHora = new OracleParameter("p_hora", OracleDbType.Varchar2);
                        OracleParameter idUsuario = new OracleParameter("p_idusuario", OracleDbType.Varchar2);
                        OracleParameter inputDescripcion = new OracleParameter("p_descripcion", OracleDbType.Varchar2);

                        OracleParameter outputParameter = new OracleParameter("p_resultado", OracleDbType.Decimal);
                        outputParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.AddRange(new OracleParameter[] {
                            idHistorialOperario,
                            inputFecha,
                            inputHora,
                            inputDescripcion,
                            idUsuario,
                            outputParameter
                        });
                        foreach (HistorialUsuario item in historialUsuario)
                        {
                            if (string.IsNullOrEmpty(item.IdHistorialOperario))
                                continue;

                            idHistorialOperario.Value = item.IdHistorialOperario;
                            inputFecha.Value = item.Fecha.ToString();
                            inputHora.Value = item.Hora;
                            idUsuario.Value = idOperario;
                            inputDescripcion.Value = item.Descripcion;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)outputParameter.Value;

                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;

                            if (respuesta.IdRespuesta == 0)
                            {
                                respuesta.Mensaje = "Ok";
                            }
                            else
                            {
                                respuesta.Mensaje = "Error al Guardar ";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }

        public async Task<Respuesta<DetalleOperario>> ObtenerDatosOperario(string usuarioRed)
        {
            Respuesta<DetalleOperario> respuesta = new Respuesta<DetalleOperario>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"select op.idOperario,op.idCargo,car.cargo,op.idsitio,
                                        usu.nombre,usu.apellidos,op.idturno 
                                        from tbl_Operario op 
                                        inner join tbl_usuario usu on usu.numeroDocumento = op.numeroDocumento 
                                        inner join tbl_cargo car on car.idCargo = op.idCargo 
                                        where usu.usuariored = :p_usuarioRed";

                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {
                        command.Parameters.Add(new OracleParameter("p_usuarioRed", usuarioRed));
                        
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Obtener los valores de salida
                                DetalleOperario detalleOperario = new DetalleOperario();
                                detalleOperario.IdOperario = reader["idOperario"].ToString();
                                detalleOperario.IdCargo = reader["idCargo"].ToString();
                                detalleOperario.DescripcionCargo = reader["cargo"].ToString();
                                detalleOperario.Nombre = reader["nombre"].ToString();
                                detalleOperario.Apellidos = reader["apellidos"].ToString();
                                detalleOperario.IdSitio = reader["idsitio"].ToString();
                                //detalleOperario.IdTurno = reader["idturno"].ToString();

                                respuesta.Detalle = new DetalleOperario();
                                respuesta.Detalle = detalleOperario;
                            }

                            respuesta.IdRespuesta = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();

            }

            return respuesta;
        }
    }
}
