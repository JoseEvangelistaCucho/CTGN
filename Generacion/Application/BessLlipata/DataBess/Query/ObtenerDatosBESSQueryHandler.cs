using Generacion.Application.BessLlipata.Query;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Bess;
using MediatR;

namespace Generacion.Application.BessLlipata.DataBess.Query
{

    public class ObtenerDatosBESSQuery : IRequest<Respuesta<Dictionary<string, object>>>
    {
        public string Fecha { get; set; }
        public string TipoConsulta { get; set; }
    }
    public class ObtenerDatosBESSQueryHandler : IRequestHandler<ObtenerDatosBESSQuery, Respuesta<Dictionary<string, object>>>
    {
        //DataBess
        private readonly Logger _logger;
        private readonly ObtenerDatosBessLlipata _obtenerDatosBessLlipata;
        private readonly Function _function;
        public ObtenerDatosBESSQueryHandler(ObtenerDatosBessLlipata obtenerDatosBessLlipata, Function function, Logger logger)
        {
            _logger = logger;
            _function = function;
            _obtenerDatosBessLlipata = obtenerDatosBessLlipata;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerDatosBESSQuery request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();
            try
            {
                respuesta.Detalle = new Dictionary<string, object>();

                await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDatosBessPorFecha(request));

            }
            catch (Exception ex)
            {
                _logger.LogError("Error ObtenerDatosBESSQueryHandler : " + ex.Message.ToString());
            }
            return respuesta;
        }
        public async Task<Dictionary<string, object>> ObtenerDatosBessPorFecha(ObtenerDatosBESSQuery request)
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();
            try
            {
                Respuesta<List<DBDataBess>> datos = await _obtenerDatosBessLlipata.ObtenerDatosBEES(request.Fecha, request.TipoConsulta);

                respuesta.Add("listaDataBESS", datos.Detalle);
                respuesta.Add("requiereArchivos", datos.Detalle.Count < 0);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error ObtenerDatosBessPorFecha : " + ex.Message.ToString());

            }
            return respuesta;
        }
    }
}
