using Generacion.Application.ION.Query;
using lecturaCampo = Generacion.Application.LecturaCampo.Query.LecturaCampo;
using Generacion.Application.ReporteProduccion.Query;
using Generacion.Models;
using MediatR;
using Generacion.Models.ReporteProduccion;
using Generacion.Models.ION;
using Generacion.Application.ReporteDiarioGAS.Query;
using Generacion.Models.ReporteDiarioGAS;
using Generacion.Models.ReporteRAM;
using System.Drawing;
using System.Collections.Generic;

namespace Generacion.Application.RAM.Query
{
    public class DatosProduccionQuery : IRequest<Respuesta<Dictionary<string, object>>>
    {
        public string Fecha { get; set; }
        public string TipoBusqueda { get; set; }
    }
    public class DatosProduccionHandler : IRequestHandler<DatosProduccionQuery, Respuesta<Dictionary<string, object>>>
    {
        private readonly IMediator _sender;
        private readonly ConsultarProduccion _consultarProduccion;
        private readonly ConsultarION _consultarION;
        private readonly lecturaCampo _lecturaCampo;
        private readonly ObtenerDatosReporteGAS _obtenerDatosReporteGAS;
        private readonly ObtenerRegistrosRAM _obtenerRegistrosRAM;

        public DatosProduccionHandler(ObtenerRegistrosRAM obtenerRegistrosRAM, IMediator sender, ConsultarProduccion consultarProduccion, ConsultarION consultarION, lecturaCampo lecturaCampo, ObtenerDatosReporteGAS obtenerDatosReporteGAS)
        {
            _obtenerRegistrosRAM = obtenerRegistrosRAM;
            _lecturaCampo = lecturaCampo;
            _consultarION = consultarION;
            _consultarProduccion = consultarProduccion;
            _sender = sender;
            _obtenerDatosReporteGAS = obtenerDatosReporteGAS;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(DatosProduccionQuery request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();
            try
            {
                //PRODUCCIÓN JUNIO MWh			
                //se tiene que restar para obtener Gen.1 act.energy MWh y Gen.2 act.energy MWh (obtener  con todo y decimales)
                //adicional a eso se deben de sumar ambos GE par obtener el total 
                //--TRAE CORRECTAMENTE LOS DATOS , FALTA VALIDAR LOS DATOS DEL REPORTE DE PRODUCCION
                /* var datosGeneralesProduccion2 = await _sender.Send(new ObtenerDatosProduccion()
                 {
                     Fecha = request.Fecha.ToString("yyyy")
                 });**/
                Respuesta<List<CityGateFlow>> cityGateYesterday = await _consultarProduccion.ObtenerRegistroCityGate(request.Fecha, request.TipoBusqueda, "READING YESTERDAY");
                Respuesta<List<CityGateFlow>> cityGateToday = await _consultarProduccion.ObtenerRegistroCityGate(request.Fecha, request.TipoBusqueda, "READING TODAY");


                Respuesta<List<EnergiaProducida>> datosGeneralesProduccion = await _consultarProduccion.ObtenerRegistroProduccion(request.Fecha, "ENERGY PRODUCED", request.TipoBusqueda);

                Dictionary<DateTime, List<EnergiaProducida>> energiaProducidaPorFecha =
                    datosGeneralesProduccion.Detalle
                    .GroupBy(x => new DateTime(DateTime.Parse(x.Fecha).Year, DateTime.Parse(x.Fecha).Month, 1))
                    .ToDictionary(
                        outerGroup => outerGroup.Key,
                        outerGroup => outerGroup
                                    .OrderBy(y => DateTime.Parse(y.Fecha))
                                    .ToList()
                    );


                //Horas de Operación	
                Respuesta<List<ReporteProduccionStatus>> detalleProduccion = await _consultarProduccion.ObtenerDatosProduccionStatus(request.Fecha, request.TipoBusqueda);
                var datosDetalleProduccion = await ObtenerDetalleProduccionPorNumeroGenerador(detalleProduccion.Detalle);

                //ION 

                Respuesta<List<ReporteProducion>> datosION = await _consultarProduccion.ObtenerDatosProduccion(request.Fecha, request.TipoBusqueda);

                /*  Respuesta<List<DatosFormatoMGD>> datosION = await _sender.Send(new ObtenerDatosIONQuery()
                  {
                      Fecha = request.Fecha
                  });**/

                //GAS

                Respuesta<List<DetalleReporteGas>> detalleReporteGas = await _obtenerDatosReporteGAS.ObtenerDetallesReportePorAño(request.Fecha);

                List<DetalleReporteGas> lineaDos = detalleReporteGas.Detalle
                    .OrderBy(y => DateTime.Parse(y.Fecha))
                    .Where(x => x.Hora.StartsWith("18"))
                    .ToList();

                lineaDos = lineaDos.OrderBy(x => DateTime.Parse(x.Fecha)).ToList();


                //ACEITE	
                Respuesta<List<LevelLubeOilCartel>> datosProduccionCarter = await _consultarProduccion.ObtenerRegistroLevelCartel(request.Fecha, request.TipoBusqueda, "");

                Dictionary<string, List<LevelLubeOilCartel>> datosHoyYAdd =
                   datosProduccionCarter.Detalle
                       .GroupBy(x => x.TipoCarter)
                       .ToDictionary(
                           innerGroup => innerGroup.Key,
                           innerGroup => innerGroup
                               .OrderBy(y => DateTime.Parse(y.Fecha))
                               .ToList()
                       );

                Respuesta<List<DBRAM>> respuestaRAM = await _obtenerRegistrosRAM.ObtenerRegistroRam(request.Fecha, request.TipoBusqueda);

                string nombreMes = DateTime.Now.ToString("MMMM", new System.Globalization.CultureInfo("es-ES"));

                Dictionary<string, DBRAM> datosRamPorMes = respuestaRAM.Detalle.ToDictionary(x => DateTime.Parse(x.Fecha).ToString("MMMM", new System.Globalization.CultureInfo("es-ES")));

                Respuesta<List<ArranqueSincronizacion>> respuestaArranque = await _consultarProduccion.ObtenerNumeroArranque(string.Empty, request.TipoBusqueda, request.Fecha);

                 var datosSincroAgrupados =
                    respuestaArranque.Detalle
                    .GroupBy(x => new DateTime(DateTime.Parse(x.Fecha).Year, DateTime.Parse(x.Fecha).Month, 1))
                    .ToDictionary(
                        outerGroup => outerGroup.Key,
                        outerGroup => outerGroup
                                    .OrderBy(y => DateTime.Parse(y.Fecha))
                                    .ToList()
                    );

                Dictionary<string, Dictionary<int, Dictionary<string, ArranqueSincronizacion>>> datosSincro = await ObtenerDetalleSincronizacionPorNumeroGenerador(respuestaArranque.Detalle);

                Respuesta<List<ViewOIL>> respuestaRAMOil = await _obtenerRegistrosRAM.ObtenerRegistroRamOil();

                Dictionary<int, Dictionary<DateTime, Dictionary<int, Dictionary<int, Dictionary<string, ViewOIL>>>>> datosVistaOil = await ObtenerDatosOilPorFechaPosicion(respuestaRAMOil.Detalle);

                Dictionary<int, Dictionary<int, Dictionary<string, List<decimal>>>> datosGraficosOil =
                                                respuestaRAMOil.Detalle
                                                    .GroupBy(x => x.Posicion)
                                                    .ToDictionary(
                                                        grupo1 => grupo1.Key,
                                                        grupo1 => grupo1
                                                            .GroupBy(x => x.NumeroGe)
                                                            .ToDictionary(
                                                                grupo2 => grupo2.Key,
                                                                grupo2 => grupo2
                                                                    .GroupBy(x => x.IdTipoEngine)
                                                                    .ToDictionary(
                                                                        grupo3 => grupo3.Key,
                                                                        grupo3 => grupo3.Select(x => x.Detalle).ToList()
                                                                    )
                                                            )
                                                    );

                respuesta.Detalle = new Dictionary<string, object>();
                respuesta.Detalle.Add("datosGeneralesProduccion", energiaProducidaPorFecha);
                respuesta.Detalle.Add("datosDetalleProduccion", datosDetalleProduccion);
                respuesta.Detalle.Add("datosION", datosION.Detalle);
                respuesta.Detalle.Add("reporteGas", lineaDos);
                respuesta.Detalle.Add("datosAceite", datosHoyYAdd);
                respuesta.Detalle.Add("cityGateYesterday", cityGateYesterday.Detalle);
                respuesta.Detalle.Add("cityGateToday", cityGateToday.Detalle);
                respuesta.Detalle.Add("datosRam", respuestaRAM.Detalle);
                respuesta.Detalle.Add("datosRamPorMes", datosRamPorMes);
                respuesta.Detalle.Add("datosArranque", datosSincro);
                respuesta.Detalle.Add("datosVistaOil", datosVistaOil);
                respuesta.Detalle.Add("datosGraficosOil", datosGraficosOil);

            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }



        public async Task<Dictionary<int, Dictionary<DateTime, Dictionary<int, Dictionary<int, Dictionary<string, ViewOIL>>>>>> ObtenerDatosOilPorFechaPosicion(List<ViewOIL> datos)
        {
            Dictionary<int, Dictionary<DateTime, Dictionary<int, Dictionary<int, Dictionary<string, ViewOIL>>>>> datosVistaOil =
                     new Dictionary<int, Dictionary<DateTime, Dictionary<int, Dictionary<int, Dictionary<string, ViewOIL>>>>>();

            foreach (var item in datos)
            {
                int year = DateTime.Parse(item.Fecha).Year;

                if (!datosVistaOil.ContainsKey(year))
                {
                    datosVistaOil[year] = new Dictionary<DateTime, Dictionary<int, Dictionary<int, Dictionary<string, ViewOIL>>>>();
                }

                if (!datosVistaOil[year].ContainsKey(DateTime.Parse(item.Fecha)))
                {
                    datosVistaOil[year][DateTime.Parse(item.Fecha)] = new Dictionary<int, Dictionary<int, Dictionary<string, ViewOIL>>>();
                }

                if (!datosVistaOil[year][DateTime.Parse(item.Fecha)].ContainsKey(item.Posicion))
                {
                    datosVistaOil[year][DateTime.Parse(item.Fecha)][item.Posicion] = new Dictionary<int, Dictionary<string, ViewOIL>>();
                }

                if (!datosVistaOil[year][DateTime.Parse(item.Fecha)][item.Posicion].ContainsKey(item.NumeroGe))
                {
                    datosVistaOil[year][DateTime.Parse(item.Fecha)][item.Posicion][item.NumeroGe] = new Dictionary<string, ViewOIL>();
                }

                datosVistaOil[year][DateTime.Parse(item.Fecha)][item.Posicion][item.NumeroGe][item.IdTipoEngine] = item;
            }
            return datosVistaOil;
        }



        public async Task<Dictionary<string, Dictionary<int, Dictionary<string, ArranqueSincronizacion>>>> ObtenerDetalleSincronizacionPorNumeroGenerador(List<ArranqueSincronizacion> arranque)
        {
            var datosSincroAgrupados =
                arranque
                .GroupBy(x => new DateTime(DateTime.Parse(x.Fecha).Year, DateTime.Parse(x.Fecha).Month, 1))
                .ToDictionary(
                    outerGroup => outerGroup.Key.ToString("MMMM", new System.Globalization.CultureInfo("es-ES")),
                    outerGroup =>
                        outerGroup
                        .GroupBy(innerGroup => innerGroup.NumeroGenerador)
                        .ToDictionary(
                            innerGroup => innerGroup.Key,
                            innerGroup =>
                                innerGroup
                                .GroupBy(x => x.Tipo)
                                .ToDictionary(
                                    x => x.Key,
                                    x => x.OrderByDescending(y => DateTime.Parse(y.Fecha)).First()
                                )
                        )
                );

            return datosSincroAgrupados;
        }




        public async Task<Dictionary<string, Dictionary<int, ArranqueSincronizacion>>> ObtenerDetalleSincronizacionPorNumeroGenerador1(List<ArranqueSincronizacion> arranque)
        {

            var datosSincroAgrupados =
                  arranque
                  .GroupBy(x => new DateTime(DateTime.Parse(x.Fecha).Year, DateTime.Parse(x.Fecha).Month, 1))
                  .ToDictionary(
                      outerGroup => outerGroup.Key,
                      outerGroup => outerGroup
                                  .OrderBy(y => DateTime.Parse(y.Fecha))
                                  .ToList()
                  );
            Dictionary<string, Dictionary<int, ArranqueSincronizacion>> datosSincro = new Dictionary<string, Dictionary<int, ArranqueSincronizacion>>();
            try
            {
                foreach (var item in datosSincroAgrupados.Values)
                {

                    ArranqueSincronizacion ultimoRegistroGe1 = item.Where(x => x.NumeroGenerador.Equals(1)).OrderByDescending(x => DateTime.Parse(x.Fecha)).FirstOrDefault();
                    ArranqueSincronizacion ultimoRegistroGe2 = item.Where(x => x.NumeroGenerador.Equals(2)).OrderByDescending(x => DateTime.Parse(x.Fecha)).FirstOrDefault();
                    string nombreMesSincro = DateTime.Parse(ultimoRegistroGe1.Fecha).ToString("MMMM", new System.Globalization.CultureInfo("es-ES"));

                    if (!datosSincro.ContainsKey(nombreMesSincro))
                    {
                        datosSincro.Add(nombreMesSincro, new Dictionary<int, ArranqueSincronizacion>());
                    }

                    datosSincro[nombreMesSincro].Add(ultimoRegistroGe1.NumeroGenerador, ultimoRegistroGe1);
                    datosSincro[nombreMesSincro].Add(ultimoRegistroGe2.NumeroGenerador, ultimoRegistroGe2);

                }
            }
            catch (Exception ex)
            {

            }

            return datosSincro;
        }


        public async Task<Dictionary<DateTime, Dictionary<int, List<ReporteProduccionStatus>>>> ObtenerDetalleProduccionPorNumeroGenerador(List<ReporteProduccionStatus> obtenerDatos)
        {

            Dictionary<DateTime, Dictionary<int, List<ReporteProduccionStatus>>> lecturasHoy =
                    obtenerDatos
                    .OrderBy(x => x.NumeroGenerador)
                    .GroupBy(x => new DateTime(DateTime.Parse(x.Fecha).Year, DateTime.Parse(x.Fecha).Month, 1))
                    .ToDictionary(
                        outerGroup => outerGroup.Key,
                        outerGroup => outerGroup
                            .OrderBy(x => x.NumeroGenerador)
                            .GroupBy(x => x.NumeroGenerador)
                            .ToDictionary(
                                innerGroup => innerGroup.Key,
                                innerGroup => innerGroup
                                    .OrderBy(y => DateTime.Parse(y.Fecha))
                                    .ToList()
                            )
                    );

            return lecturasHoy;
        }
    }
}
