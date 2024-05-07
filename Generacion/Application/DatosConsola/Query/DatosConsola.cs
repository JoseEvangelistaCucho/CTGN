using Generacion.Application.DataBase;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using Generacion.Models.DatosConsola;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Globalization;
using Generacion.Application.Funciones;

namespace Generacion.Application.DatosConsola.Query
{
    public class DatosConsola
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosConsola(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }
        // AQUI TRAE DATA DE LA TABLA DET_CONSOLA DONDE LO ORDENA SEGUN LA FILE DE FOMRA DESCENDIENTE
        public async Task<Respuesta<Dictionary<string, List<DatosFormatoConsola>>>> ObtenerRegistroDeConsola(string fechaInicio, string fechaFin, string idSitio)
        {
            Respuesta<Dictionary<string, List<DatosFormatoConsola>>> respuesta = new Respuesta<Dictionary<string, List<DatosFormatoConsola>>>();
            Dictionary<string, List<DatosFormatoConsola>> datosFormatoConsola = new Dictionary<string, List<DatosFormatoConsola>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"SELECT
                                            IdDetalleConsola, P, Q, E, EQ, IL1, IL2,
                                            IL3, U12, U23, U31, Hora, fila,fecha
                                        FROM
                                            tbl_Det_Consola
                                        WHERE
                                            fecha = TO_DATE(:fechaInicio, 'DD/MM/YY HH24:MI') 
                                            ORDER BY  fila asc";
                    /*"SELECT
                                            IdDetalleConsola, P, Q, E, EQ, IL1, IL2,
                                            IL3, U12, U23, U31, Hora, fila,fecha
                                        FROM
                                            tbl_Det_Consola
                                        WHERE
                                            fecha BETWEEN
                                            TO_DATE(:fechaInicio, 'DD/MM/YY HH24:MI') AND TO_DATE(:fechaFin, 'DD/MM/YY HH24:MI')
                                           and (fila <> 24 and fecha = TO_DATE(:fechaInicio, 'DD/MM/YY HH24:MI')) 
                                            or (fila >= 24 and fecha = TO_DATE(:fechaFin, 'DD/MM/YY HH24:MI'))    
                                            ORDER BY  fila asc";*/
                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {

                        command.Parameters.Add(":fechaInicio", OracleDbType.Varchar2).Value = fechaInicio;
                        //command.Parameters.Add(":fechaFin", OracleDbType.Varchar2).Value = fechaFin;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<DatosFormatoConsola> listaRegistroConsola = new List<DatosFormatoConsola>();
                            DatosFormatoConsola registroConsola = new DatosFormatoConsola();
                            while (reader.Read())
                            {
                                registroConsola = new DatosFormatoConsola();
                                registroConsola.IdDetalleConsola = reader["IdDetalleConsola"].ToString();
                                registroConsola.PotenciaActiva = decimal.Parse(reader["P"].ToString());
                                registroConsola.PotenciaReactiva = decimal.Parse(reader["Q"].ToString());
                                registroConsola.EnergiaActiva = decimal.Parse(reader["E"].ToString());
                                registroConsola.EnergiaReactiva = decimal.Parse(reader["EQ"].ToString());
                                registroConsola.CorrienteLinea1 = decimal.Parse(reader["IL1"].ToString());
                                registroConsola.CorrienteLinea2 = decimal.Parse(reader["IL2"].ToString());
                                registroConsola.CorrienteLinea3 = decimal.Parse(reader["IL3"].ToString());
                                registroConsola.Voltaje = decimal.Parse(reader["U12"].ToString());
                                registroConsola.Voltaje23 = decimal.Parse(reader["U23"].ToString());
                                registroConsola.Voltaje31 = decimal.Parse(reader["U31"].ToString());
                                registroConsola.Hora = reader["Hora"].ToString();
                                registroConsola.Fila = int.Parse(reader["Fila"].ToString());

                                listaRegistroConsola.Add(registroConsola);

                            }
                            // Obtener la primera línea para ID "BAA901"
                            var primeraLineaBAA = listaRegistroConsola
                                .Where(x => x.IdDetalleConsola.StartsWith($"{idSitio}-BAA901"))
                                .ToList();

                            // Obtener la primera línea para ID "EG01"
                            var primeraLineaEG1 = listaRegistroConsola
                                .Where(x => x.IdDetalleConsola.StartsWith($"{idSitio}-EG01"))
                                .ToList();

                            // Obtener la primera línea para ID "EG02"
                            var primeraLineaEG2 = listaRegistroConsola
                                .Where(x => x.IdDetalleConsola.StartsWith($"{idSitio}-EG02"))
                                .ToList();

                            // Obtener la primera línea para ID "EG01"
                            var primeraLineaBFA = listaRegistroConsola
                                .Where(x => x.IdDetalleConsola.StartsWith($"{idSitio}-BFA901"))
                                .ToList();

                            primeraLineaBAA = primeraLineaBAA
                                .OrderBy(h => h.Fila)
                                .ToList();


                            primeraLineaEG1 = primeraLineaEG1
                                .OrderBy(h => h.Fila)
                                .ToList();

                            primeraLineaEG2 = primeraLineaEG2
                                .OrderBy(h => h.Fila)
                                .ToList();

                            primeraLineaBFA = primeraLineaBFA
                                .OrderBy(h => h.Fila)
                                .ToList();


                            datosFormatoConsola.Add("BAA901", primeraLineaBAA);
                            datosFormatoConsola.Add("EG01", primeraLineaEG1);
                            datosFormatoConsola.Add("BFA901", primeraLineaBFA);
                            datosFormatoConsola.Add("EG02", primeraLineaEG2);

                            respuesta.Detalle = datosFormatoConsola;
                            respuesta.IdRespuesta = 0;
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
        public async Task<Respuesta<List<TiposDeRegistroConsola>>> ObtenerTiposDeRegistro()
        {
            Respuesta<List<TiposDeRegistroConsola>> respuesta = new Respuesta<List<TiposDeRegistroConsola>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"select IdRegistroConsola,TipoRegistro,Descripcion  
                                        from tbl_Registro_Consola";

                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<TiposDeRegistroConsola> tiposDeRegistroConsola = new List<TiposDeRegistroConsola>();
                            TiposDeRegistroConsola registroConsola = new TiposDeRegistroConsola();
                            while (reader.Read())
                            {
                                registroConsola = new TiposDeRegistroConsola();
                                registroConsola.IdRegistroConsola = reader["IdRegistroConsola"].ToString();
                                registroConsola.TipoRegistro = reader["TipoRegistro"].ToString();
                                registroConsola.Descripcion = reader["Descripcion"].ToString();
                                tiposDeRegistroConsola.Add(registroConsola);
                            }
                            respuesta.Detalle = new List<TiposDeRegistroConsola>();
                            respuesta.Detalle = tiposDeRegistroConsola;
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
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();

            }

            return respuesta;
        }

        public async Task<Respuesta<Dictionary<string, CabecerasTabla>>> ObtenerCabecerasDeTabla()
        {

            Respuesta<Dictionary<string, CabecerasTabla>> respuesta = new Respuesta<Dictionary<string, CabecerasTabla>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"select IdTipoEngine,Detalle,Descripcion,TablaReference from tbl_Tipo_Cabecera";

                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            Dictionary<string, CabecerasTabla> keyValuesHeaders = new Dictionary<string, CabecerasTabla>();
                            CabecerasTabla cabecerasTabla = new CabecerasTabla();
                            while (reader.Read())
                            {
                                cabecerasTabla = new CabecerasTabla();
                                cabecerasTabla.IdTipoEngine = reader["IdTipoEngine"].ToString();
                                cabecerasTabla.Detalle = reader["Detalle"].ToString();
                                cabecerasTabla.Descripcion = reader["Descripcion"].ToString();
                                cabecerasTabla.TablaReference = reader["TablaReference"].ToString();

                                keyValuesHeaders.Add(cabecerasTabla.IdTipoEngine, cabecerasTabla);
                            }
                            respuesta.Detalle = new Dictionary<string, CabecerasTabla>();
                            respuesta.Detalle = keyValuesHeaders;

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
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }

        public async Task<Respuesta<Dictionary<string, List<RegistrosDatosGenerator>>>> ObtenerDetalleGenerador(string fecha, string fechaFin, string idSitio)
        {
            Respuesta<Dictionary<string, List<RegistrosDatosGenerator>>> respuesta = new Respuesta<Dictionary<string, List<RegistrosDatosGenerator>>>();
            Dictionary<string, List<RegistrosDatosGenerator>> datosGenerator = new Dictionary<string, List<RegistrosDatosGenerator>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerGenPorFecha", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_Fecha", OracleDbType.Varchar2, fecha, System.Data.ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("p_FechaFin", OracleDbType.Varchar2, fechaFin, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<RegistrosDatosGenerator> registrosDatosGenerators = new List<RegistrosDatosGenerator>();
                            RegistrosDatosGenerator registrosDatosGenerator = new RegistrosDatosGenerator();
                            registrosDatosGenerator.detalleGeneradores = new Dictionary<string, DetGeneratorTC>();

                            while (reader.Read())
                            {
                                registrosDatosGenerator = new RegistrosDatosGenerator();
                                registrosDatosGenerator.Fecha = reader["Fecha"].ToString();
                                registrosDatosGenerator.Hora = reader["Hora"].ToString();
                                registrosDatosGenerator.L1 = int.Parse(reader["L1"].ToString());
                                registrosDatosGenerator.L2 = int.Parse(reader["L2"].ToString());
                                registrosDatosGenerator.L3 = int.Parse(reader["L3"].ToString());
                                registrosDatosGenerator.D = int.Parse(reader["D"].ToString());
                                registrosDatosGenerator.ND = int.Parse(reader["ND"].ToString());
                                registrosDatosGenerator.TersionalVibration = double.Parse(reader["TersionalVibration"].ToString());
                                registrosDatosGenerator.IdFormatoConsola = reader["IdFormatoConsola"].ToString();
                                registrosDatosGenerator.IdDetGeneratorConsola = reader["IdDetGeneratorConsola"].ToString();

                                registrosDatosGenerator.detalleGeneradores = await ObtenerDetalleTC(registrosDatosGenerator.IdDetGeneratorConsola);
                                registrosDatosGenerators.Add(registrosDatosGenerator);
                            }
                            //DateTime fechaComparar = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                           // registrosDatosGenerators.RemoveAll(item => item.Fecha == fechaComparar.Date.ToString() && item.Hora == "0:00");

                           // registrosDatosGenerators = registrosDatosGenerators.OrderBy(h => TimeSpan.Parse(h.Hora)).ToList();
                            registrosDatosGenerators = registrosDatosGenerators.OrderBy(h => int.Parse(h.Hora.Split(':')[0])).ToList();


                            var generator1 = registrosDatosGenerators
                                .Where(x => x.IdDetGeneratorConsola.StartsWith($"{idSitio}-Generator1"))
                                .ToList();

                            var generator2 = registrosDatosGenerators
                                .Where(x => x.IdDetGeneratorConsola.StartsWith($"{idSitio}-Generator2"))
                                .ToList();

                            datosGenerator.Add("Generator1", generator1);
                            datosGenerator.Add("Generator2", generator2);

                            respuesta.Detalle = datosGenerator;
                            if (!command.Parameters["p_resultado"].Value.Equals(DBNull.Value))
                            {
                                OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                                respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
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
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }

        public async Task<Dictionary<string, DetGeneratorTC>> ObtenerDetalleTC(string id)
        {
            Dictionary<string, DetGeneratorTC> respuesta = new Dictionary<string, DetGeneratorTC>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerGenPorIdConsola", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_IdDetGeneratorConsola", OracleDbType.Varchar2, id, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            DetGeneratorTC detGenerator = new DetGeneratorTC();

                            while (reader.Read())
                            {
                                detGenerator = new DetGeneratorTC();
                                detGenerator.IdDetGenerator = reader["IdDetGenerator"].ToString();
                                detGenerator.Fecha = reader["fecha"].ToString();
                                detGenerator.IdTipoEngine = reader["idtipoengine"].ToString();
                                detGenerator.TempOut = int.Parse(reader["tempout"].ToString());
                                detGenerator.Speed = int.Parse(reader["speed"].ToString());
                                detGenerator.IdDetGeneratorConsola = reader["IdDetGeneratorConsola"].ToString();

                                respuesta.Add(detGenerator.IdTipoEngine, detGenerator);
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

        public async Task<Respuesta<Dictionary<string, List<RegistrosDatosEngine>>>> ObtenerDatosEngine(string fecha, string fechaFin, string idSitio)
        {
            Respuesta<Dictionary<string, List<RegistrosDatosEngine>>> respuesta = new Respuesta<Dictionary<string, List<RegistrosDatosEngine>>>();
            Dictionary<string, List<RegistrosDatosEngine>> detalle = new Dictionary<string, List<RegistrosDatosEngine>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerEngPorFecha", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_Fecha", OracleDbType.Varchar2, fecha, System.Data.ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("p_FechaFin", OracleDbType.Varchar2, fechaFin, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<RegistrosDatosEngine> registrosDatosEngines = new List<RegistrosDatosEngine>();
                            RegistrosDatosEngine registros = new RegistrosDatosEngine();
                            registros.detalleEngine = new Dictionary<string, DetalleEngine>();

                            while (reader.Read())
                            {
                                registros = new RegistrosDatosEngine();
                                registros.IdDetEngineConsola = reader["IdDetEngineConsola"].ToString();
                                registros.Hora = reader["Hora"].ToString();
                                registros.RunHours = double.Parse(reader["RunHours"].ToString());
                                registros.CATemp = double.Parse(reader["CaTemp"].ToString());
                                registros.DiffPressJetPulse = double.Parse(reader["DiffPressJetPulse"].ToString());
                                registros.CAPress = double.Parse(reader["CAPress"].ToString());
                                registros.ExhGasAvgTemp = double.Parse(reader["ExhGasAvgTemp"].ToString());
                                registros.CylPressAvg = double.Parse(reader["CylPressAvg"].ToString());
                                registros.Fecha = reader["Fecha"].ToString();
                                registros.IdFormatoConsola = reader["IdFormatoConsola"].ToString();

                                registros.detalleEngine = await ObtenerDetalleEng(registros.IdDetEngineConsola);
                                registrosDatosEngines.Add(registros);
                            }
/*
                            DateTime fechaComparar = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            registrosDatosEngines.RemoveAll(item => item.Fecha == fechaComparar.Date.ToString() && item.Hora == "0:00");
                            */

                            registrosDatosEngines = registrosDatosEngines.OrderBy(h => int.Parse(h.Hora.Split(':')[0])).ToList();

                            var datosEngine1 = registrosDatosEngines
                                .Where(x => x.IdDetEngineConsola.StartsWith($"{idSitio}-Engine1"))
                                .ToList();

                            var datosEngine2 = registrosDatosEngines
                                .Where(x => x.IdDetEngineConsola.StartsWith($"{idSitio}-Engine2"))
                                .ToList();


                            detalle.Add("Engine1", datosEngine1);
                            detalle.Add("Engine2", datosEngine2);

                            respuesta.Detalle = detalle;
                            if (!command.Parameters["p_resultado"].Value.Equals(DBNull.Value))
                            {
                                OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                                respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
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
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }


        public async Task<Dictionary<string, DetalleEngine>> ObtenerDetalleEng(string id)
        {
            Dictionary<string, DetalleEngine> respuesta = new Dictionary<string, DetalleEngine>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerDetEgine", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_IdDetEngineConsola", OracleDbType.Varchar2, id, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            DetalleEngine detalleEngine = new DetalleEngine();

                            while (reader.Read())
                            {
                                detalleEngine = new DetalleEngine();
                                detalleEngine.IdDetEngine = reader["IdDetEngine"].ToString();
                                detalleEngine.IdTipoEngine = reader["IdTipoEngine"].ToString();
                                detalleEngine.Fecha = reader["Fecha"].ToString();
                                detalleEngine.Presion = double.Parse(reader["Presion"].ToString());
                                detalleEngine.Temp = double.Parse(reader["Temp"].ToString());
                                detalleEngine.IdDetEngineConsola = reader["IdDetEngineConsola"].ToString();

                                respuesta.Add(detalleEngine.IdTipoEngine, detalleEngine);
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

        //public async Task<Dictionary<int, LecturasMedianoche>> ObtenerLecturaMediaNoche(string id,string fecha)
        public async Task<List<LecturasMedianoche>> ObtenerLecturaMediaNoche(string id,string fecha)
        {
            //Dictionary<int, LecturasMedianoche> respuesta = new Dictionary<int, LecturasMedianoche>();
            List<LecturasMedianoche> respuesta = new List<LecturasMedianoche>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerLectMed", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_IdFormatoConsola", OracleDbType.Varchar2, id, System.Data.ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("p_fecha", OracleDbType.Varchar2, fecha, System.Data.ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("p_sitio", OracleDbType.Varchar2, datosOperario.IdSitio, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            LecturasMedianoche lecturas = new LecturasMedianoche();

                            while (reader.Read())
                            {
                                lecturas = new LecturasMedianoche();
                                lecturas.IdLecturas = reader["IdLecturas"].ToString();
                                lecturas.NumeroEG = int.Parse(reader["NumeroEG"].ToString());
                                lecturas.GasconsumedEG = decimal.Parse(reader["GasconsumedEG"].ToString());
                                lecturas.GasEnergiaEG = decimal.Parse(reader["GasEnergiaEG"].ToString());
                                lecturas.ReadingToday = decimal.Parse(reader["ReadingToday"].ToString());
                                lecturas.IdFormatoConsola = reader["IdFormatoConsola"].ToString();
                                lecturas.fecha = reader["Fecha"].ToString();

                                //respuesta.Add(lecturas.NumeroEG, lecturas);
                                respuesta.Add(lecturas);
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

        public async Task<Respuesta<List<OutGoingFeeder>>> ObtenerDetalleBAO(string id, string fecha)
        {
            Respuesta<List<OutGoingFeeder>> respuesta = new Respuesta<List<OutGoingFeeder>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    var datosOperario = await _function.ObtenerDatosOperario();
                    using (OracleCommand command = new OracleCommand("proc_ObtenerDetalleBAO", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_IdTipoEngine", OracleDbType.Varchar2, id.ToUpper(), System.Data.ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("p_Fecha", OracleDbType.Varchar2, fecha, System.Data.ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("p_Sitio", OracleDbType.Varchar2, datosOperario.IdSitio, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<OutGoingFeeder> datos = new List<OutGoingFeeder>();
                            OutGoingFeeder outGoing = new OutGoingFeeder();

                            while (reader.Read())
                            {
                                outGoing = new OutGoingFeeder();
                                outGoing.IdOutGoing = reader["IdOutGoing"].ToString();
                                outGoing.IdTipoEngine = reader["IdTipoEngine"].ToString();
                                outGoing.KWh = decimal.Parse(reader["kWh"].ToString());
                                outGoing.KVARh = decimal.Parse(reader["kVARh"].ToString());
                                outGoing.Hora = reader["Hora"].ToString();
                                outGoing.IdFormatoConsola = reader["IdFormatoConsola"].ToString();
                                outGoing.Fila = int.Parse(reader["Fila"].ToString());
                                outGoing.Fecha = reader["Fecha"].ToString();

                                datos.Add(outGoing);
                            }

                            datos = datos.OrderBy(h => h.Fila).ToList();
                            respuesta.Detalle = datos;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return respuesta;
        }
        public async Task<Respuesta<FormatoConsola>> ObtenerFormatoConsola(string id)
        {
            Respuesta<FormatoConsola> respuesta = new Respuesta<FormatoConsola>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ConsFormatoConsola", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdFormatoConsola", OracleDbType.Varchar2).Value = id;
                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            FormatoConsola formatoConsola = new FormatoConsola();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                formatoConsola = new FormatoConsola();
                                formatoConsola.IdFormatoConsola = row["IdFormatoConsola"].ToString();
                                formatoConsola.Observaciones = row["Observaciones"].ToString();
                                formatoConsola.Fecha = row["Fecha"].ToString();
                            }

                            respuesta.Detalle = formatoConsola;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Respuesta<List<decimal>>> ObtenerDatosDetConsola(string fechaInicio, string fechaFin, string idDetalleConsola, int fila)
        {
            Respuesta<List<decimal>> respuesta = new Respuesta<List<decimal>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerDatosDetConsola", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_fechaInicio", OracleDbType.Varchar2).Value = fechaInicio;
                        command.Parameters.Add("p_fechaFin", OracleDbType.Varchar2).Value = fechaFin;
                        command.Parameters.Add("p_IdDetalleConsola", OracleDbType.Varchar2).Value = idDetalleConsola;
                        command.Parameters.Add("p_fila", OracleDbType.Int32).Value = fila;

                        command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            List<decimal> datos = new List<decimal>();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                decimal valorBFA = decimal.Parse(row["E"].ToString());
                                int filadb = int.Parse(row["fila"].ToString());
                                string fecha = row["Fecha"].ToString();

                                datos.Add(valorBFA);
                            }

                            respuesta.Detalle = datos;
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
