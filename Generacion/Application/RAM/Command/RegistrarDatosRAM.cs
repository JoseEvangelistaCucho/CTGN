using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.ReporteRAM;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.RAM.Command
{
    public class RegistrarDatosRAM : IRegistroRAM
    {
        private readonly Function _function;
        private readonly IConexionBD _conexion;
        public RegistrarDatosRAM(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }

        public async Task<Respuesta<string>> GuardarDatosRAM(DBRAM dbRAM)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("GuardarDatosReporteRam", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agrega los parámetros con sus valores
                        command.Parameters.Add("p_idReporteRam", OracleDbType.Varchar2).Value = dbRAM.IdReporteRam;
                        command.Parameters.Add("p_ConsumoServiciosAuxiliares", OracleDbType.Decimal).Value = dbRAM.ConsumoServiciosAuxiliares;
                        command.Parameters.Add("p_LHV_kJkgGE1", OracleDbType.Decimal).Value = dbRAM.LHV_kJkgGE1;
                        command.Parameters.Add("p_LHV_kJkgGE2", OracleDbType.Decimal).Value = dbRAM.LHV_kJkgGE2;
                        command.Parameters.Add("p_HorasDerateoEquivalenteGE1", OracleDbType.Varchar2).Value = dbRAM.HorasDerateoEquivalenteGE1;
                        command.Parameters.Add("p_HorasDerateoEquivalenteGE2", OracleDbType.Varchar2).Value = dbRAM.HorasDerateoEquivalenteGE2;
                        command.Parameters.Add("p_CapacidadMaximaNetaGE1", OracleDbType.Decimal).Value = dbRAM.CapacidadMaximaNetaGE1;
                        command.Parameters.Add("p_CapacidadMaximaNetaGE2", OracleDbType.Decimal).Value = dbRAM.CapacidadMaximaNetaGE1;
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = dbRAM.Fecha;
                        command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;
                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

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
                            respuesta.Mensaje = "Error al Guardar.";
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }


        /// <summary>
        /// Funcion que Guarda los datos de la vista  Oil del Excel
        /// </summary>
        /// <param name="dbRAM"></param>
        /// <returns></returns>
        public async Task<Respuesta<string>> GuardarDatosRAMOil(ViewOIL  dbRAM)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("GuardarDatosGeneralesRAMOil", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdReporteRamAceite", OracleDbType.Varchar2).Value = dbRAM.IdReporteRamAceite;
                        command.Parameters.Add("p_IdReporteRam", OracleDbType.Varchar2).Value = dbRAM.IdReporteRam;
                        command.Parameters.Add("p_IdTipoEngine", OracleDbType.Varchar2).Value = dbRAM.IdTipoEngine;
                        command.Parameters.Add("p_Detalle", OracleDbType.Decimal).Value = dbRAM.Detalle;
                        command.Parameters.Add("p_NumeroGe", OracleDbType.Decimal).Value = dbRAM.NumeroGe;
                        command.Parameters.Add("p_Posicion", OracleDbType.Decimal).Value = dbRAM.Posicion;
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = dbRAM.Fecha;
                        command.Parameters.Add("p_idOperario", OracleDbType.Varchar2).Value = datosOperario.IdOperario;
                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

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
                            respuesta.Mensaje = "Error al Guardar.";
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
    }
}
