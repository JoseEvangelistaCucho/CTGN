using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.DashBoard;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.Generic;

namespace Generacion.Application.DashBoard.Filtro.Query
{
    public class DatosFiltro
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosFiltro(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }


        public async Task<Respuesta<List<DashboardDetalleFiltro>>> ObtenerDatosFiltroPorSitio(string seleccion, string IdReporteFiltro)
        {
            Respuesta<List<DashboardDetalleFiltro>> respuesta = new Respuesta<List<DashboardDetalleFiltro>>();
            DetalleOperario user = await _function.ObtenerDatosOperario();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ObtenerDetalleFiltro", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        cmd.Parameters.Add("p_Seleccion", OracleDbType.Varchar2).Value = seleccion;
                        cmd.Parameters.Add("p_IdReporteFiltro", OracleDbType.Varchar2).Value = IdReporteFiltro;
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();

                        respuesta.Detalle = new List<DashboardDetalleFiltro>();
                        DashboardDetalleFiltro dashboardDetalleFiltro = new DashboardDetalleFiltro();
                        while (reader.Read())
                        {
                            dashboardDetalleFiltro = new DashboardDetalleFiltro()
                            {
                                IdDetalleFiltro = reader["idDetalleFiltro"].ToString(),
                                Fecha = reader["Fecha"].ToString(),
                                OperadorEjecutor = reader["OperadorEjecutor"].ToString(),
                                Espesor = decimal.Parse(reader["ESPESOR"].ToString()),
                                HorasOperacion = decimal.Parse(reader["HorasOperacion"].ToString()),
                                NumeroGenerador = decimal.Parse(reader["NumeroGenerador"].ToString()),
                                ProximaHoraCambio = decimal.Parse(reader["ProximaHoraCambio"].ToString())
                            };

                            respuesta.Detalle.Add(dashboardDetalleFiltro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Respuesta<Dictionary<decimal, DashboardDetalleFiltro>>> ObtenerDetalleDashboardPorNumeroGE(string seleccion, string IdReporteFiltro)
        {
            Respuesta<List<DashboardDetalleFiltro>> datosFiltro = await ObtenerDatosFiltroPorSitio(seleccion, IdReporteFiltro);
            datosFiltro.Detalle = datosFiltro.Detalle
                .OrderBy(x =>
                {
                    DateTime fecha;
                    return DateTime.TryParse(x.Fecha, out fecha) ? fecha : DateTime.MinValue;
                })
                .ToList();

            // Tomar los dos últimos valores de la lista
            var ultimosDosValores = datosFiltro.Detalle
                .Skip(Math.Max(0, datosFiltro.Detalle.Count - 2)) // Ignorar los primeros elementos si hay menos de dos
                .Take(2)
                .ToList();

            // Crear un diccionario con los dos últimos valores
            Respuesta<Dictionary<decimal, DashboardDetalleFiltro>> respuesta = new Respuesta<Dictionary<decimal, DashboardDetalleFiltro>>();
            respuesta.Detalle = ultimosDosValores.ToDictionary(x => x.NumeroGenerador);

            return respuesta;
        }
    }
}
