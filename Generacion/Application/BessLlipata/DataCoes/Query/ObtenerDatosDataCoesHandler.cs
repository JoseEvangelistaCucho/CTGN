using Generacion.Application.BessLlipata.Query;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.DataCoes;
using MediatR;
using System.Globalization;

namespace Generacion.Application.BessLlipata.DataCoes.Query
{
    public class ObtenerDatosDataCoes : IRequest<Respuesta<Dictionary<string, object>>>
    {
        public DateTime Fecha { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
    public class ObtenerDatosDataCoesHandler : IRequestHandler<ObtenerDatosDataCoes, Respuesta<Dictionary<string, object>>>
    {
        private readonly ObtenerDatosBessLlipata _obtenerDatosBessLlipata;
        private readonly Function _function;
        private Dictionary<string, List<Demanda>> _diccionarioOrdenado;
        public ObtenerDatosDataCoesHandler(ObtenerDatosBessLlipata obtenerDatosBessLlipata, Function function)
        {
            _function = function;
            _obtenerDatosBessLlipata = obtenerDatosBessLlipata;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerDatosDataCoes request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();
            try
            {
                respuesta.Detalle = new Dictionary<string, object>();
                Respuesta<List<Demanda>> datosBess = await _obtenerDatosBessLlipata.ObtenerDatosCOES(request.FechaInicio, request.FechaFin);

                var datosPorFecha = await ObtenerDatosPorFecha(datosBess.Detalle);

                await _function.AgregarDatosKeyString(respuesta.Detalle, datosPorFecha);
                await _function.AgregarDatosKeyString(respuesta.Detalle, await CalcularDiaCoincidente(datosBess.Detalle));
                await _function.AgregarDatosKeyString(respuesta.Detalle, await CalcularValorexMaximoPorDia(datosPorFecha));
                await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDatosPorFechaSeleccionada(request.Fecha, datosBess.Detalle));

            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Dictionary<string, object>> ObtenerDatosPorFechaSeleccionada(DateTime fecha, List<Demanda> listDemandas)
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();
            try
            {
                List<Demanda> demandas = new List<Demanda>();

                demandas = listDemandas.Where(x => x.Fecha.ToString("dd/MM/yyyy").Equals(fecha.ToString("dd/MM/yyyy"))).ToList();

                demandas = demandas.OrderBy(d => d.Fecha).ToList();

                respuesta.Add("detallePorFechaSeleccionado", demandas);

            }
            catch (Exception ex)
            {



            }
            return respuesta;
        }

        public async Task<Dictionary<string, object>> CalcularValorexMaximoPorDia(Dictionary<string, object> datos)
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();
            try
            {
                Dictionary<string, Demanda> valoresMaximoPorHora = new Dictionary<string, Demanda>();
                Dictionary<string, Demanda> valoresMaximoPorDia = new Dictionary<string, Demanda>();
                foreach (List<Demanda> item in _diccionarioOrdenado.Values)
                {
                    Demanda maximoValorHora = item.Where(x =>
                                            {
                                                if (TimeSpan.TryParse(x.Hora, out var hora))
                                                {
                                                    Console.WriteLine($"{x.Hora}: {hora.TotalHours}");
                                                    return hora.TotalHours >= 17 && hora.TotalHours <= 23;
                                                }
                                                return false;
                                            })
                                            .OrderByDescending(d => d.Ejecutado)
                                            .FirstOrDefault();

                    Demanda maximoValorDia = item
                                            .OrderByDescending(d => d.Ejecutado)
                                            .FirstOrDefault();

                    if (maximoValorHora != null)
                    {
                        valoresMaximoPorHora.Add(maximoValorHora.Fecha.ToString("dd/MM/yyyy"), maximoValorHora);
                    }
                    if (maximoValorDia != null)
                    {
                        valoresMaximoPorDia.Add(maximoValorDia.Fecha.ToString("dd/MM/yyyy"), maximoValorDia);
                    }
                }

                respuesta.Add("diasCoincidentesDia", valoresMaximoPorDia);
                respuesta.Add("diasCoincidentesHora", valoresMaximoPorHora);
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Dictionary<string, object>> CalcularDiaCoincidente(List<Demanda> datosBess)
        {
            Dictionary<string, object> respueta = new Dictionary<string, object>();

            try
            {
                Demanda maxDemanda = null;
                if (datosBess.Count > 0)
                {

                    var hora = int.Parse(datosBess[0].Hora.Split(':')[1]);
                    List<Demanda> demandaHoraria = datosBess
                                                    .Where(x =>
                                                    {
                                                        if (TimeSpan.TryParse(x.Hora, out var hora))
                                                        {
                                                            Console.WriteLine($"{x.Hora}: {hora.Hours}");
                                                            return hora.Hours >= 17 && hora.Hours <= 23;
                                                        }
                                                        return false;
                                                    })
                                                    .ToList();
                    maxDemanda = demandaHoraria.OrderByDescending(d => d.Ejecutado).FirstOrDefault();

                }
                respueta.Add("demandaCoindicente", maxDemanda);

            }
            catch (Exception ex)
            {

            }
            return respueta;
        }
        public async Task<Dictionary<string, object>> ObtenerDatosPorFecha(List<Demanda> datosBess)
        {
            Dictionary<string, object> respueta = new Dictionary<string, object>();
            try
            {
                Dictionary<string, List<Demanda>> diccionario = datosBess
                                                                 .GroupBy(d => d.Fecha.ToString("dd/MM/yyyy"))
                                                                 .ToDictionary(
                                                                     g => g.Key,
                                                                     g => g.ToList()
                                                                 );

                _diccionarioOrdenado = diccionario.OrderBy(x => DateTime.Parse(x.Key))
                                                     .ToDictionary(x => x.Key, x => x.Value);


                respueta.Add("requiereArchivos", diccionario.Count > 0);
                respueta.Add("DatosCoes", _diccionarioOrdenado);
            }
            catch (Exception ex)
            {

            }
            return respueta;
        }
    }
}
