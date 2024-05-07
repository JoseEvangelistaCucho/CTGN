using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.DashBoard;
using Generacion.Models.Usuario;
using MediatR;

namespace Generacion.Application.DashBoard.Filtro.Command
{
    public class GuardarDatosDashboard : IRequest<Respuesta<string>>
    {
        public List<DashboardDetalleFiltro> detalleFiltro { get; set; }
    }
    public class GuardarDatosDashboardHandler : IRequestHandler<GuardarDatosDashboard, Respuesta<string>>
    {
        private readonly Function _function;
        private readonly IDashBoard _dashBoard;

        public GuardarDatosDashboardHandler(Function function, IDashBoard dashBoard)
        {
            _dashBoard = dashBoard;
            _function = function;
        }
        public async Task<Respuesta<string>> Handle(GuardarDatosDashboard request, CancellationToken cancellationToken)
        {
            DetalleOperario user = await _function.ObtenerDatosOperario();
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                foreach (var item in request.detalleFiltro)
                {
                    string mensajesError = item.ValidarPropiedadesNulasOVacias();
                    if (mensajesError.Any())
                    {
                        respuesta.IdRespuesta = 99;
                        respuesta.Mensaje = mensajesError;
                    }
                }

                respuesta = await _dashBoard.GuardarDatosFiltro(request.detalleFiltro, user.IdSitio);

            }
            catch (Exception e)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = e.Message.ToString();
            }
            return respuesta;
        }
    }
}
