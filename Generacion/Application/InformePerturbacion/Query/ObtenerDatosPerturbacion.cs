using Generacion.Models;
using Generacion.Models.InformePerturbacion;
using MediatR;
using System.Linq;

namespace Generacion.Application.InformePerturbacion.Query
{
    public class ObtenerDatosPerturbacion : IRequest<Respuesta<Dictionary<string,object>>>
    {
        public string Fecha {  get; set; }
        public string Hora {  get; set; }
    }
    public class ObtenerDatosPerturbacionHandler : IRequestHandler<ObtenerDatosPerturbacion, Respuesta<Dictionary<string, object>>>
    {
        private readonly DatosPerturbacion _datosPerturbacion;
        public ObtenerDatosPerturbacionHandler(DatosPerturbacion datosPerturbacion)
        {
            _datosPerturbacion = datosPerturbacion;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerDatosPerturbacion request, CancellationToken cancellationToken)
        {
            Respuesta < Dictionary<string, object> > respuesta = new Respuesta<Dictionary<string, object>>();
            try
            {
                Respuesta<InformeGeneralPerturbacion> datosInforme = await _datosPerturbacion.ObtenerDatosPrincipal(request.Fecha, request.Hora);
                Respuesta<DetalleInformePerturbacion> datosPerturbacion = await _datosPerturbacion.ConsultarDetalle(request.Fecha, request.Hora);
                Respuesta<List<SecuenciaCronologica>> datosSecuenciaCronologico= await _datosPerturbacion.ObteterSecuentaCronologico(request.Fecha, request.Hora);
                Respuesta<List<SuministrosInterrumpidos>> datosSumInterrumpido= await _datosPerturbacion.ObtenerSuministrosinterrumpidos(request.Fecha, request.Hora);

                if (datosPerturbacion.Detalle != null)
                {
                    datosPerturbacion.Detalle.RutaImagenes = await ObtenerImagenes(datosPerturbacion.Detalle.RutaImagenes);
                }

                respuesta.Detalle = new Dictionary<string, object>();
                respuesta.Detalle.Add("reload",true);
                respuesta.Detalle.Add("datosPrincipal", datosInforme.Detalle);
                respuesta.Detalle.Add("datosDetallePerturbacion", datosPerturbacion.Detalle); 
                respuesta.Detalle.Add("datosSumInterrumpido", datosSumInterrumpido.Detalle);
                respuesta.Detalle.Add("datosSecuencia",await ObtenerSecuenciaPorItem(datosSecuenciaCronologico.Detalle));

            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<Dictionary<int, List<SecuenciaCronologica>>> ObtenerSecuenciaPorItem(List<SecuenciaCronologica> datos)
        {
            Dictionary<int, List<SecuenciaCronologica>> respuesta = new Dictionary<int, List<SecuenciaCronologica>>();

            try
            {
                respuesta = datos.OrderBy(x => x.Posicion).GroupBy(x => x.Posicion)
                                 .ToDictionary(x => x.Key, x => x.ToList());
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
