using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models.DatosConsola;
using Generacion.Models;
using Generacion.Models.InformePerturbacion;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Generacion.Application.InformePerturbacion.Query
{
    public class DatosPerturbacion
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosPerturbacion(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }

        public async Task<Respuesta<InformeGeneralPerturbacion>> ObtenerDatosPrincipal(string fecha,string hora)
        {
            Respuesta<InformeGeneralPerturbacion> respuesta = new Respuesta<InformeGeneralPerturbacion>();
            
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ConsultarInformePerturbacion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_idSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_hora", OracleDbType.Varchar2).Value = hora;

                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            InformeGeneralPerturbacion informeGeneralPerturbacion = new InformeGeneralPerturbacion();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                respuesta.Detalle = new InformeGeneralPerturbacion(){

                                    IdReportePerturbacion = row["idReportePerturbacion"].ToString(),
                                    Evento = row["evento"].ToString(),
                                    Fecha = row["fecha"].ToString(),
                                    Hora = row["hora"].ToString(),
                                    Componente = row["componente"].ToString(),
                                    Propietario = row["propietario"].ToString(),
                                    sitio = row["idsitio"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }

        public async Task<Respuesta<DetalleInformePerturbacion>> ConsultarDetalle(string fecha, string hora)
        {
            Respuesta<DetalleInformePerturbacion> respuesta = new Respuesta<DetalleInformePerturbacion>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ConsultarDetallePerturbacion", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_idSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_hora", OracleDbType.Varchar2).Value = hora;

                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            DetalleInformePerturbacion detalleInforme = new DetalleInformePerturbacion();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                respuesta.Detalle = new DetalleInformePerturbacion()
                                {
                                    Anexos = row["anexos"].ToString(),
                                    CondicionesPrevias = row["condicionesPrevias"].ToString(),
                                    DescripcionEvento = row["descripcionEvento"].ToString(),
                                    IdDetallePerturbacion = row["idDetallePerturbacion"].ToString(),
                                    RutaImagenes = row["rutaImagenes"].ToString(),
                                    IdReportePerturbacion = row["idReportePerturbacion"].ToString()
                                   
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }

        public async Task<Respuesta<List<SecuenciaCronologica>>> ObteterSecuentaCronologico(string fecha, string hora)
        {
            Respuesta<List<SecuenciaCronologica>> respuesta = new Respuesta<List<SecuenciaCronologica>> ();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ConsultarSequenciaCronologica", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_idSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_hora", OracleDbType.Varchar2).Value = hora;

                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            respuesta.Detalle = new List<SecuenciaCronologica>();
                            SecuenciaCronologica detalleSecuencia = new SecuenciaCronologica();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                detalleSecuencia = new SecuenciaCronologica()
                                {
                                    IdDetallePerturbacion = row["IdDetallePerturbacion"].ToString(),
                                    Componente = row["componente"].ToString(),
                                    Hora = row["Hora"].ToString(),
                                    DescripcionEvento = row["DescripcionEvento"].ToString(),
                                    IdSecuenciaCronologica = row["IdSecuenciaCronologica"].ToString(),
                                    Posicion = int.Parse(row["Posicion"].ToString())
                                };

                                respuesta.Detalle.Add(detalleSecuencia);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }


        //  Task<Respuesta<string>> GuardarSecuenciaPrincipal(SecuenciaCronologica datos);
        public async Task<Respuesta<List<SuministrosInterrumpidos>>> ObtenerSuministrosinterrumpidos(string fecha, string hora)
        {
            Respuesta<List<SuministrosInterrumpidos>> respuesta = new Respuesta<List<SuministrosInterrumpidos>>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ConsultarSumInterrumpidos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_idSitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_hora", OracleDbType.Varchar2).Value = hora;

                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            respuesta.Detalle = new List<SuministrosInterrumpidos>();
                            SuministrosInterrumpidos suministros = new SuministrosInterrumpidos();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                suministros = new SuministrosInterrumpidos()
                                {
                                    Equipo = row["Equipo"].ToString(),
                                    IdDetallePerturbacion = row["IdDetallePerturbacion"].ToString(),
                                    IdSuministrosInterrumpidos = row["IdSuministrosInterrumpidos"].ToString(),
                                    Potencia =int.Parse(row["Potencia"].ToString()),
                                    TiempoDuracion = row["TiempoDuracion"].ToString(),
                                    Tiempofinal = row["Tiempofinal"].ToString(),
                                    TiempoInicio = row["TiempoInicio"].ToString()
                                };

                                respuesta.Detalle.Add(suministros);
                            }
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
