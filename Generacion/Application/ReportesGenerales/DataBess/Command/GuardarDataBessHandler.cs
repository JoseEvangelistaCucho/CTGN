using Generacion.Models;
using MediatR;
using Generacion.Models.Bess;

namespace Generacion.Application.ReportesGenerales.DataBess.Command
{
    public class GuardarDatosBess : IRequest<Respuesta<List<DBDataBess>>>
    {
        public IFormFile Archivo { get; set; }
    }
    public class GuardarDataBessHandler : IRequestHandler<GuardarDatosBess, Respuesta<List<DBDataBess>>>
    {
        public async Task<Respuesta<List<DBDataBess>>> Handle(GuardarDatosBess request, CancellationToken cancellationToken)
        {
            Respuesta<List<DBDataBess>> respuesta = new Respuesta<List<DBDataBess>>();
            try
            {

                using (var reader = new StreamReader(request.Archivo.OpenReadStream()))
                {
                    string contenido = reader.ReadToEnd();
                    string[] lineas = contenido.Split('\n');

                    respuesta.Detalle = new List<DBDataBess>();
                    DBDataBess datos = new DBDataBess();
                    bool primerElemento = true;
                    foreach (string linea in lineas)
                    {
                        if (!primerElemento && !string.IsNullOrEmpty(linea))
                        {
                            string[] valores = linea.Split('\t');

                            string fechaArchivo = await ObtenerValorEnPosicion(valores, 0);
                            string horaArchivo = await ObtenerValorEnPosicion(valores, 1);
                            DateTime fechaParseado = DateTime.Now;
                            string[] formatos = { "MM/dd/yyyy", "M/dd/yyyy" };
                            if (DateTime.TryParseExact(fechaArchivo, formatos, null, System.Globalization.DateTimeStyles.None, out DateTime fecha))
                            {
                                fechaParseado = fecha;
                            }

                            datos = new DBDataBess()
                            {
                                IdDataBess = $"data-bess_{fechaParseado.ToString("dd-MM-yyyy")}{horaArchivo.Split(' ')[0].Replace(':','-')}",
                                Fecha = fechaParseado,
                                Hora = horaArchivo.Split(' ')[0].Length == 7 ? $"0{horaArchivo.Split(' ')[0]}": horaArchivo.Split(' ')[0],
                                LLIXI625_PV = decimal.Parse(await ObtenerValorEnPosicion(valores, 2)),
                                LLIXI622_PV = decimal.Parse(await ObtenerValorEnPosicion(valores, 3)),
                                LLIXI633_PV = decimal.Parse(await ObtenerValorEnPosicion(valores, 4)),
                                LLIXI634_PV = decimal.Parse(await ObtenerValorEnPosicion(valores, 5)),
                                LLIXI634_PV2 = decimal.Parse(await ObtenerValorEnPosicion(valores, 6))
                            };
                            respuesta.Detalle.Add(datos);
                        }
                        primerElemento = false;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }

        public async Task<string> ObtenerValorEnPosicion(string[] array, int posicion)
        {
            string respuesta = "";

            if (posicion >= 0 && posicion < array.Length)
            {
                respuesta = array[posicion];

                if (string.IsNullOrEmpty(respuesta))
                {
                    respuesta = "0";
                }
            }
            else
            {
                respuesta = "0";
            }
            return respuesta;
        }
    }
}
