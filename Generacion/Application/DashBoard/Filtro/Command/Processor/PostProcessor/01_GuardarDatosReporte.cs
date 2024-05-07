using Generacion.Application.FiltroCentrifugo;
using Generacion.Models;
using Generacion.Models.FiltroCentrifugo;
using MediatR.Pipeline;

namespace Generacion.Application.DashBoard.Filtro.Command.Processor.PostProcessor
{
    public class _01_GuardarDatosReporte : IRequestPostProcessor<GuardarDatosDashboard, Respuesta<string>>
    {
        private readonly IRegistroFiltroCentrifugo _registroFiltro;
        public _01_GuardarDatosReporte(IRegistroFiltroCentrifugo registroFiltroCentrifugo)
        {
            _registroFiltro = registroFiltroCentrifugo;
        }
        public Task Process(GuardarDatosDashboard request, Respuesta<string> response, CancellationToken cancellationToken)
        {
            if (response.IdRespuesta == 0)
            {
                ReporteFiltro reporteFiltro = new ReporteFiltro()
                {
                    Fecha = DateTime.Now.ToString("dd/MM/yyyy"),
                    IdReporteFiltro = request.detalleFiltro[0].IdReporteFiltro,
                    Tipo = request.detalleFiltro[0].IdDetalleFiltro.Split('-')[1]
                };

                var respuesta = _registroFiltro.GuardarReporteFiltro(reporteFiltro).Result;

                if (respuesta.IdRespuesta != 0)
                {
                    throw new Exception("Hubo un error al guardar el ReporteFiltro");
                }
            }
            return Task.CompletedTask;
        }
    }
}
