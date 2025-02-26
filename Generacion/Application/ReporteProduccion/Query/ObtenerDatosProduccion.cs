using datosConsola = Generacion.Application.DatosConsola.Query.DatosConsola;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using MediatR;
using Generacion.Models.ReporteProduccion;

namespace Generacion.Application.ReporteProduccion.Query
{
    public class ObtenerDatosProduccion : IRequest<Respuesta<Dictionary<string, object>>>
    {
        public DateTime Fecha { get; set; }
    }
    public class ObtenerDatosProduccionHandler : IRequestHandler<ObtenerDatosProduccion, Respuesta<Dictionary<string, object>>>
    {

        private readonly Function _function;
        private readonly datosConsola _datosConsola;
        private readonly ConsultarProduccion _consultarProduccion;

        public ObtenerDatosProduccionHandler(ConsultarProduccion consultarProduccion, Function function, datosConsola datosConsola)
        {
            _consultarProduccion = consultarProduccion;
            _function = function;
            _datosConsola = datosConsola;
        }

        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerDatosProduccion request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();

            try
            {
                respuesta.Detalle = new Dictionary<string, object>();

               /* if (!string.IsNullOrEmpty(request.Fecha))
                {
                    _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerEnergiaProducidaData(request));

                }
                else
                {*/
                    _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerenergiaProducida(request));
                    _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerArranquesSincronizaciones(request.Fecha));

               // }


                respuesta.IdRespuesta = 0;
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 0;
                respuesta.Mensaje = "Error al procesar la solicitud";
            }

            return respuesta;
        }
        public async Task<Dictionary<string, object>> ObtenerArranquesSincronizaciones(DateTime fecha)
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();
            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();

                var respuestaArranque = await _consultarProduccion.ObtenerNumeroArranque($"{datosOperario.IdSitio}-ELD-RPT-PROD_{fecha.AddDays(-1).ToString("yyyy-MM-dd")}", "dia", string.Empty);

                Dictionary<string, Dictionary<int, ArranqueSincronizacion>> datosSincro = respuestaArranque.Detalle
                                                                                             .GroupBy(x => x.Tipo) 
                                                                                             .ToDictionary(
                                                                                                 outerGroup => outerGroup.Key, 
                                                                                                 outerGroup => outerGroup
                                                                                                     .ToDictionary(
                                                                                                         innerGroup => innerGroup.NumeroGenerador 
                                                                                                     )
                                                                                             );

            respuesta.Add("DatosSincro", datosSincro);

            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }


        //public async Task<Dictionary<string, object>> ObtenerEnergiaProducidaData(ObtenerDatosProduccion obtenerDatos)
        //{
        //    Dictionary<string, object> respuesta = new Dictionary<string, object>();

        //    var datosOperario = await _function.ObtenerDatosOperario();

        //    List<LecturasMedianoche> datosLecturasHoy = await _datosConsola.ObtenerLecturaMediaNoche(null, obtenerDatos.Fecha);

        //    Dictionary<DateTime, Dictionary<int, List<LecturasMedianoche>>> lecturasHoy =
        //            datosLecturasHoy
        //            .OrderBy(x => x.NumeroEG)
        //            .GroupBy(x => new DateTime(DateTime.Parse(x.fecha).Year, DateTime.Parse(x.fecha).Month, 1))
        //            .ToDictionary(
        //                outerGroup => outerGroup.Key,
        //                outerGroup => outerGroup
        //                    .OrderBy(x => x.NumeroEG)
        //                    .GroupBy(x => x.NumeroEG)
        //                    .ToDictionary(
        //                        innerGroup => innerGroup.Key,
        //                        innerGroup => innerGroup
        //                            .OrderBy(y => DateTime.Parse(y.fecha)) 
        //                            .ToList()
        //                    )
        //            );


        //    respuesta.Add("datosLecturas", lecturasHoy);

        //    return respuesta;
        //}
        public async Task<Dictionary<string, object>> ObtenerenergiaProducida(ObtenerDatosProduccion obtenerDatos)
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            var datosOperario = await _function.ObtenerDatosOperario();

            /* string idReporteHoy = $"{datosOperario.IdSitio}-ELD-CTL-OM002_{DateTime.Now.ToString("yyyy-MM-dd")}";
             string idReporteAyer = $"{datosOperario.IdSitio}-ELD-CTL-OM002_{DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}";
             */
            string idReporteHoy = $"{datosOperario.IdSitio}-ELD-CTL-OM002_{obtenerDatos.Fecha.ToString("yyyy-MM-dd")}";
            string idReporteAyer = $"{datosOperario.IdSitio}-ELD-CTL-OM002_{obtenerDatos.Fecha.AddDays(-1).ToString("yyyy-MM-dd")}";

            var datosLecturasHoy = await _datosConsola.ObtenerLecturaMediaNoche(idReporteHoy, obtenerDatos.Fecha.ToString());//$"{datosOperario.IdSitio}-ELD-CTL-OM002_{DateTime.Now.ToString("yyyy-MM-dd")}", fecha);
            Dictionary<int, LecturasMedianoche> lecturasHoy = datosLecturasHoy.ToDictionary(x => x.NumeroEG);

            var datosLecturasAyer = await _datosConsola.ObtenerLecturaMediaNoche(idReporteAyer, obtenerDatos.Fecha.ToString());//$"{datosOperario.IdSitio}-ELD-CTL-OM002_{DateTime.Now.ToString("yyyy-MM-dd")}", fecha);
            Dictionary<int, LecturasMedianoche> lecturasAyer = datosLecturasAyer.ToDictionary(x => x.NumeroEG);


            Dictionary<string, Dictionary<int, LecturasMedianoche>> unionLecturas = new Dictionary<string, Dictionary<int, LecturasMedianoche>>();
            unionLecturas.Add("ayer", lecturasAyer);
            unionLecturas.Add("hoy", lecturasHoy);

            respuesta.Add("datosLecturas", unionLecturas);

            return respuesta;
        }
    }
}
