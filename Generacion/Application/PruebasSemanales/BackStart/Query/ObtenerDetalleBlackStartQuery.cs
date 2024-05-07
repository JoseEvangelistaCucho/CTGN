using Generacion.Models;
using Generacion.Models.PruebasSemanales.BlackStart;
using MediatR;

namespace Generacion.Application.PruebasSemanales.BackStart.Query
{
    public class ObtenerDetallePruebaSemanalQuery : IRequest<Respuesta<Dictionary<string, object>>>
    {
        public string Tipo { get; set; }
    }
    public class ObtenerDetallePruebaSemanalQueryHandler : IRequestHandler<ObtenerDetallePruebaSemanalQuery, Respuesta<Dictionary<string, object>>>
    {
        private readonly ObtenerRegistrosPruebaSemanal _registrosPruebaSemanal;
        public List<string> _keys = new List<string>()
        {
            "pruebaSemanal","detallePruebaSemanal"
        };
        public ObtenerDetallePruebaSemanalQueryHandler(ObtenerRegistrosPruebaSemanal registrosPruebaSemanal)
        {
            _registrosPruebaSemanal = registrosPruebaSemanal;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerDetallePruebaSemanalQuery request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();

            try
            {
                respuesta.Detalle = new Dictionary<string, object>();
                respuesta.Detalle.Add("keys", _keys);

                var datosPruebaSemanal = await _registrosPruebaSemanal.ObtenerDatosPruebaSemanal(request.Tipo);

                if (datosPruebaSemanal.Detalle.IdPruebaSemanal != null)
                {
                    respuesta.Detalle.Add(_keys[0], datosPruebaSemanal.Detalle);
                    var detallePruebaSemanal = await ObtenerDatosOrdenados(datosPruebaSemanal.Detalle.IdPruebaSemanal);
                    respuesta.Detalle.Add(_keys[1], detallePruebaSemanal);
                }

            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Dictionary<string, Dictionary<string, DetallePruebaSemanal>>> ObtenerDatosOrdenados(string IdPruebaSemanal)
        {
            Respuesta<List<DetallePruebaSemanal>> detallePruebaSemanal = await _registrosPruebaSemanal.ObtenerDetallePruebaSemanal(IdPruebaSemanal);

            Dictionary<string, Dictionary<string, DetallePruebaSemanal>> datosOrganizados = new Dictionary<string, Dictionary<string, DetallePruebaSemanal>>();

            var gruposPorFecha = detallePruebaSemanal.Detalle.Select(x => x.Fecha).ToList();
            foreach (var fecha in gruposPorFecha)
            {
                if (!datosOrganizados.ContainsKey(fecha))
                {
                    datosOrganizados[fecha] = new Dictionary<string, DetallePruebaSemanal>();
                }
                datosOrganizados[fecha] = detallePruebaSemanal.Detalle.ToDictionary(x => x.IdSubtitulo, x => x);
            }

            return datosOrganizados;
        }
    }
}
