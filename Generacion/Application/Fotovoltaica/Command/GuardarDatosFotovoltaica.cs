using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.FotoVoltaica;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.Fotovoltaica.Command
{
    public class GuardarDatosFotovoltaica : IGuardarDatosFotovoltaica
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public GuardarDatosFotovoltaica(IConexionBD conexion, Function funtion)
        {
            _conexion = conexion;
            _function = funtion;
        }

        public async Task<Respuesta<string>> GuardarStatusGenerados(DetalleFotovoltaica detalle)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("InsertarStatusFotovoltaica", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        
                        command.Parameters.Add("p_IdDetalleGenerado", OracleDbType.Varchar2).Value = detalle.IdDetalleGenerado;
                        command.Parameters.Add("p_planta", OracleDbType.Decimal).Value = detalle.Planta;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = detalle.Fecha;
                        command.Parameters.Add("p_IdReporteFotoVol", OracleDbType.Varchar2).Value = detalle.IdReporteFotoVol;
                        command.Parameters.Add("p_IdTipoEngine", OracleDbType.Varchar2).Value = detalle.IdTipoEngine;

                        command.Parameters.Add("p_Resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDecimal outputParameter = (OracleDecimal)command.Parameters["p_resultado"].Value;
                        respuesta.IdRespuesta = (int)outputParameter.Value;

                        if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = "Inserción exitosa.";
                        }
                        else 
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

        public async Task<Respuesta<string>> GuardarDatosReporte(ReporteFotovoltaica reporte)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("InsertarReporteFotovoltaica", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdReporteFotoVol", OracleDbType.Varchar2).Value = reporte.IdReporteFotoVol;
                        /*command.Parameters.Add("p_EstandarCarbono", OracleDbType.Decimal).Value = reporte.EstandarCarbono;
                        command.Parameters.Add("p_ReduccionCo2", OracleDbType.Decimal).Value = reporte.ReduccionCo2;
                        command.Parameters.Add("p_PlantacionArboles", OracleDbType.Decimal).Value = reporte.PlantacionArboles;*/
                        command.Parameters.Add("p_ImagenReporte", OracleDbType.Varchar2).Value = reporte.ImagenReporte;
                        command.Parameters.Add("p_Manager", OracleDbType.Varchar2).Value = reporte.Manager;
                        command.Parameters.Add("p_Safety", OracleDbType.Varchar2).Value = reporte.Safety;
                        command.Parameters.Add("p_HumanResource", OracleDbType.Varchar2).Value = reporte.HumanResources;
                        command.Parameters.Add("p_Nota", OracleDbType.Varchar2).Value = reporte.Nota;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = reporte.Fecha;
                        command.Parameters.Add("p_IdSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_IdOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;

                        command.Parameters.Add("p_Resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

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
                respuesta.Mensaje = "Error en el procedimiento.";
            }

            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarDetalleReporte(FotovoltaicaGenerada fotovoltaica)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("InsertarDetalleFotovoltaica", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        
                        command.Parameters.Add("p_IdVoltajeGenerado", OracleDbType.Varchar2).Value = fotovoltaica.IdVoltajeGenerado;
                        command.Parameters.Add("p_Detalle", OracleDbType.Varchar2).Value = fotovoltaica.Detalle;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = fotovoltaica.Fecha;
                        command.Parameters.Add("p_EstandarCarbono", OracleDbType.Decimal).Value = fotovoltaica.EstandarCarbono;
                        command.Parameters.Add("p_ReduccionCo2", OracleDbType.Decimal).Value = fotovoltaica.ReduccionCo2;
                        command.Parameters.Add("p_PlantacionArboles", OracleDbType.Decimal).Value = fotovoltaica.PlantacionArboles;
                        command.Parameters.Add("p_NumeroMes", OracleDbType.Int32).Value = fotovoltaica.NumeroMes;
                        command.Parameters.Add("p_IdReporteFotoVol", OracleDbType.Varchar2).Value = fotovoltaica.IdReporteFotoVol;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

                        command.Parameters.Add("p_Resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDecimal outputParameter = (OracleDecimal)command.Parameters["p_resultado"].Value;
                        respuesta.IdRespuesta = (int)outputParameter.Value;

                        if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = "Inserción exitosa.";
                        }
                        else 
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
