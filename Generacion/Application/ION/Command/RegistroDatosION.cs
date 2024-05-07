using Generacion.Application.DataBase;
using Generacion.Models;
using Generacion.Models.ION;
using Generacion.Models.MGD;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Linq;

namespace Generacion.Application.ION.Command
{
    public class RegistroDatosION : IDatosION
    {
        private readonly IConexionBD _conexion;
        public RegistroDatosION(IConexionBD conexion)
        {
            _conexion = conexion;
        }
        public async Task<Respuesta<DatosFormatoMGD>> GuardarDatosION(DatosMGD datos)
        {
            Respuesta<DatosFormatoMGD> respuesta = new Respuesta<DatosFormatoMGD>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "proc_GuardarFormatoIon";

                        // Definir parámetros de entrada
                        var paramFecha = command.Parameters.Add("p_DATE_TIME", OracleDbType.TimeStamp);
                        var paramHoras = command.Parameters.Add("p_HORAS", OracleDbType.Varchar2);
                        var paramKwhDelInt = command.Parameters.Add("p_KWH_DEL_INT", OracleDbType.Decimal);
                        var paramKvarhDelInt = command.Parameters.Add("p_KVARH_DEL_INT", OracleDbType.Decimal);
                        var paramKwhRecInt = command.Parameters.Add("p_KWH_REC_INT", OracleDbType.Decimal);
                        var paramKvarhRecInt = command.Parameters.Add("p_KVARH_REC_INT", OracleDbType.Decimal);
                        var paramVllAvg = command.Parameters.Add("p_VLL_AVG", OracleDbType.Decimal);
                        var paramFreq = command.Parameters.Add("p_FREQ", OracleDbType.Decimal);
                        var paramKwDelInt = command.Parameters.Add("p_KW_DEL_INT", OracleDbType.Decimal);
                        var paramKvarDelInt = command.Parameters.Add("p_KVAR_DEL_INT", OracleDbType.Decimal);
                        var paramKwRecInt = command.Parameters.Add("p_KW_REC_INT", OracleDbType.Decimal);
                        var paramKvarRecInt = command.Parameters.Add("p_KVAR_REC_INT", OracleDbType.Decimal);
                        var paramIdSitio = command.Parameters.Add("p_IDSITIO", OracleDbType.Varchar2);
                        var paramIdOperario = command.Parameters.Add("p_IDOPERARIO", OracleDbType.Varchar2);

                        // Definir parámetro de salida
                        var outputParameter = command.Parameters.Add("p_resultado", OracleDbType.Decimal);
                        outputParameter.Direction = ParameterDirection.Output;
                        OracleDecimal oracleDecimalValue;
                        foreach (DatosFormatoMGD formatoIon in datos.Revenue)
                        {
                            DateTime fechaHora = DateTime.Parse(formatoIon.Date_Time);

                            string fecha = fechaHora.ToShortDateString();
                            string hora = fechaHora.ToString("HH:mm");

                            paramFecha.Value = formatoIon.Date_Time;
                            paramHoras.Value = hora;
                            paramKwhDelInt.Value = formatoIon.kWhDelInt;
                            paramKvarhDelInt.Value = formatoIon.kVARhDelInt;
                            paramKwhRecInt.Value = formatoIon.kWhRecInt;
                            paramKvarhRecInt.Value = formatoIon.kVARhRecInt;
                            paramVllAvg.Value = formatoIon.VllAvg;
                            paramFreq.Value = formatoIon.Freq;
                            paramKwDelInt.Value = formatoIon.KWDelInt;
                            paramKvarDelInt.Value = formatoIon.KVARDelInt;
                            paramKwRecInt.Value = formatoIon.KWRecInt;
                            paramKvarRecInt.Value = formatoIon.KVARRecInt;
                            paramIdSitio.Value = datos.Idsitio;
                            paramIdOperario.Value = datos.IdOperario;

                            command.ExecuteNonQuery();

                            oracleDecimalValue = (OracleDecimal)outputParameter.Value;
                        }


                        respuesta.IdRespuesta = (int)outputParameter.Value;

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
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }
    }
}
