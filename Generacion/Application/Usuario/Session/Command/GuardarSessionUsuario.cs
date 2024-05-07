using Generacion.Application.Funciones;
using Generacion.Application.Usuario.Query;
using Generacion.Models;
using MediatR;

namespace Generacion.Application.Usuario.Session.Command
{
    public class GuardarSessionUsuario : IRequest<Respuesta<string>>
    {
        public string Detalle { get; set; }
        public string RutaAccion { get; set; }

    }

    public class GuardarSessionUsuarioHandler : IRequestHandler<GuardarSessionUsuario, Respuesta<string>>
    {
        private readonly GuardarDatosDeSession _guardarDatosDeSession;
        private readonly ConsultarUsuario _consultarUsuario;
        
        public GuardarSessionUsuarioHandler(ConsultarUsuario consultarUsuario, GuardarDatosDeSession guardarDatosDeSession)
        {
            _consultarUsuario = consultarUsuario;
            _guardarDatosDeSession = guardarDatosDeSession;
        }
        public async Task<Respuesta<string>> Handle(GuardarSessionUsuario request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = null;
            try
            {
                DatosSession datos = new DatosSession()
                {
                    Accion = "",//validar como obtener la accion o procedimiento que se ejecutara
                    RutaAccion = request.RutaAccion,
                    Descripcion = request.Detalle,
                    
                };

                await _guardarDatosDeSession.GuardarDatosSession(datos);

                var idTurno = await ObtenerTurnoHorario();

                await _guardarDatosDeSession.GuardarSessionUsuario(idTurno);

            }
            catch (Exception ex)
            {

            }

            return respuesta;

        }



        public async Task<string> ObtenerTurnoHorario()
        {
            string respuesta = string.Empty;
            var horarioOperario = await _consultarUsuario.ObtenerTurnos();
            DateTime horaActual = DateTime.Now;
            int hora = horaActual.Hour.Equals(0) ? 12 : horaActual.Hour;
            int horaSinFormatear = horaActual.Hour;

            if (hora >= horarioOperario.Detalle[0].Hora && horaSinFormatear < horarioOperario.Detalle[1].Hora)
            {
                respuesta = horarioOperario.Detalle[0].idTurno;
            }
            else if (hora >= horarioOperario.Detalle[1].Hora && hora < horarioOperario.Detalle[2].Hora)
            {
                respuesta =  horarioOperario.Detalle[1].idTurno;
            }
            else
            {
                respuesta = horarioOperario.Detalle[2].idTurno;
            }

            return respuesta;
        }
    }
}
