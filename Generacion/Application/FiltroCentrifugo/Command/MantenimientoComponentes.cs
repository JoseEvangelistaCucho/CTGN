using Generacion.Application.Common;
using Generacion.Application.DashBoard.Filtro.Query;
using Generacion.Application.FiltroCentrifugo.Query;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.DashBoard;
using Generacion.Models.FiltroCentrifugo;
using Generacion.Models.Usuario;
using MediatR;
using Newtonsoft.Json;

namespace Generacion.Application.FiltroCentrifugo.Command
{
    public class MantenimientoComponentes : IRequest<Respuesta<Dictionary<string, object>>>
    {
        public string TipoComponente { get; set; }
        public string Seleccion { get; set; }
        public bool RequiereId { get; set; }
    }

    public class MantenimientoComponentesHandler : IRequestHandler<MantenimientoComponentes, Respuesta<Dictionary<string, object>>>
    {
        private readonly DatosFiltroCentrifugo _datosFiltroCentrifugo;
        private readonly Function _function;
        private readonly DatosFiltro _datosFiltro;

        public MantenimientoComponentesHandler(
           DatosFiltroCentrifugo datosFiltroCentrifugo,
           Function function,
           DatosFiltro datosFiltro)
        {
            _function = function;
            _datosFiltroCentrifugo = datosFiltroCentrifugo;
            _datosFiltro = datosFiltro;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(MantenimientoComponentes request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();
            respuesta.Detalle = new Dictionary<string, object>();
            DetalleOperario user = await _function.ObtenerDatosOperario();

            Respuesta<ReporteFiltro> detalleFiltro = await _datosFiltroCentrifugo.ObtenerReporteFiltro(request.TipoComponente);

            Respuesta<Dictionary<decimal, EspecificacionFiltro>> especificacionesFiltro = new Respuesta<Dictionary<decimal, EspecificacionFiltro>>();
            Respuesta<List<DetalleFiltro>> datosFiltro = new Respuesta<List<DetalleFiltro>>();
            Respuesta<Dictionary<decimal, DashboardDetalleFiltro>>  datosDashboard = new Respuesta<Dictionary<decimal, DashboardDetalleFiltro>>();
            if (detalleFiltro.Detalle != null)
            {
                especificacionesFiltro = await _datosFiltroCentrifugo.ObtenerEspecificacionesPorNumeroGE(detalleFiltro.Detalle.IdReporteFiltro);
            }

            string idReporteFiltro = string.Empty;

            if (request.RequiereId)
            {
                if (detalleFiltro.Detalle != null)
                {
                    idReporteFiltro = detalleFiltro.Detalle.IdReporteFiltro;
                    datosDashboard = await _datosFiltro.ObtenerDetalleDashboardPorNumeroGE(string.Empty, detalleFiltro.Detalle.IdReporteFiltro);
                }
            }
            if (string.IsNullOrEmpty(request.TipoComponente))
            {
                datosFiltro = await _datosFiltroCentrifugo.ObtenerDatosFiltroPorSitio(request.Seleccion, idReporteFiltro);
            }
            else
            {
                datosFiltro = await _datosFiltroCentrifugo.ObtenerDatosFiltroPorSitioyTipo(request.TipoComponente);
            }



            respuesta.Detalle.Add("especificacionesFiltro", especificacionesFiltro.Detalle);
            respuesta.Detalle.Add("datosFiltro", datosFiltro.Detalle);
            respuesta.Detalle.Add("datosDashboard", datosDashboard.Detalle);
            return respuesta;
        }
    }
}
