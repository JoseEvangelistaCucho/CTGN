using Generacion.Application.DashBoard.TurboCompresor.Query;
using Generacion.Models;
using Generacion.Models.TurboCompresor;
using MediatR;

namespace Generacion.Application.DashBoard.CambioAceite.Query
{
    public class ObtenerIDDatosTurboCompresor : IRequest<Respuesta<DatosTurboCompresor>>
    {
    }

    public class ObtenerIDDatosTurboCompresorHanlder : IRequestHandler<ObtenerIDDatosTurboCompresor, Respuesta<DatosTurboCompresor>>
    {
        private readonly ConsultaDatosTurboCompresor _consultaDatosTurboCompresor;
        public ObtenerIDDatosTurboCompresorHanlder(ConsultaDatosTurboCompresor consultaDatosTurboCompresor)
        {
            _consultaDatosTurboCompresor = consultaDatosTurboCompresor;
        }
        public async Task<Respuesta<DatosTurboCompresor>> Handle(ObtenerIDDatosTurboCompresor request, CancellationToken cancellationToken)
        {
            Respuesta<DatosTurboCompresor> respuesta = await _consultaDatosTurboCompresor.ObtenerDatosTurboCompresor();

            return respuesta;
        }
    }
}
