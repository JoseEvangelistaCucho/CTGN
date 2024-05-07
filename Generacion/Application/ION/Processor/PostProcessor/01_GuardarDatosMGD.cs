using Generacion.Application.ION.Query;
using Generacion.Models.ION;
using Generacion.Models;
using MediatR.Pipeline;
using Generacion.Application.Funciones;
using Generacion.Models.MGD;
using Generacion.Models.Usuario;
using Generacion.Application.MGD;

namespace Generacion.Application.ION.Processor.PostProcessor
{
    public class _01_GuardarDatosMGD : IRequestPostProcessor<ObtenerDatosIONQuery, Respuesta<List<DatosFormatoMGD>>>
    {
        private readonly Function _function;
        private readonly IDatosMGD _datosMGD;
        private readonly IDatosION _datosION;

        public _01_GuardarDatosMGD(Function function, IDatosMGD datosMGD, IDatosION datosION)
        {
            _datosMGD = datosMGD;
            _function = function;
            _datosION = datosION;
        }
        public async Task Process(ObtenerDatosIONQuery request, Respuesta<List<DatosFormatoMGD>> response, CancellationToken cancellationToken)
        {
            try
            {
                DetalleOperario detalleOperario = await _function.ObtenerDatosOperario();

                if (response != null && response.Detalle.Count > 0)
                {
                    DatosMGD datosMGD = new DatosMGD()
                    {
                        Fecha = request.Fecha.ToString("yyyy-MM-dd"),
                        IdOperario = detalleOperario.IdOperario,
                        IdReporteMGD = $"MGD-{detalleOperario.IdSitio}-{request.Fecha.ToString("dd_MM_yyyy")}",
                        Idsitio = detalleOperario.IdSitio,
                        Revenue = response.Detalle
                    };
                    var respuestaMGD = await _datosMGD.GuardarDatosConsultaMGD(datosMGD);
                    var guardarDatosION = await _datosION.GuardarDatosION(datosMGD);

                }
            }
            catch (Exception ex) {

            }

            Task.CompletedTask.Wait();
        }
    }
}
