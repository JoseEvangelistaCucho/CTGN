using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Controllers;
using Generacion.Models;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.ReporteModulacion.Command
{
    public class GuardarDatosModulacion
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public GuardarDatosModulacion(IConexionBD conexion, Function function)
        {
            _conexion = conexion;
            _function = function;
        }

        public async Task<Respuesta<string>> GuardarDetallesModulacion(DateTime fecha, decimal tipoCambio, ValoresPEMFSoles valoresPEM)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();
                await Task.Run(() =>
                {
                    using (OracleConnection connection = _conexion.ObtenerConexion())
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("InsertarDetalleModulacion", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_idDetModulacion", OracleDbType.Varchar2).Value = $"{user.IdSitio}_Modulacion_{fecha.ToString("ddMMyyyy")}";
                            command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                            command.Parameters.Add("p_fecha", OracleDbType.TimeStamp).Value = fecha;
                            command.Parameters.Add("p_PEMP", OracleDbType.Decimal).Value = valoresPEM.PEMP;
                            command.Parameters.Add("p_PEMf", OracleDbType.Decimal).Value = valoresPEM.PEMF;
                            command.Parameters.Add("p_tipoCambio", OracleDbType.Decimal).Value = tipoCambio;
                            command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = user.IdOperario;

                            command.Parameters.Add("p_resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;
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
                                respuesta.Mensaje = "Error al guardar.";
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al guardar.";
            }
            return respuesta;
        }
    }
}
