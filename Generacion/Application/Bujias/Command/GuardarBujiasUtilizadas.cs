using Generacion.Application.Almacen;
using Generacion.Application.Almacen.Command;
using Generacion.Application.Funciones;
using Generacion.Models;
using MediatR;

namespace Generacion.Application.Bujias.Command
{
    public class GuardarBujiasUtilizadas :IRequest<Respuesta<string>>
    {
    }

    public class GuardarBujiasUtilizadasHandler : IRequestHandler<GuardarBujiasUtilizadas, Respuesta<string>>
    {
        private readonly IAlmacen _almacen;
        private readonly Logger _logger;
        public GuardarBujiasUtilizadasHandler(IAlmacen almacen, Logger logger)
        {
            _almacen = almacen;
            _logger = logger;
        }
        public async Task<Respuesta<string>> Handle(GuardarBujiasUtilizadas request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                var datos = await _almacen.GuardarBujiasUtilizadas(1);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error ObtenerDatosBESSQueryHandler : " + ex.Message.ToString());
            }

            return respuesta;

        }
    }
}
