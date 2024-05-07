using Generacion.Application.Common;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.ION;
using MediatR;

namespace Generacion.Application.ION.Query
{
    public class ObtenerDatosIONQuery : IRequest<Respuesta<List<DatosFormatoMGD>>>
    {
        public DateTime Fecha { get; set; }
    }

    public class ObtenerDatosIONQueryHandler : IRequestHandler<ObtenerDatosIONQuery, Respuesta<List<DatosFormatoMGD>>>
    {
        private readonly ConsultarION _consultarION;
        private readonly Function _function;

        public ObtenerDatosIONQueryHandler(Function function, ConsultarION consultarION)
        {
            _function = function;
            _consultarION = consultarION;
        }
        public async Task<Respuesta<List<DatosFormatoMGD>>> Handle(ObtenerDatosIONQuery request, CancellationToken cancellationToken)
        {
            Respuesta<List<DatosFormatoMGD>> respuesta = new Respuesta<List<DatosFormatoMGD>>();

            try
            {
                var datosOperario = await _function.ObtenerDatosOperario();
                string codigoION = datosOperario.IdSitio.Equals(Constante.codigoCentralLuren) ? Constante.codigoIONPedregal : Constante.codigoIONLuren;

                TimeSpan horaEspecifica = new TimeSpan(5, 0, 0);
                respuesta = await _consultarION.ObtenerDatosIONSQL(codigoION, Constante.codigosION, request.Fecha+ horaEspecifica);

                respuesta.Detalle = respuesta.Detalle.OrderBy(x => DateTime.Parse(x.Date_Time)).ToList();
            }
            catch (Exception ex)
            {
            }

            return respuesta;
        }
    }
}
