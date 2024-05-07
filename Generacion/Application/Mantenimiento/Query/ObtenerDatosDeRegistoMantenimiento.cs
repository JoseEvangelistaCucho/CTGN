using Generacion.Application.DashBoard.TKVessel.Query;
using datosConsola = Generacion.Application.DatosConsola.Query.DatosConsola;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using MediatR;

namespace Generacion.Application.Mantenimiento.Query
{
    public class ObtenerDatosDeRegistoMantenimiento :IRequest<Respuesta<Dictionary<string, object>>>
    {
    }

    public class ObtenerDatosDeRegistoMantenimientoHandler : IRequestHandler<ObtenerDatosDeRegistoMantenimiento, Respuesta<Dictionary<string, object>>>
    {
        private readonly ISender _sender;
        private readonly Function _function;
        private readonly datosConsola _datosConsola;
        public ObtenerDatosDeRegistoMantenimientoHandler( Function function, ISender sender, datosConsola datosConsola)
        {
            _datosConsola = datosConsola;
            _function = function;
            _sender = sender;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerDatosDeRegistoMantenimiento request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();
            try
            {
                respuesta.Detalle = new Dictionary<string, object>();

                await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDetalleTKVessel());
                await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDatosDeOperacionDeGenerador());

            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Dictionary<string,object>> ObtenerDatosDeOperacionDeGenerador()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                Respuesta<Dictionary<string, List<RegistrosDatosEngine>>> datosEngine = await _datosConsola.ObtenerDatosEngine(DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), datosOperario.IdSitio);
                respuesta.Add("RuningHours", datosEngine.Detalle);

            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }

        public async Task<Dictionary<string, object>> ObtenerDetalleTKVessel()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            var datosTKVessel = await _sender.Send(new ObtenerDetalledatosTKVesselPorGE());

            respuesta.Add("datosTKVessel", datosTKVessel.Detalle);

            return respuesta;
        }
    }
}
