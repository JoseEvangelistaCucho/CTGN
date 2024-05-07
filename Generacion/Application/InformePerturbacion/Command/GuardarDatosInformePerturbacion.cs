using Generacion.Application.Common;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.InformePerturbacion;
using MediatR;

namespace Generacion.Application.InformePerturbacion.Command
{
    public class GuardarDatosInformePerturbacion : IRequest<Respuesta<string>>
    {
        public InformeGeneralPerturbacion InformeGeneralPerturbacion { get; set; }
        public DetalleInformePerturbacion DetalleInformePerturbacion { get; set; }
        public List<SecuenciaCronologica> SecuenciaCronologica { get; set; }
        public List<SuministrosInterrumpidos> SuministrosInterrumpidos { get; set; }
        public List<string> Imagenesbase64 { get; set; }
    }

    public class GuardarDatosInformePerturbacionHandler : IRequestHandler<GuardarDatosInformePerturbacion, Respuesta<string>>
    {
        private readonly IRegistroDatosPerturbacion _registroDatosPerturbacion;
        private readonly FotoServidor _fotoServidor;
        private readonly ProcessExecutionContextExtensions _context;
        public GuardarDatosInformePerturbacionHandler(IRegistroDatosPerturbacion registroDatosPerturbacion, FotoServidor fotoServidor, ProcessExecutionContextExtensions context)
        {
            _context = context;
            _fotoServidor = fotoServidor;
            _registroDatosPerturbacion = registroDatosPerturbacion;
        }
        public async Task<Respuesta<string>> Handle(GuardarDatosInformePerturbacion request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                respuesta = await _registroDatosPerturbacion.GuardarDatosPrincipal(request.InformeGeneralPerturbacion);

                string rutaImagenes = await GuardarImagen(request.Imagenesbase64);

                respuesta = await _registroDatosPerturbacion.GuardarDetallesPrincipal(request.DetalleInformePerturbacion, rutaImagenes);

                foreach (var item in request.SecuenciaCronologica)
                {
                    respuesta = await _registroDatosPerturbacion.GuardarSecuenciaPrincipal(item);
                }
                foreach (var item in request.SuministrosInterrumpidos)
                {
                    respuesta = await _registroDatosPerturbacion.GuardarSuministrosinterrumpidos(item);
                }

                request.Imagenesbase64 = rutaImagenes.Split("|#|").ToList();

            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la petición.";
            }

            return respuesta;
        }

        public async Task<string> GuardarImagen(List<string> imagenBase64)
        {
            string respuesta = string.Empty;
            try
            {
                int index = 0;
                foreach (var item in imagenBase64)
                {
                    string base64Data = item.Split(',').Last();
                    byte[] imageBytes = Convert.FromBase64String(base64Data);

                    _fotoServidor.GuardarArchivos(imageBytes, index, index.ToString(), $"/InformePerturbacion/{DateTime.Now.ToString("yyyy")}/{DateTime.Now.ToString("MM")}", "png", DateTime.Now.ToString("dd-MM-yyyy"));
                    index++;
                }

                Dictionary<int, string> rutasGuardadas = (Dictionary<int, string>)_context["rutaArchivoGuardado"];

                foreach (var item in rutasGuardadas)
                {
                    respuesta = $"{item.Value}|#|{respuesta}";
                }

            }
            catch (Exception ex)
            {
            }

            return respuesta;
        }
    }
}
