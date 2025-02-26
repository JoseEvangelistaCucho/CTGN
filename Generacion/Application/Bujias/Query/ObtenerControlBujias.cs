using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Almacen.Bujias;
using Generacion.Models.Usuario;
using MediatR;
using Newtonsoft.Json;

namespace Generacion.Application.Bujias.Query
{
    public class ObtenerControlBujias : IRequest<Respuesta<Dictionary<string,object>>>
    {
    }
    public class ObtenerControlBujiasHandler : IRequestHandler<ObtenerControlBujias, Respuesta<Dictionary<string, object>>>
    {
        private readonly Function _function;
        private readonly ConsultaBujias _consultaBujias;
        private readonly Logger _logger;

        public ObtenerControlBujiasHandler(Function function, ConsultaBujias consultaBujias, Logger logger)
        {
            _consultaBujias = consultaBujias;
            _function = function;
            _logger = logger;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerControlBujias request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();

            try
            {
                respuesta.Detalle = new Dictionary<string, object>();
                await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDiccionarioControlPorGE());
                await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerControlCambioPorLado());


            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la solicitud.";
                _logger.LogError("Error ObtenerControlBujiasHandler : " + ex.Message.ToString());
            }
            return respuesta;
        }

        public async Task<Dictionary<string,object>> ObtenerDiccionarioControlPorGE()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();

                Respuesta<List<DetalleRegistroBujias>> datosControl = await _consultaBujias.ObtenerControlCambio(user.IdSitio);

                //  Dictionary<int,DetalleRegistroBujias>diccinarioEG = datosControl.Detalle.ToDictionary(x => x.Numerogenerador);
                Dictionary<int, DetalleRegistroBujias> diccionarioEG = new Dictionary<int, DetalleRegistroBujias>();

                foreach (var detalle in datosControl.Detalle)
                {
                    try
                    {
                        diccionarioEG.Add(detalle.Numerogenerador, detalle);
                    }
                    catch (ArgumentException)
                    {
                        diccionarioEG[detalle.Numerogenerador] = detalle;
                    }
                }
                respuesta.Add("diccinarioEG", diccionarioEG);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error ObtenerDiccionarioControlPorGE : " + ex.Message.ToString());
            }

            return respuesta;
        }

        public async Task<Dictionary<string,object>> ObtenerControlCambioPorLado()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();
            try
            {
                DetalleOperario user = await _function.ObtenerDatosOperario();


                Respuesta<List<RegistroBujias>> listaEg1 = await _consultaBujias.ObtenerdetalleControlCambio(string.Empty, 0, 1, user.IdSitio);
                Respuesta<List<RegistroBujias>> listaEg2 = await _consultaBujias.ObtenerdetalleControlCambio(string.Empty, 0, 2, user.IdSitio);

                var datosDiccionarioPorGE = new Dictionary<int, Dictionary<string, List<Dictionary<int, Dictionary<string, List<RegistroBujias>>>>>>();
                datosDiccionarioPorGE.Add(1, new Dictionary<string, List<Dictionary<int, Dictionary<string, List<RegistroBujias>>>>>
                {
                    {
                        "A", ObtenerRegistrosPorItem(listaEg1.Detalle, "A")
                    },
                    {
                        "B", ObtenerRegistrosPorItem(listaEg1.Detalle, "B")
                    }
                });
                datosDiccionarioPorGE.Add(2, new Dictionary<string, List<Dictionary<int, Dictionary<string, List<RegistroBujias>>>>>
                {
                    {
                        "A", ObtenerRegistrosPorItem(listaEg2.Detalle, "A")
                    },
                    {
                        "B", ObtenerRegistrosPorItem(listaEg2.Detalle, "B")
                    }
                });



                Dictionary<int,Dictionary<int,decimal>> promedioTotalPorGe = new Dictionary<int, Dictionary<int, decimal>>();

                promedioTotalPorGe.Add(1,await obtenerPromedioBujiasPorGE(listaEg1.Detalle));
                promedioTotalPorGe.Add(2,await obtenerPromedioBujiasPorGE(listaEg2.Detalle));


                respuesta.Add("promedioTotalPorGE", promedioTotalPorGe);
                respuesta.Add("detalleControlCambio", datosDiccionarioPorGE);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error ObtenerControlCambioPorLado : " + ex.Message.ToString());
            }

            return respuesta;
        }

        public async Task<Dictionary<int,decimal>> obtenerPromedioBujiasPorGE(List<RegistroBujias> datosBujias)
        {
            Dictionary<int, decimal> respuesta = new Dictionary<int, decimal>();
            if (datosBujias.Count != 0)
            {
                var totalItem = datosBujias.Select(m => m.Item).Distinct().ToList() ;
                //var totalItem = datosBujias.GroupBy(x => x.Item).Select(x => x).ToList();
                decimal promedioTotal = 0;

                foreach (int item in totalItem)
                {
                    decimal sumaTotal = datosBujias.Where(x => x.IdSubtituloCampo.Equals("HorasEnCurso") && x.Item.Equals(item)).Sum(x => x.Detalle);
                    
                    decimal cantidadBujias = (datosBujias.Where(x => x.Item.Equals(item)).Count() / 3);
                    promedioTotal = Math.Round(sumaTotal / cantidadBujias,2);

                    respuesta.Add(item, promedioTotal);
                }
            }
            return respuesta;
        }

        List<Dictionary<int, Dictionary<string, List<RegistroBujias>>>> ObtenerRegistrosPorItem(List<RegistroBujias> lista, string lado)
        {
            return lista
                .Where(x => x.Lado.Equals(lado))
                // .OrderBy(x => x.Fecha)
                .GroupBy(x => x.Item)
                .Select(groupByFecha => groupByFecha
                    // .OrderBy(x => x.Item)
                    .GroupBy(x => x.Item)
                    .ToDictionary(
                        groupByItem => groupByItem.Key,
                        groupByItem => groupByItem
                            .GroupBy(x => x.IdSubtituloCampo)
                            .ToDictionary(
                                groupByIdSubtituloCampo => groupByIdSubtituloCampo.Key,
                                groupByIdSubtituloCampo => groupByIdSubtituloCampo.ToList()
                            )
                    )
                )
                .ToList();
        }

    }
}
