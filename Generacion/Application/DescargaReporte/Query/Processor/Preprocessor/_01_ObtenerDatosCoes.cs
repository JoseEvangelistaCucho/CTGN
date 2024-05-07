using Generacion.Application.BessLlipata.DataCoes.Query;
using Generacion.Application.Common;
using MediatR;
using MediatR.Pipeline;
using NuGet.Packaging;
using Stimulsoft.Blockly.Model;

namespace Generacion.Application.DescargaReporte.Query.Processor.Preprocessor
{
    public class _01_ObtenerDatosCoes : IRequestPreProcessor<ObtenerRegistroReporte>
    {
        public IMediator _sender;
        public ProcessExecutionContextExtensions _context;
        public _01_ObtenerDatosCoes(IMediator mediator, ProcessExecutionContextExtensions context)
        {
            _sender = mediator;
            _context = context;
        }
        public async Task Process(ObtenerRegistroReporte request, CancellationToken cancellationToken)
        {
            DateTime primerDiaDelMes = new DateTime(DateTime.Parse(request.Fecha).Year, 1, 1, 0, 30, 0);
            DateTime ultimoDiaDelMes = new DateTime(DateTime.Parse(request.Fecha).Year, 12, 1, 0, 30, 0);
            var datosCoes = await _sender.Send(new ObtenerDatosDataCoes()
            {
                FechaInicio = primerDiaDelMes,
                FechaFin = ultimoDiaDelMes,
                Fecha = DateTime.Parse(request.Fecha)
            });

            _context.AddRange(datosCoes.Detalle);

            Task.CompletedTask.Wait();
        }
    }
}
