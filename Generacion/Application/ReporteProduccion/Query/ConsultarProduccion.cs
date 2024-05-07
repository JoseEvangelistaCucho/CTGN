using Generacion.Models.ReporteProduccion;
using Generacion.Models;
using Generacion.Application.DataBase;
using System.Data;
using Oracle.ManagedDataAccess.Types;
using Generacion.Application.Funciones;

namespace Generacion.Application.ReporteProduccion.Query
{
    public class ConsultarProduccion
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public ConsultarProduccion(IConexionBD conexion, Function funtion)
        {
            _conexion = conexion;
            _function = funtion;
        }
        public async Task<Respuesta<List<EnergiaProducida>>> ObtenerRegistroProduccion(string fecha, string tipoEnergia, string tipoConsulta)
        {
            Respuesta<List<EnergiaProducida>> respuesta = new Respuesta<List<EnergiaProducida>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();
                    //string sqlQuery = @"SELECT * FROM Tbl_EnergiaProducida WHERE Fecha = :fechaActual AND TipoEnergia = 'READING TODAY'";

                    using (OracleCommand command = new OracleCommand("ConsultarEnergiaProducida", connection))
                    {
                        //command.Parameters.Add(":fechaActual", OracleDbType.Varchar2).Value = fechaActual;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add("p_fechaActual", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_tipoEnergia", OracleDbType.Varchar2).Value = tipoEnergia;
                        command.Parameters.Add("p_tipoConsulta", OracleDbType.Varchar2).Value = tipoConsulta;
                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

                        // Parámetro de salida (cursor)
                        command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)command.Parameters["p_resultado"].Value).GetDataReader();

