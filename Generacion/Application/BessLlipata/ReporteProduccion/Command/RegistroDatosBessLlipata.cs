using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Bess;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.BessLlipata.ReporteProduccion.Command
{
    public class RegistroDatosBessLlipata : IReporteBessLlipata
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistroDatosBessLlipata(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }
        public async Task<Respuesta<string>> GuardarDatosReporte(DBBessLlipata datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("InsertOrUpdateBessLlipata", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Configura los parámetros de entrada
                        cmd.Parameters.Add("p_idBessLlipata", OracleDbType.Varchar2).Value = datos.IdBessLlipata;
                        cmd.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        cmd.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = datos.Fecha;
                        cmd.Parameters.Add("p_TiempoDescarga", OracleDbType.Decimal).Value = datos.TiempoDescarga;
                        cmd.Parameters.Add("p_EnergiaDescarga", OracleDbType.Decimal).Value = datos.EnergiaDescarga;
                        cmd.Parameters.Add("p_RatioDescargaHora", OracleDbType.Decimal).Value = datos.RatioDescargaHora;
                        cmd.Parameters.Add("p_HoraDescargaDelSistema", OracleDbType.Varchar2).Value = datos.HoraDescargaDelSistema;
                        cmd.Parameters.Add("p_ContadorDescargasMes", OracleDbType.Decimal).Value = datos.ContadorDescargasMes;
                        cmd.Parameters.Add("p_TiempoRecarga", OracleDbType.Decimal).Value = datos.TiempoRecarga;
                        cmd.Parameters.Add("p_EnergiaCargada", OracleDbType.Decimal).Value = datos.EnergiaCargada;
                        cmd.Parameters.Add("p_RatioCargaHora", OracleDbType.Decimal).Value = datos.RatioCargaHora;
                        cmd.Parameters.Add("p_HoraCargaSistema", OracleDbType.Varchar2).Value = datos.HoraCargaSistema;
                        cmd.Parameters.Add("p_ContadorRecargasMes", OracleDbType.Decimal).Value = datos.ContadorRecargasMes;
                        cmd.Parameters.Add("p_RecortePotenciaMarconaFP", OracleDbType.Varchar2).Value = datos.RecortePotenciaMarconaFP;
                        cmd.Parameters.Add("p_TiempoDescargaFP", OracleDbType.Varchar2).Value = datos.TiempoDescargaFP;
                        cmd.Parameters.Add("p_HoraCargaFP", OracleDbType.Varchar2).Value = datos.HoraCargaFP;
                        cmd.Parameters.Add("p_BessLlipata", OracleDbType.Varchar2).Value = datos.BessLlipataValue;
                        cmd.Parameters.Add("p_nota", OracleDbType.Varchar2).Value = datos.Nota;
                        cmd.Parameters.Add("p_observacion", OracleDbType.Varchar2).Value = datos.Observacion;
                        cmd.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;

                        cmd.Parameters.Add("p_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)cmd.Parameters["p_Resultado"].Value;
                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                        if (respuesta.IdRespuesta == 0)
                        {
                            respuesta.Mensaje = "Insertado correctamente";
                        }
                        else if (respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = "Actualizado correctamente";
                        }
                        else if (respuesta.IdRespuesta == 99)
                        {
                            respuesta.Mensaje = "Error al ejecutar el procedimiento almacenado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al ejecutar el procedimiento almacenado";
            }
            return respuesta;
        }
    }
}
