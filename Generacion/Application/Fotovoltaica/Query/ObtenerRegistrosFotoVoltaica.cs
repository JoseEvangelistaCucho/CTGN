using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.FotoVoltaica;
using MediatR;

namespace Generacion.Application.Fotovoltaica.Query
{
    public class ObtenerRegistrosFotoVoltaica : IRequest<Respuesta<Dictionary<string, object>>>
    {
        public string Fecha { get; set; }
    }
    public class ObtenerRegistrosFotoVoltaicaHandler : IRequestHandler<ObtenerRegistrosFotoVoltaica, Respuesta<Dictionary<string, object>>>
    {
        private readonly Function _funtion; 
        private readonly DatosFotovoltaica _datosFotovoltaica; 
        public ObtenerRegistrosFotoVoltaicaHandler(Function function, DatosFotovoltaica datosFotovoltaica)
        {
            _funtion = function;
            _datosFotovoltaica = datosFotovoltaica;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerRegistrosFotoVoltaica request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();
            try
            {
                var datosValoresCabecera = await _funtion.ObtenerDatosCabecera();

                Respuesta<ReporteFotovoltaica> datosReporte = await _datosFotovoltaica.ObtenerDatosFotovoltaica(request.Fecha);

                if (datosReporte.Detalle == null && request.Fecha== DateTime.Now.ToString("dd/MM/yyyy"))
                {
                    datosReporte = await _datosFotovoltaica.ObtenerDatosFotovoltaica(DateTime.Parse(request.Fecha).AddDays(-1).ToString("dd/MM/yyyy"));
                }

                if (datosReporte.Detalle != null)
                {
                    datosReporte.Detalle.ImagenReporte = await ObtenerImagenes(datosReporte.Detalle.ImagenReporte);
                }

                Respuesta<List<FotovoltaicaGenerada>> detalleGenerado = await _datosFotovoltaica.ObtenerDetalleGenerado(request.Fecha, DateTime.Parse(request.Fecha).Month);

                respuesta.Detalle = new Dictionary<string, object>();
                respuesta.Detalle.Add("datosCabecera", datosValoresCabecera);
                respuesta.Detalle.Add("datosReporte", datosReporte.Detalle);
                respuesta.Detalle.Add("detalleGenerado", detalleGenerado.Detalle);
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<string> ObtenerImagenes(string rutaImagenes)
        {
            string respuesta = string.Empty;

            string[] rutas = rutaImagenes.Split(new string[] { "|#|" }, StringSplitOptions.RemoveEmptyEntries);

            byte[][] imagenesBytes = new byte[rutas.Length][];

            for (int i = 0; i < rutas.Length; i++)
            {
                imagenesBytes[i] = File.ReadAllBytes(rutas[i].Trim());
            }

            string[] imagenesBase64 = new string[imagenesBytes.Length];
            for (int i = 0; i < imagenesBytes.Length; i++)
            {
                imagenesBase64[i] = Convert.ToBase64String(imagenesBytes[i]);
            }

            foreach (var imagenBase64 in imagenesBase64)
            {
                respuesta = $"{respuesta}<img src='data:image/png;base64,{imagenBase64}' alt='Imagen'>";
            }

            return respuesta;
        }
    }
}
