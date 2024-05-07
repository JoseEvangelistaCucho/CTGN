using Generacion.Application.Common;
using Generacion.Application.Funciones;
using Generacion.Application.RAM.Query;
using Generacion.Models;
using Generacion.Models.ReporteRAM;
using MediatR;
using OfficeOpenXml;

namespace Generacion.Application.RAM.Command
{
    public class GuardarRegistroRAM : IRequest<Respuesta<Dictionary<string, object>>>
    {
        //public string Tipo { get; set; }
        public List<ViewOIL> VistaAceite { get; set; }
        public List<DBRAM>? DBRAM { get; set; }
    }
    public class DBRAMHandler : IRequestHandler<GuardarRegistroRAM, Respuesta<Dictionary<string, object>>>
    {
        private readonly IRegistroRAM _registroRAM;
        private readonly ProcessExecutionContextExtensions _executionContextExtensions;
        private readonly Function _function;
        private readonly IMediator _sender;
        public DBRAMHandler(Function function, IRegistroRAM registroRAM, IMediator sender, ProcessExecutionContextExtensions executionContextExtensions)
        {
            _function = function;
            _executionContextExtensions = executionContextExtensions;
            _registroRAM = registroRAM;
            _sender = sender;
        }

        public async Task<Respuesta<Dictionary<string, object>>> Handle(GuardarRegistroRAM request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();

            try
            {
                //  if (request.Tipo.Contains("oil"))
                //  {
                Respuesta<string> respuestaRam = new Respuesta<string>();

                foreach (var item in request.VistaAceite)
                {
                    respuestaRam = await _registroRAM.GuardarDatosRAMOil(item);
                }
                //   }
                ////    else
                //    {

                var ram = request.DBRAM.Where(x => DateTime.Parse(x.Fecha).Month == DateTime.Now.Month).FirstOrDefault();
                respuestaRam = await _registroRAM.GuardarDatosRAM(ram);
                //    }

                var respuestaData = await _sender.Send(new DatosProduccionQuery()
                {
                    TipoBusqueda = "año",
                    Fecha = DateTime.Now.ToString("yyyy")
                });

                var respuestaEvento = await _sender.Send(new ObtenerDatosEvento()
                {
                    Fecha = DateTime.Now.ToString("yyyy"),
                    Tipo = "año"
                });

                var datosOperario = await _function.ObtenerDatosOperario();

                _executionContextExtensions.TryAdd("eventosPorFecha", respuestaEvento.Detalle["eventosPorFecha"]);
                _executionContextExtensions.TryAdd("datosRAM", respuestaData.Detalle);
                _executionContextExtensions.TryAdd("datosOperario", datosOperario);


            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }
    }
}
