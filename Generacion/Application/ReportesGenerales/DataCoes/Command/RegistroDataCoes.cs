using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.DataCoes;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.ReportesGenerales.DataCoes.Command
{
    public class RegistroDataCoes : IRegistroDataCoes
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistroDataCoes(IConexionBD conexion, Function funtion)
        {
            _conexion = conexion;
            _function = funtion;
        }
        public async Task<Respuesta<string>> GuardarDatosExcelReporte(Demanda demanda)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    // Crear y configurar el comando
                    using (OracleCommand command = new OracleCommand("InsertOrUpdateDataCoes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_idDataCoes", OracleDbType.Varchar2).Value =$"{datosOperario.IdSitio}Data-Coes{demanda.Fecha}";
                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = demanda.Hora;
                        command.Parameters.Add("p_fecha", OracleDbType.TimeStamp).Value = demanda.Fecha;
                        command.Parameters.Add("p_ejecutado", OracleDbType.Decimal).Value = demanda.Ejecutado;
                        command.Parameters.Add("p_programacionDiaria", OracleDbType.Decimal).Value = demanda.ProgramacionDiaria;
                        command.Parameters.Add("p_programacionSemanal", OracleDbType.Decimal).Value = demanda.ProgramacionSemanal;
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

            }
            return respuesta;
        }
    }
}
