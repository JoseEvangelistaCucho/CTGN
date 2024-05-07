using Generacion.Models;
using Generacion.Models.Aceite;
using MediatR;

namespace Generacion.Application.DashBoard.CambioAceite.Query
{
    public class ObtenerDetallesAceitePorEGyTipo : IRequest<Respuesta<Dictionary<int, DetalleControlAceite>>>
    {
    }
    public class ObtenerDetallesAceitePorEGyTipoHandler : IRequestHandler<ObtenerDetallesAceitePorEGyTipo, Respuesta<Dictionary<int, DetalleControlAceite>>>
    {
        private readonly DatosControlRegistroAceite _datosControlRegistro;
        public ObtenerDetallesAceitePorEGyTipoHandler(
            DatosControlRegistroAceite datosControlRegistro
            )
        {
            _datosControlRegistro = datosControlRegistro;
        }
        public async Task<Respuesta<Dictionary<int, DetalleControlAceite>>> Handle(ObtenerDetallesAceitePorEGyTipo request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<int, DetalleControlAceite>> respuesta = new Respuesta<Dictionary<int, DetalleControlAceite>>();
            try
            {
                var datosGE1 = await _datosControlRegistro.ObtenerDetalleControlAceite(1);
                var datosGE2 = await _datosControlRegistro.ObtenerDetalleControlAceite(2);
                respuesta.Detalle = new Dictionary<int, DetalleControlAceite>();
                if (datosGE1.Detalle != null)
                {
                    respuesta.Detalle.Add(1, datosGE1.Detalle);
                }
                if (datosGE2.Detalle != null)
                {
                    respuesta.Detalle.Add(2, datosGE2.Detalle);
                }

                respuesta.IdRespuesta = datosGE1.IdRespuesta;
                respuesta.Mensaje = datosGE1.Mensaje;
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }
    }
}
