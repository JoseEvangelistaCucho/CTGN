using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Generacion.Application.Usuario.Session.Command
{
    public class GuardarDatosDeSession
    {
        private readonly Function _function;
        private readonly IConexionBD _conexion;
        public GuardarDatosDeSession(IConexionBD conexionBD, Function function)
        {
            _function = function;
            _conexion = conexionBD;
        }


        public async Task<Respuesta<string>> GuardarSessionUsuario(string idTurno)
        {
            Respuesta<string> respuesta = null;
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("InsertOrUpdateHistorialSession", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agrega los parámetros con sus valores
                        command.Parameters.Add("p_IdHistorial", OracleDbType.Varchar2).Value = $"{datosOperario.IdSitio}_{datosOperario.Nombre.Substring(0,2).ToUpper()}{datosOperario.Apellidos.Substring(0,2).ToUpper()}-{DateTime.Now.ToString("ddMMyyyy")}";
                        command.Parameters.Add("p_IdOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;
                        command.Parameters.Add("p_NombreOperario", OracleDbType.Varchar2).Value = $"{datosOperario.Nombre} {datosOperario.Apellidos}";
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_IdTurno", OracleDbType.Varchar2).Value = idTurno;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = DateTime.Now.ToString("dd/MM/yyyy");

                        command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }


        public async Task<Respuesta<string>> GuardarDatosSession(DatosSession datos)
        {
            Respuesta<string> respuesta = null;
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("InsertarHistorialAccion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agrega los parámetros con sus valores
                        command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;
                        command.Parameters.Add("p_rutaAccion", OracleDbType.Varchar2).Value = datos.RutaAccion;
                        command.Parameters.Add("p_accion", OracleDbType.Varchar2).Value = datos.Accion;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_descripcion", OracleDbType.Varchar2).Value = datos.Descripcion;

                        command.ExecuteNonQuery();
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

    public class DatosSession
    {
        public string RutaAccion { get; set; }
        public string Accion { get; set; }
        public string Descripcion { get; set; }

    }

}
