using Generacion.Application.DataBase;
using Generacion.Application.DataBase.cache;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Almacen;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.Almacen.Command
{
    public class RegistrosAlmacen : IAlmacen
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistrosAlmacen(Function function, IConexionBD conexion)
        {
            _function = function;
            _conexion = conexion;
        }
        public async Task<Respuesta<string>> GuardarDatosAlmacen(Componentes componentes,string idSitio)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "InsertarComponenteCompleto";
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add("p_TipoComponenteID", OracleDbType.Varchar2).Value = componentes.TipoComponenteID;
                        command.Parameters.Add("p_LoteID", OracleDbType.Varchar2).Value = $"{componentes.NumeroLote.Trim()}{DateTime.Now.ToString("ddMMyyyy")}";
                        command.Parameters.Add("p_ComponenteID", OracleDbType.Varchar2).Value = $"{componentes.NombreComponente.Replace(" ", "")}{componentes.NumeroLote}_{DateTime.Now.ToString("ddMMyyyy")}";
                        command.Parameters.Add("p_NombreComponente", OracleDbType.Varchar2).Value = componentes.NombreComponente;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = idSitio;
                        command.Parameters.Add("p_NumeroLote", OracleDbType.Varchar2).Value = componentes.NumeroLote;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = DateTime.Now.ToString("dd/MM/yyyy");
                        command.Parameters.Add("p_Cantidad", OracleDbType.Int32).Value = int.Parse(componentes.Cantidad);
                        command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();
                        
                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;
                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                        respuesta.Mensaje = "Ok";
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

        public async Task<Respuesta<string>> GuardarDatosPrestados(Prestamo prestamo)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("InsertarComponentePrestado", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //string[] componenteIDSplit = prestamo.ComponenteID.Split('_');
                        //string nuevoComponenteID = $"P_{componenteIDSplit[0]}_{DateTime.Now.ToString("ddMMyyyy")}";
                        command.Parameters.Add("p_ComponentePrestadoID", OracleDbType.Varchar2).Value = prestamo.ComponentePrestadoID;
                       // command.Parameters.Add("p_ComponenteID", OracleDbType.Varchar2).Value = prestamo.ComponenteID;
                        //command.Parameters.Add("p_NewComponenteID", OracleDbType.Varchar2).Value = nuevoComponenteID;
                        command.Parameters.Add("p_FechaPrestamo", OracleDbType.Varchar2).Value = DateTime.Now.ToString("dd/MM/yyyy");
                        command.Parameters.Add("p_PrestamoDesde", OracleDbType.Varchar2).Value = prestamo.PrestamoDesde;
                        command.Parameters.Add("p_PrestamoHacia", OracleDbType.Varchar2).Value = prestamo.PrestamoHacia;
                        command.Parameters.Add("p_CantidadPrestamo", OracleDbType.Decimal).Value = prestamo.CantidadPrestamo;

                        command.Parameters.Add("p_Resultado", OracleDbType.Decimal).Direction = System.Data.ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;
                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;

                        if (respuesta.IdRespuesta == 0)
                        {
                            respuesta.Mensaje = "Éxito";
                        }
                        else if (respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = "Cantidad insuficiente";
                        }
                        else
                        {
                            respuesta.Mensaje = "Error desconocido";
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

        public async Task<Respuesta<string>> GuardarBujiasUtilizadas(int cantidad)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("InsertarBujiaUtilizado", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_CantidadUsado", OracleDbType.Decimal).Value = cantidad;

                        command.Parameters.Add("p_Resultado", OracleDbType.Decimal).Direction = System.Data.ParameterDirection.Output;
                        command.Parameters.Add("p_Mensaje", OracleDbType.Varchar2).Direction = System.Data.ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;
                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;

                        if (respuesta.IdRespuesta == 0)
                        {
                            respuesta.Mensaje = "Éxito";
                        }
                        else
                        {
                            respuesta.Mensaje = command.Parameters["p_resultado"].Value.ToString();
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
