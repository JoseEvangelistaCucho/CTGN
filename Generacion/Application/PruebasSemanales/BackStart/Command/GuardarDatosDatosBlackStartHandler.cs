using Generacion.Models;
using Generacion.Models.PruebasSemanales.BlackStart;
using MediatR;

namespace Generacion.Application.PruebasSemanales.BackStart.Command
{
    public class GuardarDatosPruebaSemanal : IRequest<Respuesta<object>>
    {
        public List<DetallePruebaSemanal> DetallePruebaSemanal { get; set; }
        public PruebaSemanal? PruebaSemanal { get; set; }
        public string Tipo { get; set; }
    }


    public class GuardarDatosDatosPruebaSemanalHandler : IRequestHandler<GuardarDatosPruebaSemanal, Respuesta<object>>
    {
        private readonly IRegistroDatosPruebaSemanal _registroDatos;

        public GuardarDatosDatosPruebaSemanalHandler(IRegistroDatosPruebaSemanal registroDatos)
        {
            _registroDatos = registroDatos;
        }
        public async Task<Respuesta<object>> Handle(GuardarDatosPruebaSemanal request, CancellationToken cancellationToken)
        {
            Respuesta<object> respuesta = new Respuesta<object>();

            try
            {
                int index = 0;
                Respuesta<string> respuestaDetalle = new Respuesta<string>();
                foreach (var item in request.DetallePruebaSemanal)
                {
                    if (respuestaDetalle.IdRespuesta == 99)
                        continue;

                    respuestaDetalle = await _registroDatos.GuardarDetallePruebaSemanal(item);   
                }

                if (respuestaDetalle.IdRespuesta.Equals(0))
                {                   
                    respuestaDetalle = await _registroDatos.GuardarDatosPruebaSemanal(request.PruebaSemanal, request.Tipo);
                }
                respuesta.IdRespuesta = respuestaDetalle.IdRespuesta;

            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }
    }
}
