using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models.ReporteRAM;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;

namespace Generacion.Application.RAM.Query
{
    public class ObtenerRegistrosRAM
    {
        private readonly Function _function;
        private readonly IConexionBD _conexion;
        public ObtenerRegistrosRAM(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }


        public async Task<Respuesta<List<DBRAM>>> ObtenerRegistroRam(string fecha , string tipoBusqueda)
        {
            Respuesta<List<DBRAM>> respuesta = new Respuesta<List<DBRAM>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerDatosRamPorFecha", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new OracleParameter("p_fecha", OracleDbType.Varchar2, fecha, System.Data.ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("p_tipoBusqueda", OracleDbType.Varchar2, tipoBusqueda, System.Data.ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("p_sitio", OracleDbType.Varchar2, datosOperario.IdSitio, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            respuesta.Detalle = new List<DBRAM>();
                            DBRAM ram = new DBRAM();
                            while (reader.Read())
                            {
                                ram = new DBRAM();

                                ram.IdReporteRam = reader["idReporteRam"].ToString();
                                ram.ConsumoServiciosAuxiliares =decimal.Parse(reader["ConsumoServiciosAuxiliares"].ToString());
                                ram.LHV_kJkgGE1 =decimal.Parse(reader["LHV_kJkgGE1"].ToString());
                                ram.LHV_kJkgGE2 =decimal.Parse(reader["LHV_kJkgGE2"].ToString());
                                ram.HorasDerateoEquivalenteGE1 = reader["HorasDerateoEquivalenteGE1"].ToString();
                                ram.HorasDerateoEquivalenteGE2 = reader["HorasDerateoEquivalenteGE2"].ToString();
                                ram.CapacidadMaximaNetaGE1 = decimal.Parse(reader["CapacidadMaximaNetaGE1"].ToString());
                                ram.CapacidadMaximaNetaGE2 = decimal.Parse(reader["CapacidadMaximaNetaGE2"].ToString());
                                ram.Fecha = reader["fecha"].ToString();

                                respuesta.Detalle.Add(ram);
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




        public async Task<Respuesta<List<ViewOIL>>> ObtenerRegistroRamOil()
        {
            Respuesta<List<ViewOIL>> respuesta = new Respuesta<List<ViewOIL>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerDatosRamOil", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new OracleParameter("p_sitio", OracleDbType.Varchar2, datosOperario.IdSitio, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            respuesta.Detalle = new List<ViewOIL>();
                            ViewOIL ram = new ViewOIL();
                            while (reader.Read())
                            {
                                ram = new ViewOIL();

                                ram.IdReporteRamAceite = reader["IdReporteRamAceite"].ToString();
                                ram.IdReporteRam = reader["IdReporteRam"].ToString();
                                ram.IdTipoEngine = reader["IdTipoEngine"].ToString();
                                ram.NumeroGe = int.Parse(reader["NumeroGe"].ToString());
                                ram.Detalle = decimal.Parse(reader["Detalle"].ToString());
                                ram.Posicion = int.Parse(reader["Posicion"].ToString());
                                ram.Fecha = reader["Fecha"].ToString();

                                respuesta.Detalle.Add(ram);
                            }

                            respuesta.Detalle = respuesta.Detalle.OrderBy(x => DateTime.Parse(x.Fecha)).ToList();
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
