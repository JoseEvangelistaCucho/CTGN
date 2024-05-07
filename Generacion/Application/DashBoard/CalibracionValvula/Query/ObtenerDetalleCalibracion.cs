using Generacion.Application.DashBoard.TurboCompresor.Query;
using Generacion.Models;
using Generacion.Models.CalibracionValvula;
using MediatR;

namespace Generacion.Application.DashBoard.CalibracionValvula.Query
{
    public class ObtenerDetalleCalibracion : IRequest<Respuesta<Dictionary<int,DetalleCalibracionValvula>>>
    {
    }

    public class ObtenerDetalleCalibracionHandler : IRequestHandler<ObtenerDetalleCalibracion, Respuesta<Dictionary<int, DetalleCalibracionValvula>>>
    {
        private readonly DatosCalibracionValvula _datosCalibracion;
        public ObtenerDetalleCalibracionHandler(DatosCalibracionValvula datosCalibracion)
        {
            _datosCalibracion = datosCalibracion;
        }
        public async  Task<Respuesta<Dictionary<int, DetalleCalibracionValvula>>> Handle(ObtenerDetalleCalibracion request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<int, DetalleCalibracionValvula>> respuesta = new Respuesta<Dictionary<int, DetalleCalibracionValvula>>();
            try
            {
                respuesta.Detalle = new Dictionary<int, DetalleCalibracionValvula>();
                Respuesta<DetalleCalibracionValvula> respuestaCalibracionValvula = await _datosCalibracion.ObtenerDetalleCalibracionPorNumeroGE(1);
                respuesta.Detalle.Add(1, respuestaCalibracionValvula.Detalle);

                respuestaCalibracionValvula = await _datosCalibracion.ObtenerDetalleCalibracionPorNumeroGE(2);
                respuesta.Detalle.Add(2, respuestaCalibracionValvula.Detalle);

            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la solicitud.";
            }

            return respuesta;
        }
    }
}
