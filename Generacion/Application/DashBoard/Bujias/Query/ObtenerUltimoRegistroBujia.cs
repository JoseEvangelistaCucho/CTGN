using Generacion.Application.Bujias.Query;
using Generacion.Models;
using Generacion.Models.Almacen.Bujias;
using MediatR;

namespace Generacion.Application.DashBoard.Bujias.Query
{
    public class ObtenerUltimoRegistroBujia : IRequest<Respuesta<Dictionary<int, RegistroBujias>>>
    {
    }

    public class ObtenerUltimoRegistroBujiaHandler : IRequestHandler<ObtenerUltimoRegistroBujia, Respuesta<Dictionary<int, RegistroBujias>>>
    {

        private readonly ConsultaBujias _consultaBujias;

        public ObtenerUltimoRegistroBujiaHandler(ConsultaBujias consultaBujias)
        {
            _consultaBujias = consultaBujias;
        }
        public async Task<Respuesta<Dictionary<int, RegistroBujias>>> Handle(ObtenerUltimoRegistroBujia request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<int, RegistroBujias>> respuesta = new Respuesta<Dictionary<int, RegistroBujias>>();

            try
            {
                var datosbujiasGe1 = await _consultaBujias.ObtenerRegistrosPorGE(1);

                respuesta.Detalle = new Dictionary<int, RegistroBujias>();
                if (datosbujiasGe1.Detalle != null)
                {
                    respuesta.Detalle.Add(1, datosbujiasGe1.Detalle);
                }

                var datosbujiasGe2 = await _consultaBujias.ObtenerRegistrosPorGE(2);
                if (datosbujiasGe2.Detalle != null)
                {
                    respuesta.Detalle.Add(2, datosbujiasGe2.Detalle);
                }
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }
    }
}
