using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.DatosConsola.Command
{
    public class DatosRegistroConsola : IDatosRegistroConsola
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosRegistroConsola(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }

        public async Task<Respuesta<string>> GuardarDatosEG(List<DatosFormatoConsola> datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    foreach (var datosRegistroConsola in datos)
                    {
                        using (OracleCommand command = new OracleCommand("proc_InsertarDetConsola", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdDetalleConsola", OracleDbType.Varchar2).Value = datosRegistroConsola.IdDetalleConsola;
                            command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datosRegistroConsola.Fecha;
                            command.Parameters.Add("p_Hora", OracleDbType.Varchar2).Value = datosRegistroConsola.Hora;
                            command.Parameters.Add("p_IdRegistroConsola", OracleDbType.Varchar2).Value = datosRegistroConsola.IdRegistroConsola;
                            command.Parameters.Add("p_p", OracleDbType.Decimal).Value = datosRegistroConsola.PotenciaActiva;
                            command.Parameters.Add("p_Q", OracleDbType.Decimal).Value = datosRegistroConsola.PotenciaReactiva;
                            command.Parameters.Add("p_E", OracleDbType.Decimal).Value = datosRegistroConsola.EnergiaActiva;
                            command.Parameters.Add("p_EQ", OracleDbType.Decimal).Value = datosRegistroConsola.EnergiaReactiva;
                            command.Parameters.Add("p_IL1", OracleDbType.Decimal).Value = datosRegistroConsola.CorrienteLinea1;
                            command.Parameters.Add("p_IL2", OracleDbType.Decimal).Value = datosRegistroConsola.CorrienteLinea2;
                            command.Parameters.Add("p_IL3", OracleDbType.Decimal).Value = datosRegistroConsola.CorrienteLinea3;
                            command.Parameters.Add("p_U12", OracleDbType.Decimal).Value = datosRegistroConsola.Voltaje;
                            command.Parameters.Add("p_U23", OracleDbType.Decimal).Value = datosRegistroConsola.Voltaje23;
                            command.Parameters.Add("p_U31", OracleDbType.Decimal).Value = datosRegistroConsola.Voltaje31;
                            command.Parameters.Add("p_IdOperario", OracleDbType.Varchar2).Value = datosRegistroConsola.IdOperario;
                            command.Parameters.Add("p_IdSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                            command.Parameters.Add("p_IdFormatoConsola", OracleDbType.Varchar2).Value = datosRegistroConsola.IdformatoConsola;

                            command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                            if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                            {
                                respuesta.Mensaje = "Ok";
                            }
                            else
                            {
                                respuesta.Mensaje = "No pudo consultar.";
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

        public async Task<Respuesta<string>> GuardarDatosGenerador(List<RegistroDatosGenerator> datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();
                    foreach (var datosConsola in datos)
                    {
                        using (OracleCommand command = new OracleCommand("proc_GuardarDatosGenerador", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdDetGeneratorConsola", OracleDbType.Varchar2).Value = datosConsola.IdDetGeneratorConsola;
                            command.Parameters.Add("p_Hora", OracleDbType.Varchar2).Value = datosConsola.Hora;
                            command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datosConsola.Fecha;
                            command.Parameters.Add("p_L1", OracleDbType.Decimal).Value = datosConsola.L1;
                            command.Parameters.Add("p_L2", OracleDbType.Decimal).Value = datosConsola.L2;
                            command.Parameters.Add("p_L3", OracleDbType.Decimal).Value = datosConsola.L3;
                            command.Parameters.Add("p_D", OracleDbType.Decimal).Value = datosConsola.D;
                            command.Parameters.Add("p_ND", OracleDbType.Decimal).Value = datosConsola.ND;
                            command.Parameters.Add("p_TersionalVibration", OracleDbType.Decimal).Value = datosConsola.TersionalVibration;
                            command.Parameters.Add("p_IdFormatoConsola", OracleDbType.Varchar2).Value = datosConsola.IdFormatoConsola;

                            command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                            if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                            {
                                respuesta = await GuardarDetallesGenerador(datosConsola.detalleGeneradores);
                            }
                            else
                            {
                                respuesta.Mensaje = "Error al consultar.";
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

        public async Task<Respuesta<string>> GuardarDetallesGenerador(List<DetGeneratorTC> detGenerators)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_GuardarDetalleGenerador", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        OracleParameter idDetGeneratorConsola = new OracleParameter("p_IdDetGeneratorConsola", OracleDbType.Varchar2);
                        OracleParameter fecha = new OracleParameter("p_fecha", OracleDbType.Varchar2);
                        OracleParameter idDetalleGenerador = new OracleParameter("p_IdDetGenerator", OracleDbType.Varchar2);
                        OracleParameter idTipoEngine = new OracleParameter("p_IdTipoEngine", OracleDbType.Varchar2);
                        OracleParameter tempOut = new OracleParameter("p_TempOut", OracleDbType.Decimal);
                        OracleParameter speed = new OracleParameter("p_Speed", OracleDbType.Decimal);

                        OracleParameter outputParameter = new OracleParameter("p_resultado", OracleDbType.Decimal);
                        outputParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.AddRange(new OracleParameter[] {
                            idDetGeneratorConsola,
                            fecha,
                            idDetalleGenerador,
                            idTipoEngine,
                            tempOut,
                            speed,
                            outputParameter
                        });
                        foreach (DetGeneratorTC item in detGenerators)
                        {
                            if (string.IsNullOrEmpty(item.IdDetGenerator))
                                continue;

                            idDetGeneratorConsola.Value = item.IdDetGeneratorConsola;
                            idDetalleGenerador.Value = item.IdDetGenerator;
                            idTipoEngine.Value = item.IdTipoEngine;
                            fecha.Value = item.Fecha;
                            tempOut.Value = item.TempOut;
                            speed.Value = item.Speed;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)outputParameter.Value;

                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;

                            if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
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

        public async Task<Respuesta<string>> GuardarDatosEngine(List<RegistroDetalleEngine> datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    foreach (var detalleEngine in datos)
                    {
                        using (OracleCommand command = new OracleCommand("proc_InsertarDatosEngine", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_IdDetEngineConsola", OracleDbType.Varchar2).Value = detalleEngine.IdDetEngineConsola;
                            command.Parameters.Add("p_Hora", OracleDbType.Varchar2).Value = detalleEngine.Hora;
                            command.Parameters.Add("p_RunHours", OracleDbType.Decimal).Value = detalleEngine.RunHours;
                            command.Parameters.Add("p_CaTemp", OracleDbType.Decimal).Value = detalleEngine.CATemp;
                            command.Parameters.Add("p_DiffPressJetPulse", OracleDbType.Decimal).Value = detalleEngine.DiffPressJetPulse;
                            command.Parameters.Add("p_CAPress", OracleDbType.Decimal).Value = detalleEngine.CAPress;
                            command.Parameters.Add("p_ExhGasAvgTemp", OracleDbType.Decimal).Value = detalleEngine.ExhGasAvgTemp;
                            command.Parameters.Add("p_CylPressAvg", OracleDbType.Decimal).Value = detalleEngine.CylPressAvg;
                            command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = detalleEngine.Fecha;
                            command.Parameters.Add("p_IdFormatoConsola", OracleDbType.Varchar2).Value = detalleEngine.IdFormatoConsola;
                            command.Parameters.Add("p_IdSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                            command.Parameters.Add("p_IdOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;

                            command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                            if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                            {
                                respuesta = await GuardarDetallesEngine(detalleEngine.detalleEngines);
                            }
                            else
                            {
                                respuesta.Mensaje = "Error al consultar.";
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

        public async Task<Respuesta<string>> GuardarDetallesEngine(List<DetalleEngine> detGenerators)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_InsertarDetEngine", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        OracleParameter idDetEngine = new OracleParameter("p_IdDetEngine", OracleDbType.Varchar2);
                        OracleParameter idTipoEngine = new OracleParameter("p_IdTipoEngine", OracleDbType.Varchar2);
                        OracleParameter fecha = new OracleParameter("p_Fecha", OracleDbType.Varchar2);
                        OracleParameter presion = new OracleParameter("p_Presion", OracleDbType.Decimal);
                        OracleParameter temp = new OracleParameter("p_Temp", OracleDbType.Decimal);
                        OracleParameter idDetEngineConsola = new OracleParameter("p_IdDetEngineConsola", OracleDbType.Varchar2);

                        OracleParameter outputParameter = new OracleParameter("p_resultado", OracleDbType.Decimal);
                        outputParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.AddRange(new OracleParameter[] {
                            idDetEngine,
                            idTipoEngine,
                            fecha,
                            presion,
                            temp,
                            idDetEngineConsola,
                            outputParameter
                        });
                        foreach (DetalleEngine item in detGenerators)
                        {
                            if (string.IsNullOrEmpty(item.IdDetEngine))
                                continue;

                            idDetEngine.Value = item.IdDetEngine;
                            idTipoEngine.Value = item.IdTipoEngine;
                            fecha.Value = item.Fecha;
                            presion.Value = item.Presion;
                            temp.Value = item.Temp;
                            idDetEngineConsola.Value = item.IdDetEngineConsola;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)outputParameter.Value;

                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;

                            if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
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

        public async Task<Respuesta<string>> GuardarLectMedianoche(LecturasMedianoche lecturasMedianoche)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_InsertarLectMediaN", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdLecturas", OracleDbType.Varchar2).Value = lecturasMedianoche.IdLecturas;
                        command.Parameters.Add("p_NumeroEG", OracleDbType.Decimal).Value = lecturasMedianoche.NumeroEG;
                        command.Parameters.Add("p_GasconsumedEG", OracleDbType.Decimal).Value = lecturasMedianoche.GasconsumedEG;
                        command.Parameters.Add("p_GasEnergiaEG", OracleDbType.Decimal).Value = lecturasMedianoche.GasEnergiaEG;
                        command.Parameters.Add("p_ReadingToday", OracleDbType.Decimal).Value = lecturasMedianoche.ReadingToday;
                        command.Parameters.Add("p_IdFormatoConsola", OracleDbType.Varchar2).Value = lecturasMedianoche.IdFormatoConsola;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = DateTime.Now.ToString("dd/MM/yyyy");

                        command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                        OracleParameter outputParameter = new OracleParameter("p_resultado", OracleDbType.Decimal);
                        outputParameter.Direction = System.Data.ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                        if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = "Ok";
                        }
                        else
                        {
                            respuesta.Mensaje = "No pudo consultar.";
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

        public async Task<Respuesta<string>> GuardarDetallesBAO(List<OutGoingFeeder> datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();
                    var datosOperario = await _function.ObtenerDatosOperario();
                    using (OracleCommand command = new OracleCommand("proc_InsertarOutGoingFeeder", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        OracleParameter idOutGoing = new OracleParameter("p_IdOutGoing", OracleDbType.Varchar2);
                        OracleParameter idTipoEngine = new OracleParameter("p_IdTipoEngine", OracleDbType.Varchar2);
                        OracleParameter kWh = new OracleParameter("p_kWh", OracleDbType.Decimal);
                        OracleParameter kVARh = new OracleParameter("p_kVARh", OracleDbType.Decimal);
                        OracleParameter hora = new OracleParameter("p_Hora", OracleDbType.Varchar2);
                        OracleParameter fecha = new OracleParameter("p_Fecha", OracleDbType.Varchar2); 
                        OracleParameter fila = new OracleParameter("p_Fila", OracleDbType.Decimal); 
                        OracleParameter sitio = new OracleParameter("p_Sitio", OracleDbType.Varchar2); 
                        OracleParameter idFormatoConsola = new OracleParameter("p_IdFormatoConsola", OracleDbType.Varchar2);

                        OracleParameter outputParameter = new OracleParameter("p_resultado", OracleDbType.Decimal);
                        outputParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.AddRange(new OracleParameter[] {
                            idOutGoing,
                            idTipoEngine,
                            kWh,
                            kVARh,
                            hora,
                            fecha,
                            fila,
                            sitio,
                            idFormatoConsola,
                            outputParameter
                        });
                        foreach (OutGoingFeeder item in datos)
                        {
                            if (string.IsNullOrEmpty(item.IdOutGoing))
                                continue;

                            idOutGoing.Value = item.IdOutGoing;
                            idTipoEngine.Value = item.IdTipoEngine;
                            kWh.Value = item.KWh;
                            kVARh.Value = item.KVARh;
                            hora.Value = item.Hora;
                            fecha.Value = item.Fecha;
                            fila.Value = item.Fila; 
                            sitio.Value = datosOperario.IdSitio;
                            idFormatoConsola.Value = item.IdFormatoConsola;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)outputParameter.Value;

                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;

                            if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1) 
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

        public async Task<Respuesta<string>> GuardarDatoFormato(FormatoConsola datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_InsFormatoConsola", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_IdFormatoConsola", OracleDbType.Varchar2).Value = datos.IdFormatoConsola;
                        command.Parameters.Add("p_Observaciones", OracleDbType.Varchar2).Value = datos.Observaciones;
                        command.Parameters.Add("p_Observaciones1", OracleDbType.Varchar2).Value = string.Empty;
                        command.Parameters.Add("p_Observaciones2", OracleDbType.Varchar2).Value = string.Empty;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datos.Fecha;
                        command.Parameters.Add("p_IdSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_IdOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;

                        command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                        if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = "Ok";
                        }
                        else
                        {
                            respuesta.Mensaje = "No pudo consultar.";
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
