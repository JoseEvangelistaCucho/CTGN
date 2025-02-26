using Generacion.Application.Funciones;
using Generacion.Application.ReporteProduccion;
using Generacion.Models;
using Generacion.Models.Bess;
using Generacion.Models.ReporteProduccion;
using MediatR;

namespace Generacion.Application.BessLlipata.ReporteProduccion.Command
{    
    public class ProduccionBessLlipata : IRequest<Respuesta<string>>
    {
        public DBBessLlipata DBBessLlipata { get; set; }
        public List<RegistroEventos> RegistroEventos { get; set; }
        public List<DetalleProduccion> DetalleProduccion { get; set; }
        public PortenciaMarconaVSBess DatosPortenciaMarconaVSBess { get; set; }
        public DemandaCoesVSBess DemandaCoesVSBess { get; set; }
        public DemandavsCoincidente DatosDemandavsCoincidente { get; set; }
        public CampoReporte CampoReporte { get; set; }
    }
    public class ProduccionBessLlipataHandler : IRequestHandler<ProduccionBessLlipata, Respuesta<string>>
    {
        private readonly IReporteProduccion _reporteProduccion;
        private readonly IReporteBessLlipata _reporteBessLlipata;
        private readonly Logger _logger;


        public ProduccionBessLlipataHandler(IReporteProduccion reporteProduccion, IReporteBessLlipata reporteBessLlipata, Logger logger)
        {
            _reporteProduccion = reporteProduccion;
            _reporteBessLlipata = reporteBessLlipata;
            _logger = logger;
        }

        public async Task<Respuesta<string>> Handle(ProduccionBessLlipata request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                respuesta = await GuardarDatosEvento(request.RegistroEventos);
                respuesta = await GuardarDetallesReporte(request.DetalleProduccion);
                respuesta = await GuardarDetalleReporte(request.DBBessLlipata);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error ProduccionBessLlipataHandler : " + ex.Message.ToString());
            }

            return respuesta;
        }
        public async Task<Respuesta<string>> GuardarDetalleReporte(DBBessLlipata registros)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                respuesta = await _reporteBessLlipata.GuardarDatosReporte(registros);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GuardarDetalleReporte : " + ex.Message.ToString());
            }
            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarDatosEvento(List<RegistroEventos> registros)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                respuesta = await _reporteProduccion.InsertOrUpdateRegistroEventos(registros);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GuardarDatosEvento : " + ex.Message.ToString());
            }
            return respuesta;
        }

        public async Task<Respuesta<string>> GuardarDetallesReporte(List<DetalleProduccion> registros)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                var respuestaDetalle = await _reporteProduccion.InsertOrUpdateDetalleProduccion(registros);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GuardarDetallesReporte : " + ex.Message.ToString());
            }
            return respuesta;
        }
    }
}
