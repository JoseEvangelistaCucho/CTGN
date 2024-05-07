using Generacion.Application.BessLlipata.DataBess.Query;
using Generacion.Application.BessLlipata.DataCoes.Query;
using Generacion.Application.Common;
using Generacion.Models.Bess;
using MediatR;
using MediatR.Pipeline;
using NuGet.Packaging;
using Stimulsoft.Blockly.Model;

namespace Generacion.Application.DescargaReporte.Query.Processor.Preprocessor
{
    public class _02_ObtenerDatosBess : IRequestPreProcessor<ObtenerRegistroReporte>
    {
        public IMediator _sender;
        public ProcessExecutionContextExtensions _context;
        public _02_ObtenerDatosBess(IMediator mediator, ProcessExecutionContextExtensions context)
        {
            _sender = mediator;
            _context = context;
        }
        public async Task Process(ObtenerRegistroReporte request, CancellationToken cancellationToken)
        {
            var datosBess = await _sender.Send(new ObtenerDatosBESSQuery()
            {
                TipoConsulta = "año",
                Fecha = DateTime.Parse(request.Fecha).ToString("yyyy")
            });

            _context.TryAdd("listaDataBESS",datosBess.Detalle["listaDataBESS"]);

            Task.CompletedTask.Wait();
        }
    }
}
