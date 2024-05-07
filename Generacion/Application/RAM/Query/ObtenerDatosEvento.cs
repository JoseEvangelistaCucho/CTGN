using Generacion.Application.Eventos.Query;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.ReporteProduccion;
using MediatR;

namespace Generacion.Application.RAM.Query
{
    public class ObtenerDatosEvento : IRequest<Respuesta<Dictionary<string, object>>>
    {
        public string Fecha { get; set; }
        public string Tipo { get; set; }
    }

    public class ObtenerDatosEventoHandler : IRequestHandler<ObtenerDatosEvento, Respuesta<Dictionary<string, object>>>
    {
        private readonly DatosEventos _datosEventos;
        private readonly Function _function;
        public ObtenerDatosEventoHandler(DatosEventos datosEventos, Function function)
        {
            _function = function;
            _datosEventos = datosEventos;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerDatosEvento request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();

            try
            {
                Respuesta<List<RegistroEventos>> datosEvento = await _datosEventos.obtenerDatosEventoPorFecha(request.Fecha, request.Tipo, "ReporteProduccion");

                respuesta.Detalle = new Dictionary<string, object>();

                _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDatosPorFecha(datosEvento.Detalle));

            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Dictionary<string, object>> ObtenerDatosPorFecha(List<RegistroEventos> request)
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();
            try
            {

                Dictionary<int, Dictionary<DateTime, List<RegistroEventos>>> datosAgrupados = request
                    .OrderBy(x => x.NumeroGenerador) 
                    .GroupBy(x => x.NumeroGenerador) 
                    .ToDictionary(
                        outerGroup => outerGroup.Key,
                        outerGroup => outerGroup
                            .OrderBy(x => DateTime.Parse(x.FechaParada)) 
                            .GroupBy(x => new DateTime(DateTime.Parse(x.FechaParada).Year, DateTime.Parse(x.FechaParada).Month, 1)) 
                            .ToDictionary(
                               innerGroup => innerGroup.Key,
                                innerGroup => innerGroup.ToList()
                            )
                    );

                datosAgrupados = datosAgrupados.OrderBy(x => x.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

                respuesta.Add("eventosPorFecha", datosAgrupados);
            }
            catch (Exception ex)
            {

            }
            return respuesta;

        }
    }
}
