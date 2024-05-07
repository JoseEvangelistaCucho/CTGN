using Generacion.Application.Common;
using Generacion.Application.Funciones;
using Generacion.Application.ReporteProduccion;
using Generacion.Models;
using Generacion.Models.FotoVoltaica;
using Generacion.Models.ReporteProduccion;
using MediatR;

namespace Generacion.Application.Fotovoltaica.Command
{
    public class RegistroReporteFotoVoltaica : IRequest<Respuesta<string>>
    {
        public List<DetalleFotovoltaica>? DetalleFotovoltaicas { get; set; }
        public List<FotovoltaicaGenerada>? DetalleGenerada { get; set; }
        public ReporteFotovoltaica? ReporteFotovoltaica { get; set; }
        public List<RegistroEventos> RegistroEventos { get; set; }
    }
    public class RegistroReporteFotoVoltaicaHandler : IRequestHandler<RegistroReporteFotoVoltaica, Respuesta<string>>
    {
        private readonly IGuardarDatosFotovoltaica _guardarDatos;
        private readonly ProcessExecutionContextExtensions _context;
        private readonly FotoServidor _fotoServidor;
        private readonly IReporteProduccion _reporteProduccion;
        public RegistroReporteFotoVoltaicaHandler(
            IReporteProduccion reporteProduccion,
            IGuardarDatosFotovoltaica guardarDatos,
            FotoServidor fotoServidor,
            ProcessExecutionContextExtensions context)
        {
            _reporteProduccion = reporteProduccion;
            _fotoServidor = fotoServidor;
            _guardarDatos = guardarDatos;
            _context = context;
        }
        public async Task<Respuesta<string>> Handle(RegistroReporteFotoVoltaica request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                respuesta = await _reporteProduccion.InsertOrUpdateRegistroEventos(request.RegistroEventos);

                request.ReporteFotovoltaica.ImagenReporte = await GuardarImagen(request.ReporteFotovoltaica.ImagenReporte);

                respuesta = await _guardarDatos.GuardarDatosReporte(request.ReporteFotovoltaica);
                foreach (var item in request.DetalleGenerada)
                {
                    respuesta = await _guardarDatos.GuardarDetalleReporte(item);
                }

                foreach (var item in request.DetalleFotovoltaicas)
                {
                    respuesta = await _guardarDatos.GuardarStatusGenerados(item);
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<string> GuardarImagen(string imagenBase64)
        {
            string respuesta = string.Empty;
            try
            {
                string base64Data = imagenBase64.Split(',')[1];
                byte[] imageBytes = Convert.FromBase64String(base64Data);

                _fotoServidor.GuardarArchivos(imageBytes, 1, 1.ToString(), $"\\FotoVoltaica\\{DateTime.Now.ToString("yyyy")}\\{DateTime.Now.ToString("MM")}", "png", DateTime.Now.ToString("dd-MM-yyyy"));

                var rutasArchivos = (Dictionary<int, string>)_context["rutaArchivoGuardado"];

                respuesta = rutasArchivos[1];
            }
            catch (Exception ex)
            {
            }

            return respuesta;
        }
    }
}
