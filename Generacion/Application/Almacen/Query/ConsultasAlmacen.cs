using Generacion.Application.DataBase;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Almacen;
using Generacion.Models.Almacen.Bujias;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Generacion.Application.Almacen.Query
{
    public class ConsultasAlmacen
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public ConsultasAlmacen(Function function, IConexionBD conexion)
        {
            _function = function;
            _conexion = conexion;
        }
        public async Task<Respuesta<List<TipoComponente>>> ObtenerTipo()
        {
            Respuesta<List<TipoComponente>> respuesta = new Respuesta<List<TipoComponente>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    // Crear un comando Oracle
                    using (OracleCommand command = connection.CreateCommand())
                    {
                        // Establecer el nombre del procedimiento almacenado
                        command.CommandText = "ConsultarComponentesPorTipo";
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Agregar parámetro de salida (cursor)
                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        // Ejecutar el procedimiento almacenado
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<TipoComponente> listatipo = new List<TipoComponente>();
                            TipoComponente componente = new TipoComponente();
                            while (reader.Read())
                            {
                                componente = new TipoComponente()
                                {
                                    Nombre = reader["Nombre"].ToString(),
                                    TipoComponenteID = reader["TipoComponenteID"].ToString()
                                };
                                listatipo.Add(componente);
                            }

                            respuesta.Detalle = new List<TipoComponente>();
                            respuesta.Detalle = listatipo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }

        public async Task<Respuesta<List<ComponenteBujias>>> ObtenerDatosAlmacenBujias(string tipoComponenteID)
        {
            Respuesta<List<ComponenteBujias>> respuesta = new Respuesta<List<ComponenteBujias>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerComponentesPorTipo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_TipoComponenteID", OracleDbType.Varchar2).Value = tipoComponenteID;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<ComponenteBujias> componentes = new List<ComponenteBujias>();

                            while (reader.Read())
                            {
                                ComponenteBujias componente = new ComponenteBujias
                                {
                                    ComponenteID = reader["ComponenteID"].ToString(),
                                    TipoComponenteID = reader["TipoComponenteID"].ToString(),
                                    NombreTipo = reader["NombreTipo"].ToString(),
                                    NombreComponente = reader["NombreComponente"].ToString(),
                                    Cantidad = reader["Cantidad"].ToString(),
                                    LoteID = reader["LoteID"].ToString(),
                                    NumeroLote = reader["NumeroLote"].ToString(),
                                    Fecha = Convert.ToDateTime(reader["Fecha"])
                                };

                                componentes.Add(componente);
                            }
                            respuesta.Detalle = new List<ComponenteBujias>();
                            respuesta.Detalle = componentes;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Respuesta<List<ComponenteBujias>>> ObtenerDatosporLote()
        {
            Respuesta<List<ComponenteBujias>> respuesta = new Respuesta<List<ComponenteBujias>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerComponentesPorLote", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<ComponenteBujias> componentes = new List<ComponenteBujias>();

                            while (reader.Read())
                            {
                                ComponenteBujias componente = new ComponenteBujias
                                {
                                    ComponenteID = reader["ComponenteID"].ToString(),
                                    TipoComponenteID = reader["TipoComponenteID"].ToString(),
                                    NombreTipo = reader["NombreTipo"].ToString(),
                                    NombreComponente = reader["NombreComponente"].ToString(),
                                    Cantidad = reader["Cantidad"].ToString(),
                                    LoteID = reader["LoteID"].ToString(),
                                    NumeroLote = reader["NumeroLote"].ToString(),
                                    Fecha = Convert.ToDateTime(reader["Fecha"])
                                };

                                componentes.Add(componente);
                            }
                            respuesta.Detalle = new List<ComponenteBujias>();
                            respuesta.Detalle = componentes;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Respuesta<List<PrestamoComponente>>> ObtenerDatosPrestados(string tipoComponenteID)
        {
            Respuesta<List<PrestamoComponente>> respuesta = new Respuesta<List<PrestamoComponente>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();

                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerComponentesPrestado", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_TipoComponenteID", OracleDbType.Varchar2).Value = tipoComponenteID;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<PrestamoComponente> componentes = new List<PrestamoComponente>();

                            while (reader.Read())
                            {
                                PrestamoComponente componente = new PrestamoComponente
                                {
                                    NombreComponente = reader["nombrecomponente"].ToString(),
                                    ComponentePrestadoID = reader["componenteprestadoid"].ToString(),
                                    ComponenteID = reader["componenteid"].ToString(),
                                    PrestamoDesde = reader["prestamodesde"].ToString(),
                                    PrestamoHacia = reader["prestamohacia"].ToString(),
                                    FechaPrestamos = DateTime.Parse(reader["fechaprestamo"].ToString()),
                                    CantidadPrestamo =int.Parse(reader["cantidadprestamo"].ToString())
                                };
                                componentes.Add(componente);
                            }
                            respuesta.Detalle = new List<PrestamoComponente>();
                            respuesta.Detalle = componentes;
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
