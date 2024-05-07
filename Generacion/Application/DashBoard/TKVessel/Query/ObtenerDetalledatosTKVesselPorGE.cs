using Generacion.Application.Funciones;
using Generacion.Application.Mantenimiento.Query;
using Generacion.Models;
using Generacion.Models.Mantenimiento;
using MediatR;

namespace Generacion.Application.DashBoard.TKVessel.Query
{
    public class ObtenerDetalledatosTKVesselPorGE : IRequest<Respuesta<Dictionary<string,Dictionary<int, CilindroAceiteCarter>>>>
    {
    }

    public class ObtenerDetalledatosTKVesselHandler : IRequestHandler<ObtenerDetalledatosTKVesselPorGE, Respuesta<Dictionary<string, Dictionary<int, CilindroAceiteCarter>>>>
    {
        private readonly DatosMantenimiento _mantenimiento;
        public ObtenerDetalledatosTKVesselHandler(DatosMantenimiento datosMantenimiento)
        {
            _mantenimiento = datosMantenimiento;
        }
        public async Task<Respuesta<Dictionary<string, Dictionary<int, CilindroAceiteCarter>>>> Handle(ObtenerDetalledatosTKVesselPorGE request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, Dictionary<int, CilindroAceiteCarter>>> respuesta = new Respuesta<Dictionary<string, Dictionary<int, CilindroAceiteCarter>>>();

            try
            {
                respuesta.Detalle = new Dictionary<string, Dictionary<int, CilindroAceiteCarter>>();

                var datosConsultaTKVesselNuevo = await _mantenimiento.ObtenerRegistroTKVesselPorFecha(DateTime.Now.ToString("dd/MM/yyyy"), 1);

                var datosConsultaTKVesselAnterior = await _mantenimiento.ObtenerRegistroTKVesselPorFecha(DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"), 1);


                if (datosConsultaTKVesselNuevo.Detalle.Count> 0)
                {
                    respuesta.Detalle.Add("Nuevo", datosConsultaTKVesselNuevo.Detalle.ToDictionary(x => x.NumeroGenerador));

                }

                if (datosConsultaTKVesselAnterior.Detalle.Count > 0)
                {
                    respuesta.Detalle.Add("Anterior", datosConsultaTKVesselAnterior.Detalle.ToDictionary(x => x.NumeroGenerador));
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
