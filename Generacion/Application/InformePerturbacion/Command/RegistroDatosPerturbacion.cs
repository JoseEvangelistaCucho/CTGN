using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.InformePerturbacion;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Generacion.Application.InformePerturbacion.Command
{
    public class RegistroDatosPerturbacion : IRegistroDatosPerturbacion
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistroDatosPerturbacion(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }
        public async Task<Respuesta<string>> GuardarDatosPrincipal(InformeGeneralPerturbacion datosFormatoCampo)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    OracleCommand command = new OracleCommand("GuardarInformePerturbacion", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.Add("p_idReportePerturbacion", OracleDbType.Varchar2).Value = datosFormatoCampo.IdReportePerturbacion;
                    command.Parameters.Add("p_evento", OracleDbType.Varchar2).Value = datosFormatoCampo.Evento;
                    command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = datosFormatoCampo.Fecha;
                    command.Parameters.Add("p_hora", OracleDbType.Varchar2).Value = datosFormatoCampo.Hora;
                    command.Parameters.Add("p_componente", OracleDbType.Varchar2).Value = datosFormatoCampo.Componente;
                    command.Parameters.Add("p_propietario", OracleDbType.Varchar2).Value = datosFormatoCampo.Propietario;
                    command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;
                    command.Parameters.Add("p_IdSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

                    command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = System.Data.ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                    respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                    if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                    {
                        respuesta.Mensaje = "Ok";
                    }
                    else
                    {
                        respuesta.Mensaje = "Error al guardar los datos.";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la petición.";
            }

            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarDetallesPrincipal(DetalleInformePerturbacion detallePerturbacion, string rutaImagenes)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    OracleCommand command = new OracleCommand("GuardarDetallePerturbacion", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.Add("p_idDetallePerturbacion", OracleDbType.Varchar2).Value = detallePerturbacion.IdDetallePerturbacion;
                    command.Parameters.Add("p_DescripcionEvento", OracleDbType.Varchar2).Value = detallePerturbacion.DescripcionEvento;
                    command.Parameters.Add("p_condicionesPrevias", OracleDbType.Varchar2).Value = detallePerturbacion.CondicionesPrevias;
                    command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = DateTime.Now.ToString("dd/MM/yyyy"); 
                    command.Parameters.Add("p_anexos", OracleDbType.Varchar2).Value = detallePerturbacion.Anexos;
                    command.Parameters.Add("p_rutaImagenes", OracleDbType.Varchar2).Value = rutaImagenes;
                    command.Parameters.Add("p_idReportePerturbacion", OracleDbType.Varchar2).Value = detallePerturbacion.IdReportePerturbacion;
                    command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;
                    command.Parameters.Add("p_idSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

                    command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = System.Data.ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                    respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                    if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                    {
                        respuesta.Mensaje = "Ok";
                    }
                    else
                    {
                        respuesta.Mensaje = "Error al guardar los datos.";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la petición.";
            }

            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarSecuenciaPrincipal(SecuenciaCronologica datosFormatoCampo)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    OracleCommand command = new OracleCommand("GuardarSecuenciaPerturbacion", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    
                    command.Parameters.Add("p_idSecuenciaCronologica", OracleDbType.Varchar2).Value = datosFormatoCampo.IdSecuenciaCronologica;
                    command.Parameters.Add("p_posicion", OracleDbType.Varchar2).Value = datosFormatoCampo.Posicion;
                    command.Parameters.Add("p_hora", OracleDbType.Varchar2).Value = datosFormatoCampo.Hora;
                    command.Parameters.Add("p_componente", OracleDbType.Varchar2).Value = datosFormatoCampo.Componente;
                    command.Parameters.Add("p_descripcionEvento", OracleDbType.Varchar2).Value = datosFormatoCampo.DescripcionEvento;
                    command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;
                    command.Parameters.Add("p_idDetallePerturbacion", OracleDbType.Varchar2).Value = datosFormatoCampo.IdDetallePerturbacion;

                    command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = System.Data.ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                    respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                    if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                    {
                        respuesta.Mensaje = "Ok";
                    }
                    else
                    {
                        respuesta.Mensaje = "Error al guardar los datos.";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la petición.";
            }

            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarSuministrosinterrumpidos(SuministrosInterrumpidos datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    OracleCommand command = new OracleCommand("GuardarSuministrosInter", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    
                    command.Parameters.Add("p_idSuministrosInterrumpidos", OracleDbType.Varchar2).Value = datos.IdSuministrosInterrumpidos;
                    command.Parameters.Add("p_equipo", OracleDbType.Varchar2).Value = datos.Equipo;
                    command.Parameters.Add("p_potencia", OracleDbType.Decimal).Value = datos.Potencia;
                    command.Parameters.Add("p_TiempoInicio", OracleDbType.Varchar2).Value = datos.TiempoInicio;
                    command.Parameters.Add("p_Tiempofinal", OracleDbType.Varchar2).Value = datos.Tiempofinal;
                    command.Parameters.Add("p_TiempoDuracion", OracleDbType.Varchar2).Value = datos.TiempoDuracion;
                    command.Parameters.Add("p_idDetallePerturbacion", OracleDbType.Varchar2).Value = datos.IdDetallePerturbacion;

                    command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = System.Data.ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                    respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                    if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                    {
                        respuesta.Mensaje = "Ok";
                    }
                    else
                    {
                        respuesta.Mensaje = "Error al guardar los datos.";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la petición.";
            }

            return respuesta;
        }
    }
}
