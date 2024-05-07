using Generacion.Application.DataBase;
using Generacion.Application.DatosConsola.Command;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using Generacion.Models.ION;
using Generacion.Models.MGD;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.MGD.Command
{
    public class RegistroDatosMGD : IDatosMGD
    {
        private readonly IConexionBD _conexion; 
        public RegistroDatosMGD(IConexionBD conexion)
        {
            _conexion = conexion;
        }
        public async Task<Respuesta<string>> GuardarDatosConsultaMGD(DatosMGD datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_InsertarDatosMGD", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        OracleParameter idReporte = new OracleParameter("p_IdReporteMGD", OracleDbType.Varchar2);
                        OracleParameter idsitio = new OracleParameter("p_idsitio", OracleDbType.Varchar2);
                        OracleParameter fecha = new OracleParameter("p_fecha", OracleDbType.Varchar2);
                        OracleParameter PotenciaActivaMW = new OracleParameter("p_Potencia_Activa_MW", OracleDbType.Decimal);
                        OracleParameter PotenciaReactivaMVAR = new OracleParameter("p_Potencia_Reactiva_MVAR", OracleDbType.Decimal);
                        OracleParameter ptensionKV = new OracleParameter("p_tension_kV", OracleDbType.Decimal);
                        OracleParameter frecuenciaHz = new OracleParameter("p_Frecuencia_hz", OracleDbType.Decimal);
                        OracleParameter IdOperario = new OracleParameter("p_idOperario", OracleDbType.Varchar2);
                        
                        OracleParameter outputParameter = new OracleParameter("p_resultado", OracleDbType.Decimal);
                        outputParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.AddRange(new OracleParameter[] {
                            idReporte,
                            idsitio,
                            fecha,
                            PotenciaActivaMW,
                            PotenciaReactivaMVAR,
                            ptensionKV,
                            frecuenciaHz,
                            IdOperario,
                            outputParameter
                        });
                        int i = 0;
                        foreach (DatosFormatoMGD item in datos.Revenue)
                        {
                            if (string.IsNullOrEmpty(datos.IdReporteMGD))
                                continue;
                            i++;

                            idReporte.Value = datos.IdReporteMGD+i.ToString();
                            idsitio.Value = datos.Idsitio;
                            fecha.Value = item.Date_Time;
                            PotenciaActivaMW.Value = item.KWDelInt / 1000;
                            PotenciaReactivaMVAR.Value = item.KVARDelInt / 1000;
                            ptensionKV.Value = item.VllAvg / 1000;
                            frecuenciaHz.Value = item.Freq / 1000;
                            IdOperario.Value = datos.IdOperario;

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
    }
}
