using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Bess;
using Generacion.Models.DataCoes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.ReportesGenerales.DataBess.Command
{
    public class RegistroDataBess : IRegistroDataBess
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistroDataBess(IConexionBD conexion, Function funtion)
        {
            _conexion = conexion;
            _function = funtion;
        }
        public async Task<Respuesta<string>> GuardarDatosArchivosText(DBDataBess bess)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    // Crear y configurar el comando
                    using (OracleCommand command = new OracleCommand("InsertOrUpdateDataBess", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_idDataBess", OracleDbType.Varchar2).Value = bess.IdDataBess;
                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_fecha", OracleDbType.TimeStamp).Value = bess.Fecha;
                        command.Parameters.Add("p_Hora", OracleDbType.Varchar2).Value = bess.Hora;
                        command.Parameters.Add("p_LLIXI625_PV", OracleDbType.Decimal).Value = bess.LLIXI625_PV;
                        command.Parameters.Add("p_LLIXI622_PV", OracleDbType.Decimal).Value = bess.LLIXI622_PV;
                        command.Parameters.Add("p_LLIXI633_PV", OracleDbType.Decimal).Value = bess.LLIXI633_PV;
                        command.Parameters.Add("p_LLIXI634_PV", OracleDbType.Decimal).Value = bess.LLIXI634_PV;
                        command.Parameters.Add("p_LLIXI634_PV2", OracleDbType.Decimal).Value = bess.LLIXI634_PV2;
                        command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;

                        command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDecimal outputParameter = (OracleDecimal)command.Parameters["p_resultado"].Value;
                        respuesta.IdRespuesta = (int)outputParameter.Value;

                        if (respuesta.IdRespuesta == 0)
                        {
                            respuesta.Mensaje = "Inserción exitosa.";
                        }
                        else if (respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = "Actualización exitosa.";
                        }
                        else if (respuesta.IdRespuesta == 99)
                        {
                            respuesta.Mensaje = "Error en el procedimiento.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la solicitud.";
            }
            return respuesta;
        }
    }
}
