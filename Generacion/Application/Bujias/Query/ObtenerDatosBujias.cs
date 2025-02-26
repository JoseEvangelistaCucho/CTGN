using Generacion.Application.Almacen.Query;
using Generacion.Models;
using Generacion.Models.Almacen.Bujias;
using Generacion.Models.Almacen;
using MediatR;
using Generacion.Application.Almacen;
using Generacion.Application.Funciones;
using Generacion.Application.DashBoard.Bujias.Query;

namespace Generacion.Application.Bujias.Query
{
    public class ObtenerDatosBujias : IRequest<Respuesta<Dictionary<string, object>>>
    {
    }

    public class ObtenerDatosBujiasHandler : IRequestHandler<ObtenerDatosBujias, Respuesta<Dictionary<string, object>>>
    {
        private readonly ConsultasAlmacen _consultasAlmacen;
        private readonly Function _function;
        private readonly IAlmacen _almacen;

        private readonly IBujias _bujias;
        private readonly ConsultaBujias _consultaBujias;
        private readonly Logger _logger;
        public ObtenerDatosBujiasHandler(Function function, ConsultasAlmacen consultasAlmacen, IAlmacen almacen, IBujias bujias, ConsultaBujias consultaBujias, Logger logger)
        {
            _function = function;
            _consultaBujias = consultaBujias;
            _bujias = bujias;
            _almacen = almacen;
            _consultasAlmacen = consultasAlmacen;
            _logger = logger;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerDatosBujias request, CancellationToken cancellationToken)
        {

            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();

                respuesta.Detalle = new Dictionary<string, object>();
                await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerBujiasAlmacen());
                await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerBujiasPrestados());
                await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerTotalBujias());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error ObtenerDatosBujiasHandler : " + ex.Message.ToString());
            }

            return respuesta;

        }
        public async Task<Dictionary<string, object>> ObtenerTotalBujias()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            Respuesta<List<ComponenteBujias>> detalleBujias = await _consultasAlmacen.ObtenerDatosAlmacenBujias("Bujias251023");
            Respuesta<int> bujiasUsadasGe1 = await _consultaBujias.ObtenerTotalBujiaUsado(1);
            Respuesta<int> bujiasUsadasGe2 = await _consultaBujias.ObtenerTotalBujiaUsado(2);

             Dictionary<int, int> totalBujias = new Dictionary<int, int>
            {
                { 1, bujiasUsadasGe1.Detalle }, { 2, bujiasUsadasGe2.Detalle } 
            };

            respuesta.Add("TotalBujias", totalBujias);

            return respuesta;
        }
        public async Task<Dictionary<string, object>> ObtenerBujiasAlmacen()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

           // Respuesta<List<ComponenteBujias>> detalleBujias = await _consultasAlmacen.ObtenerDatosporLote();

            Respuesta<List<ComponenteBujias>> detalleBujias = await _consultasAlmacen.ObtenerDatosAlmacenBujias("Bujias251023");

            respuesta.Add("bujiasEnAlmacen", detalleBujias.Detalle);

            return respuesta;
        }

        public async Task<Dictionary<string, object>> ObtenerBujiasPrestados()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            Respuesta<List<PrestamoComponente>> detalleBujiasPrestadas = await _consultasAlmacen.ObtenerDatosPrestados("Bujias251023");


            respuesta.Add("bujiasEnPrestamo", detalleBujiasPrestadas.Detalle);

            return respuesta;
        }
    }
}
