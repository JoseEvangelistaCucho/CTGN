using Generacion.Application.Common;
using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.PruebasSemanales.BlackStart;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Security.Policy;

namespace Generacion.Application.PruebasSemanales.BackStart.Command
{
    public class RegistroDatosPruebaSemanal : IRegistroDatosPruebaSemanal
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistroDatosPruebaSemanal(IConexionBD conexionBD, Function function)
        {
            _conexion = conexionBD;
            _function = function;
        }

        public async Task<Respuesta<string>> GuardarDetallePruebaSemanal(DetallePruebaSemanal datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("GuardarDetallePruebaSemanal", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_idDetallePruebaSemanal", OracleDbType.Varchar2).Value = datos.IdDetallePruebaSemanal;
                        command.Parameters.Add("p_idSubtitulo", OracleDbType.Varchar2).Value = datos.IdSubtitulo;
                        command.Parameters.Add("p_valorReferencia", OracleDbType.Varchar2).Value = datos.ValorReferencia;
                        command.Parameters.Add("p_detalleNumerico", OracleDbType.Decimal).Value = datos.DetalleNumerico;
                        command.Parameters.Add("p_detalleCadena", OracleDbType.Varchar2).Value = datos.DetalleCadena;
                        command.Parameters.Add("p_observaciones", OracleDbType.Varchar2).Value = datos.Observaciones;
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = datos.Fecha;
                        command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = "";
                        command.Parameters.Add("p_idPruebaSemanal", OracleDbType.Varchar2).Value = datos.IdPruebaSemanal;

                        command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;

                        if (respuesta.IdRespuesta == 0)
                        {
                            respuesta.Mensaje = Mensajes.sucess;
                        }
                        else if (respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = Mensajes.update;
                        }
                        else if (respuesta.IdRespuesta == 99)
                        {
                            respuesta.Mensaje = Mensajes.error;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = Mensajes.error;
            }
            return respuesta;
        }
        public async Task<Respuesta<string>> GuardarDatosPruebaSemanal(PruebaSemanal datos,string tipo)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("InsertOrUpdatePruebaSemanal", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_idPruebaSemanal", OracleDbType.Varchar2).Value = datos.IdPruebaSemanal;
                        command.Parameters.Add("p_tipoPruebaSemanal", OracleDbType.Varchar2).Value = tipo;
                        command.Parameters.Add("p_fecha", OracleDbType.TimeStamp).Value = datos.Fecha;
                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_activo", OracleDbType.Int32).Value = datos.Activo;
                        command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;

                        command.Parameters.Add("p_resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                        if (respuesta.IdRespuesta == 0)
                        {
                            respuesta.Mensaje = Mensajes.sucess;
                        }
                        else if (respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = Mensajes.update;
                        }
                        else if (respuesta.IdRespuesta == 99)
                        {
                            respuesta.Mensaje = Mensajes.error;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = Mensajes.error;
            }
            return respuesta;
        }
    }
}
