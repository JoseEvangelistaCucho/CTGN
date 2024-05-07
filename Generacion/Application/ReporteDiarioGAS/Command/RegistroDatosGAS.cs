using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Application.ReporteDiarioGAS;
using Generacion.Models;
using Generacion.Models.ReporteDiarioGAS;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using MReporteGAS = Generacion.Models.ReporteDiarioGAS.ReporteGAS;

namespace Generacion.Application.ReporteGAS.Command
{
    public class RegistroDatosGAS : IRegistroDatosGAS
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public RegistroDatosGAS(IConexionBD conexionBD, Function function)
        {
            _conexion = conexionBD;
            _function = function;
        }

        public async Task<Respuesta<string>> GuardarDetalle(List<DetalleReporteGas> datos)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    foreach (DetalleReporteGas item in datos)
                    {
                        using (OracleCommand command = new OracleCommand("proc_InsDetalleGAS", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;

                            OracleParameter idDetalleReporte = new OracleParameter("p_IdDetalleReporte", OracleDbType.Varchar2);
                            OracleParameter fecha = new OracleParameter("p_Fecha", OracleDbType.Varchar2);
                            OracleParameter hora = new OracleParameter("p_Hora", OracleDbType.Varchar2);
                            OracleParameter vc = new OracleParameter("p_VC", OracleDbType.Decimal);
                            OracleParameter vn = new OracleParameter("p_VN", OracleDbType.Decimal);
                            OracleParameter pl = new OracleParameter("p_PL", OracleDbType.Decimal);
                            OracleParameter fl = new OracleParameter("p_FL", OracleDbType.Decimal);
                            OracleParameter sv = new OracleParameter("p_SV", OracleDbType.Decimal);
                            OracleParameter tl = new OracleParameter("p_TL", OracleDbType.Decimal);
                            OracleParameter fc = new OracleParameter("p_FC", OracleDbType.Decimal);
                            OracleParameter fs = new OracleParameter("p_FS", OracleDbType.Decimal);
                            OracleParameter da = new OracleParameter("p_DA", OracleDbType.Decimal);
                            OracleParameter dp = new OracleParameter("p_DP", OracleDbType.Decimal);
                            OracleParameter pr = new OracleParameter("p_PR", OracleDbType.Decimal);
                            OracleParameter d1 = new OracleParameter("p_D1", OracleDbType.Decimal);
                            OracleParameter d2 = new OracleParameter("p_D2", OracleDbType.Decimal);
                            OracleParameter l = new OracleParameter("p_L", OracleDbType.Decimal);
                            OracleParameter caudalimetro = new OracleParameter("p_Caudalimetro", OracleDbType.Decimal);
                            OracleParameter presionIngreso = new OracleParameter("p_PresionIngreso", OracleDbType.Decimal);
                            OracleParameter idReporteGas = new OracleParameter("p_IdReporteGas", OracleDbType.Varchar2);
                            OracleParameter resultado = new OracleParameter("p_resultado", OracleDbType.Decimal);
                            resultado.Direction = System.Data.ParameterDirection.Output;

                            command.Parameters.AddRange(new OracleParameter[] {
                                idDetalleReporte,
                                fecha,
                                hora,
                                vc,
                                vn,
                                pl,
                                fl,
                                sv,
                                tl,
                                fc,
                                fs,
                                da,
                                dp,
                                pr,
                                d1,
                                d2,
                                l,
                                caudalimetro,
                                presionIngreso,
                                idReporteGas,
                                resultado
                            });

                            idDetalleReporte.Value = item.IdDetalleReporte;
                            fecha.Value = item.Fecha.Split(' ')[0];
                            hora.Value = item.Hora;
                            vc.Value = item.VC;
                            vn.Value = item.VN;
                            pl.Value = item.PL;
                            fl.Value = item.FL;
                            sv.Value = item.SV;
                            tl.Value = item.TL;
                            fc.Value = item.FC;
                            fs.Value = item.FS;
                            da.Value = item.DA;
                            dp.Value = item.DP;
                            pr.Value = item.PR;
                            d1.Value = item.D1;
                            d2.Value = item.D2;
                           // l.Value = item.L;
                            caudalimetro.Value = item.Caudalimetro;
                            presionIngreso.Value = item.PresionIngreso;
                            idReporteGas.Value = item.IdReporteGas;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)resultado.Value;

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


        public async void guardarIdReporte(MReporteGAS reporteGAS)
        {
            DetalleOperario user = await _function.ObtenerDatosOperario();

            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("InsertOrUpdateReporteGas", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdReporteGas", OracleDbType.Varchar2).Value = reporteGAS.IdReporteGas;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = reporteGAS.Fecha;
                        command.Parameters.Add("p_Activo", OracleDbType.Int32).Value = reporteGAS.Activo;

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            
        }
    }
}
