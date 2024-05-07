using Generacion.Application.Common;
using Generacion.Models;
using Generacion.Models.Aceite;
using MediatR;
using NuGet.Packaging;

namespace Generacion.Application.DashBoard.CambioAceite.Command
{
    public class ListaControlCambioAceite : IRequest<Respuesta<string>>
    {
        public List<ControlCambioAceite> datosControl { get; set; }
    }

    public class ControlCambioAceiteHandler : IRequestHandler<ListaControlCambioAceite, Respuesta<string>>
    {
        private readonly ProcessExecutionContextExtensions _executionContextExtensions;
        public readonly IRegistroControlAceite _registroControlAceite;
        public ControlCambioAceiteHandler(IRegistroControlAceite registroControlAceite,
            ProcessExecutionContextExtensions processExecutionContextExtensions)
        {
            _executionContextExtensions = processExecutionContextExtensions;
            _registroControlAceite = registroControlAceite;
        }
        public async Task<Respuesta<string>> Handle(ListaControlCambioAceite request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                List<ControlCambioAceite> datos = new List<ControlCambioAceite>();
                if (_executionContextExtensions.ContainsKey("DatosModificados"))
                {
                    datos = (List<ControlCambioAceite>)_executionContextExtensions["DatosModificados"];
                    request.datosControl.AddRange(datos);
                }

                foreach (var item in request.datosControl)
                {
                    if (respuesta.IdRespuesta == 99)
                        continue;

                    string mensajesError = item.ValidarPropiedadesNulasOVacias();

                    if (mensajesError.Any())
                    {
                        respuesta.IdRespuesta = 99;
                        respuesta.Mensaje = mensajesError;
                    }
                    else
                    {

                        respuesta = await _registroControlAceite.GuardarDatosControlAceite(item);
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al procesar la solicitud.";
            }
            return respuesta;
        }
    }
}
