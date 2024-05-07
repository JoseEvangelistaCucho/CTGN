using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Aceite;
using MediatR;

namespace Generacion.Application.DashBoard.CambioAceite.Query
{
    public class ObtenerDatosAceitePorEGyTipo : IRequest<Respuesta<Dictionary<int,Dictionary<string, ControlCambioAceite>>>>
    {
    }

    public class ObtenerDatosAceitePorEGyTipoHanlder : IRequestHandler<ObtenerDatosAceitePorEGyTipo, Respuesta<Dictionary<int, Dictionary<string, ControlCambioAceite>>>>
    {
        private readonly DatosControlRegistroAceite _datosControlRegistro;
        private readonly Function _function;
        public ObtenerDatosAceitePorEGyTipoHanlder(DatosControlRegistroAceite datosControlRegistro, Function function)
        {
            _datosControlRegistro = datosControlRegistro;
            _function = function;
        }
        public async Task<Respuesta<Dictionary<int, Dictionary<string, ControlCambioAceite>>>> Handle(ObtenerDatosAceitePorEGyTipo request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<int, Dictionary<string, ControlCambioAceite>>> respuesta = new Respuesta<Dictionary<int, Dictionary<string, ControlCambioAceite>>>();

            var datos = await _datosControlRegistro.ObtenerDatosControlAceite();

            respuesta.IdRespuesta = datos.IdRespuesta;
            respuesta.Mensaje = datos.Mensaje;

            respuesta.Detalle = await _function.ConvertirDiccionarioControlPorGEyTipo(datos.Detalle);

            return respuesta;
        }
    }
}