                        respuesta.Detalle = new List<EnergiaProducida>();
                        EnergiaProducida registroProduccion = new EnergiaProducida();
                        while (reader.Read())
                        {
                            registroProduccion = new EnergiaProducida();
                            registroProduccion.IdEnergyProduce = reader["IdEnergyProduce"].ToString();
                            registroProduccion.TipoEnergia = reader["TipoEnergia"].ToString();
                            registroProduccion.Fecha = reader["Fecha"].ToString();
                            registroProduccion.PmuEng_01 = decimal.Parse(reader["PmuEng_01"].ToString());
                            registroProduccion.PmuEng_02 = decimal.Parse(reader["PmuEng_02"].ToString());
                            registroProduccion.GrosEnergy = decimal.Parse(reader["GrosEnergy"].ToString());

                            respuesta.Detalle.Add(registroProduccion);
                        }
                        if (respuesta.Detalle.Count() > 0)
                        {
                            respuesta.IdRespuesta = 0;
                            respuesta.Mensaje = "Ok";
                        }
                        else
                        {
                            respuesta.IdRespuesta = 1;
                            respuesta.Mensaje = "No hay registros";
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

        public async Task<Respuesta<List<LevelLubeOilCartel>>> ObtenerRegistroLevelCartel(string fecha, string tipoConsulta, string tipoCarter)
        {

            Respuesta<List<LevelLubeOilCartel>> respuesta = new Respuesta<List<LevelLubeOilCartel>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    //string sqlQuery = @"SELECT * FROM tbl_Level_Oil_Carter WHERE Fecha = :fechaActual AND TipoCarter = 'TODAY'";

                    using (OracleCommand command = new OracleCommand("ObtenerDatosOilCarter", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //command.Parameters.Add(":fechaActual", OracleDbType.Varchar2).Value = fechaActual;
                        // Parámetros de entrada
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_TipoCarter", OracleDbType.Varchar2).Value = tipoCarter;
                        command.Parameters.Add("p_tipoConsulta", OracleDbType.Varchar2).Value = tipoConsulta;
                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

                        // Parámetro de salida (cursor)
                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)command.Parameters["p_cursor"].Value).GetDataReader();

                        List<LevelLubeOilCartel> listaLevelCartel = new List<LevelLubeOilCartel>();
                        LevelLubeOilCartel registroProduccionCartel = new LevelLubeOilCartel();
                        while (reader.Read())
                        {
                            registroProduccionCartel = new LevelLubeOilCartel();
                            registroProduccionCartel.IdLevelOilCarter = reader["IdLevelOilCarter"].ToString();
                            registroProduccionCartel.Fecha = reader["Fecha"].ToString();
                            registroProduccionCartel.TipoCarter = reader["TipoCarter"].ToString();
                            registroProduccionCartel.Generador1 = decimal.Parse(reader["Generador1"].ToString());
                            registroProduccionCartel.Generador2 = decimal.Parse(reader["Generador2"].ToString());
                            registroProduccionCartel.TotalAdded = decimal.Parse(reader["TotalAdded"].ToString());

                            listaLevelCartel.Add(registroProduccionCartel);
                        }

                        respuesta.Detalle = new List<LevelLubeOilCartel>();
                        respuesta.Detalle = listaLevelCartel;
                        if (respuesta.Detalle.Count() > 0)
                        {
                            respuesta.IdRespuesta = 0;
                            respuesta.Mensaje = "Ok";
                        }
                        else
                        {
                            respuesta.IdRespuesta = 1;
                            respuesta.Mensaje = "No hay registros";
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

        public async Task<Respuesta<List<CityGateFlow>>> ObtenerRegistroCityGate(string fecha, string tipoConsulta, string tipo)
        {

            Respuesta<List<CityGateFlow>> respuesta = new Respuesta<List<CityGateFlow>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerDatosCityGate", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_tipo", OracleDbType.Varchar2).Value = tipo;
                        command.Parameters.Add("p_tipoConsulta", OracleDbType.Varchar2).Value = tipoConsulta;

                        command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)command.Parameters["p_cursor"].Value).GetDataReader();

                        List<CityGateFlow> listaCity = new List<CityGateFlow>();
                        CityGateFlow registroProduccionCity = new CityGateFlow();
                        while (reader.Read())
                        {
                            registroProduccionCity = new CityGateFlow();
                            registroProduccionCity.IdCityGate = reader["IdCityGate"].ToString();
                            registroProduccionCity.Fecha = reader["Fecha"].ToString();
                            registroProduccionCity.Tipo = reader["Tipo"].ToString();
                            registroProduccionCity.KgEng1 = decimal.Parse(reader["KgEng1"].ToString());
                            registroProduccionCity.KgEng2 = decimal.Parse(reader["KgEng2"].ToString());

                            listaCity.Add(registroProduccionCity);

                        }

                        respuesta.Detalle = new List<CityGateFlow>();
                        respuesta.Detalle = listaCity;
                        if (respuesta.Detalle.Count() > 0)
                        {
                            respuesta.IdRespuesta = 0;
                            respuesta.Mensaje = "Ok";
                        }
                        else
                        {
                            respuesta.IdRespuesta = 1;
                            respuesta.Mensaje = "No hay registros";
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

        public async Task<Respuesta<List<TkCleanLube>>> ObtenerRegistroTkClean(string fechaActual)
        {

            Respuesta<List<TkCleanLube>> respuesta = new Respuesta<List<TkCleanLube>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"SELECT * FROM tbl_TkCleanLubeOil WHERE Fecha = :fechaActual";


                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {

                        command.Parameters.Add(":fechaActual", OracleDbType.Varchar2).Value = fechaActual;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<TkCleanLube> listaTkClean = new List<TkCleanLube>();
                            TkCleanLube registroProduccionTkClean = new TkCleanLube();
                            while (reader.Read())
                            {
                                registroProduccionTkClean = new TkCleanLube();
                                registroProduccionTkClean.IdTkCleanLube = reader["IdTkCleanLube"].ToString();
                                registroProduccionTkClean.Fecha = reader["Fecha"].ToString();
                                registroProduccionTkClean.Tipo = reader["Tipo"].ToString();
                                registroProduccionTkClean.TkLevel = decimal.Parse(reader["TkLevel"].ToString());
                                registroProduccionTkClean.TkRead = decimal.Parse(reader["TkRead"].ToString());

                                listaTkClean.Add(registroProduccionTkClean);

                            }

                            respuesta.Detalle = new List<TkCleanLube>();
                            respuesta.Detalle = listaTkClean;
                            if (respuesta.Detalle.Count() > 0)
                            {
                                respuesta.IdRespuesta = 0;
                                respuesta.Mensaje = "Ok";
                            }
                            else
                            {
                                respuesta.IdRespuesta = 1;
                                respuesta.Mensaje = "No hay registros";
                            }

                        }
                    }
                }
                if (respuesta.IdRespuesta == 0)
                {
                    respuesta.IdRespuesta = 0;
                    respuesta.Mensaje = "Ok";
                }
                else
                {
                    respuesta.IdRespuesta = 2;
                    respuesta.Mensaje = "Error al consultar.";
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }


        public async Task<Respuesta<Dictionary<string, TkCleanLube>>> ObtenerRegistroTkCleanPorTipo()
        {
            Respuesta<Dictionary<string, TkCleanLube>> respuesta = new Respuesta<Dictionary<string, TkCleanLube>>();
            try
            {
                var datosTkClean = await ObtenerRegistroTkClean(DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"));


                Dictionary<string, TkCleanLube> diccionario = datosTkClean.Detalle.ToDictionary(x => x.Tipo);

                respuesta.Detalle = new Dictionary<string, TkCleanLube>();

                respuesta.Detalle = diccionario;

            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        //public async Task<Respuesta<Dictionary<string, Dictionary<int, ArranqueSincronizacion>>>> ObtenerNumeroArranque1(string idReporte,string tipoConsulta,string fecha)
        //{
        //    Respuesta<Dictionary<string, Dictionary<int, ArranqueSincronizacion>>> respuesta = new Respuesta<Dictionary<string, Dictionary<int, ArranqueSincronizacion>>>();
        //    try
        //    {
        //        var datosOperario = await _function.ObtenerDatosOperario();
        //        using (OracleConnection connection = _conexion.ObtenerConexion())
        //        {
        //            connection.Open();

        //            using (OracleCommand command = new OracleCommand("ObtenerNumeroArranque", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;

        //                command.Parameters.Add("p_IdReporteProduccion", OracleDbType.Varchar2).Value = idReporte;
        //                command.Parameters.Add("p_TipoConsulta", OracleDbType.Varchar2).Value = tipoConsulta;
        //                command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = fecha;
        //                command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

        //                command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //                using (OracleDataAdapter adapter = new OracleDataAdapter(command))
        //                {
        //                    DataTable dataTable = new DataTable();
        //                    adapter.Fill(dataTable);
        //                    Dictionary<string, Dictionary<int, ArranqueSincronizacion>> detalle = new Dictionary<string, Dictionary<int, ArranqueSincronizacion>>();

        //                    foreach (DataRow row in dataTable.Rows)
        //                    {
        //                        int anual = Convert.ToInt32(row["anual"]);
        //                        int mensual = Convert.ToInt32(row["mensual"]);
        //                        int numeroGenerador = Convert.ToInt32(row["numeroGenerador"]);
        //                        string tipo = row["tipo"].ToString();
        //                        string idReporteProduccion = row["idReporteProduccion"].ToString();

        //                        ArranqueSincronizacion arranqueSincronizacion = new ArranqueSincronizacion
        //                        {
        //                            Anual = anual,
        //                            Mensual = mensual,
        //                            NumeroGenerador = numeroGenerador,
        //                            Tipo = tipo,
        //                            IdReporteProduccion = idReporteProduccion
        //                        };

        //                        if (!detalle.ContainsKey(arranqueSincronizacion.Tipo))
        //                        {
        //                            detalle[arranqueSincronizacion.Tipo] = new Dictionary<int, ArranqueSincronizacion>();
        //                        }
        //                        detalle[arranqueSincronizacion.Tipo][arranqueSincronizacion.NumeroGenerador] = arranqueSincronizacion;
        //                    }
        //                    respuesta.Detalle = detalle;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //    return respuesta;
        //}

        public async Task<Respuesta<List<ArranqueSincronizacion>>> ObtenerNumeroArranque(string idReporte, string tipoConsulta, string fecha)
        {
            Respuesta<List<ArranqueSincronizacion>> respuesta = new Respuesta<List<ArranqueSincronizacion>>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerNumeroArranque", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdReporteProduccion", OracleDbType.Varchar2).Value = idReporte;
                        command.Parameters.Add("p_TipoConsulta", OracleDbType.Varchar2).Value = tipoConsulta;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;

                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            respuesta.Detalle  = new List<ArranqueSincronizacion>();
                            ArranqueSincronizacion arranqueSincronizacion = new ArranqueSincronizacion();

                            foreach (DataRow row in dataTable.Rows)
                            {
                                int anual = Convert.ToInt32(row["anual"]);
                                int mensual = Convert.ToInt32(row["mensual"]);
                                int numeroGenerador = Convert.ToInt32(row["numeroGenerador"]);
                                string tipo = row["tipo"].ToString();
                                string idReporteProduccion = row["idReporteProduccion"].ToString();

                                arranqueSincronizacion = new ArranqueSincronizacion
                                {
                                    Anual = anual,
                                    Mensual = mensual,
                                    NumeroGenerador = numeroGenerador,
                                    Tipo = tipo,
                                    IdReporteProduccion = idReporteProduccion,
                                    Fecha = row["fecha"].ToString()
                                };
                                respuesta.Detalle.Add(arranqueSincronizacion);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return respuesta;
        }

        public async Task<Respuesta<Dictionary<string, ManttoVessel>>> ObtenerMantoTKVessel(string fecha)
        {
            Respuesta<Dictionary<string, ManttoVessel>> respuesta = new Respuesta<Dictionary<string, ManttoVessel>>();

            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                try
                {
                    connection.Open();

                    OracleCommand cmd = new OracleCommand("GetManttoVesselData", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = fecha;
                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_Cursor"].Value).GetDataReader();

                    Dictionary<string, ManttoVessel> mantoVessel = new Dictionary<string, ManttoVessel>();
                    ManttoVessel dato = new ManttoVessel();
                    while (reader.Read())
                    {
                        dato = new ManttoVessel()
                        {
                            IdTkManttoVessel = reader["idtkmanttovessel"].ToString(),
                            Fecha = reader["fecha"].ToString(),
                            TipoTk = reader["tipotk"].ToString(),
                            Eg1_Yesterday = decimal.Parse(reader["eg1_yesterday"].ToString()),
                            Eg1_Today = decimal.Parse(reader["eg1_today"].ToString()),
                            Eg2_Yesterday = decimal.Parse(reader["eg2_yesterday"].ToString()),
                            Eg2_Today = decimal.Parse(reader["eg2_today"].ToString())
                        };

                        mantoVessel.Add(dato.TipoTk, dato);
                    }


                    respuesta.Detalle = new Dictionary<string, ManttoVessel>();
                    respuesta.Detalle = mantoVessel;
                }
                catch (Exception ex)
                {
                }
            }
            return respuesta;
        }


        public async Task<Respuesta<List<ReporteProduccionStatus>>> ObtenerDatosProduccionStatus(string fecha, string tipoConsulta)
        {
            Respuesta<List<ReporteProduccionStatus>> respuesta = new Respuesta<List<ReporteProduccionStatus>>();
            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                try
                {
                    var datosOperario = await _function.ObtenerDatosOperario();

                    connection.Open();

                    OracleCommand cmd = new OracleCommand("ObtenerDatosReporteProdStatus", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                    cmd.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                    cmd.Parameters.Add("p_TipoConsulta", OracleDbType.Varchar2).Value = tipoConsulta;
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_Cursor"].Value).GetDataReader();

                    respuesta.Detalle = new List<ReporteProduccionStatus>();
                    ReporteProduccionStatus dato = new ReporteProduccionStatus();
                    while (reader.Read())
                    {
                        dato = new ReporteProduccionStatus()
                        {
                            IdReporteProdStatus = reader["IdReporteProdStatus"].ToString(),
                            NumeroGenerador = int.Parse(reader["NumeroGenerador"].ToString()),
                            ServiceAccumulated = decimal.Parse(reader["ServiceAccumulated"].ToString()),
                            PlannedMainten = decimal.Parse(reader["PlannedMainten"].ToString()),
                            ForcedMaintMech = decimal.Parse(reader["ForcedMaintMech"].ToString()),
                            ForcedMaintElec = decimal.Parse(reader["ForcedMaintElec"].ToString()),
                            ForcedAuxiliaries = decimal.Parse(reader["ForcedAuxiliaries"].ToString()),
                            ExternalTrips = decimal.Parse(reader["ExternalTrips"].ToString()),
                            StandBy = decimal.Parse(reader["StandBy"].ToString()),
                            RunningHours = decimal.Parse(reader["RunningHours"].ToString()),
                            HoursAvailable = decimal.Parse(reader["HoursAvailable"].ToString()),
                            IdReporteProduccion = reader["IdReporteProduccion"].ToString(),
                            Fecha = reader["fecha"].ToString()

                        };

                        respuesta.Detalle.Add(dato);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return respuesta;
        }


        public async Task<Respuesta<List<ReporteProducion>>> ObtenerDatosProduccion(string fecha, string tipoConsulta)
        {
            Respuesta<List<ReporteProducion>> respuesta = new Respuesta<List<ReporteProducion>>();
            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                try
                {
                    var datosOperario = await _function.ObtenerDatosOperario();

                    connection.Open();

                    OracleCommand cmd = new OracleCommand("ConsultarReporteProduccion", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                    cmd.Parameters.Add("p_tipoConsulta", OracleDbType.Varchar2).Value = tipoConsulta;
                    cmd.Parameters.Add("p_sitio", OracleDbType.Varchar2).Value = datosOperario.IdSitio;
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_Cursor"].Value).GetDataReader();

                    respuesta.Detalle = new List<ReporteProducion>();
                    ReporteProducion dato = new ReporteProducion();
                    while (reader.Read())
                    {
                        dato = new ReporteProducion()
                        {
                            IdReporteProduccion = reader["IdReporteProduccion"].ToString(),
                            Operadores = reader["Operadores"].ToString(),
                            TotalPlantGen = decimal.Parse(reader["TotalPlantGen"].ToString()),
                            TotalPlantExportION = decimal.Parse(reader["TotalPlantExportION"].ToString()),
                            TotalGasConsumption = decimal.Parse(reader["TotalGasConsumption"].ToString()),
                            Efficiency = decimal.Parse(reader["Efficiency"].ToString()),
                            ConsumoServiciosAuxiliares = decimal.Parse(reader["ConsumoServiciosAuxiliares"].ToString()),
                            Fecha = reader["fecha"].ToString()

                        };

                        respuesta.Detalle.Add(dato);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return respuesta;
        }
    }
}
