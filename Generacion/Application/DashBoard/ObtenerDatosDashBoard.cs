using Generacion.Application.Common;
using Generacion.Application.DashBoard.Bujias.Query;
using Generacion.Application.DashBoard.CalibracionValvula.Query;
using Generacion.Application.DashBoard.CambioAceite.Query;
using Generacion.Application.DashBoard.ControlGAS.Query;
using Generacion.Application.DashBoard.LavadoTurbo.Query;
using Generacion.Application.DashBoard.TKVessel.Query;
using Generacion.Application.DashBoard.TurboCompresor.Query;
using Generacion.Application.FiltroCentrifugo.Command;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Aceite;
using Generacion.Models.ControlGAS;
using Generacion.Models.TurboCompresor;
using MediatR;
using NuGet.Packaging;

namespace Generacion.Application.DashBoard
{
    public class ObtenerDatosDashBoard : IRequest<Respuesta<Dictionary<string, object>>>
    {
    }

    public class ObtenerDatosDashBoardHanlder : IRequestHandler<ObtenerDatosDashBoard, Respuesta<Dictionary<string, object>>>
    {
        private readonly ISender _sender;
        private readonly Function _function;
        private readonly ObtenerDetalleConsumoGas _obtenerDetalleConsumoGas;

        public ObtenerDatosDashBoardHanlder(ISender sender, ObtenerDetalleConsumoGas obtenerDetalleConsumoGas, Function function)
        {
            _obtenerDetalleConsumoGas = obtenerDetalleConsumoGas;
            _sender = sender;
            _function = function;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerDatosDashBoard request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();
            respuesta.Detalle = new Dictionary<string, object>();

            await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDatosFiltro());
            await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDetalleConsumoGas());
            await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDatosControlAceite());
            await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDatosTurboCompresor());
            await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDatosLavadoTurbo());
            await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDetalleCalibracion());
            await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDetalleTKVessel());
            await _function.AgregarDatosKeyString(respuesta.Detalle, await ObtenerDuracionBujiasPorGE());

            return respuesta;
        }


        public async Task<Dictionary<string, object>> ObtenerDuracionBujiasPorGE()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            var datosTKVessel = await _sender.Send(new ObtenerUltimoRegistroBujia());

            respuesta.Add("datosDuracionBujias", datosTKVessel.Detalle);

            return respuesta;
        }
        public async Task<Dictionary<string, object>> ObtenerDetalleTKVessel()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            var datosTKVessel = await _sender.Send(new ObtenerDetalledatosTKVesselPorGE());

            respuesta.Add("datosTKVessel", datosTKVessel.Detalle);

            return respuesta;
        }

        public async Task<Dictionary<string, object>> ObtenerDetalleCalibracion()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            var datosCalibracionValvula = await _sender.Send(new ObtenerDetalleCalibracion());

            respuesta.Add("datosCalibracionValvula", datosCalibracionValvula.Detalle);

            return respuesta;
        }


        public async Task<Dictionary<string, object>> ObtenerDatosLavadoTurbo()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            var datosLavadoTurbo = await _sender.Send(new obtenerDatoLavadoTurbo());

            var detalleLavadoTurbo = new Respuesta<Dictionary<int, Dictionary<string, DetalleLavadoTurbo>>>();
            if (datosLavadoTurbo.Detalle != null)
            {
                detalleLavadoTurbo = await _sender.Send(new obtenerDetalleLavadoTurbo()
                {
                    idLavado = datosLavadoTurbo.Detalle.IdLavadoTurbo
                });
            }

            respuesta.Add("detalleLavadoTurbo", detalleLavadoTurbo.Detalle);

            return respuesta;
        }

        public async Task<Dictionary<string, object>> ObtenerDatosTurboCompresor()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();


            var datosTurboCompresor = await _sender.Send(new ObtenerIDDatosTurboCompresor());
            respuesta.Add("datosTurboCompresor", datosTurboCompresor.Detalle);

            var detalleTurboCompresor = new Respuesta<Dictionary<int, Dictionary<string, Dictionary<int, Dictionary<string, DetalleTurboCompresor>>>>>();
            if (datosTurboCompresor.Detalle != null)
            {
                detalleTurboCompresor = await _sender.Send(new ObtenerDatosTurboCompresorPorID()
                {
                    idTurboCompresor = datosTurboCompresor.Detalle.IdTurboCompresor
                });
            }
            respuesta.Add("detalleTurboCompresor", detalleTurboCompresor.Detalle);

            return respuesta;
        }

        public async Task<Dictionary<string, object>> ObtenerDatosControlAceite()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            var datosControlAceite = await _sender.Send(new ObtenerDatosAceitePorEGyTipo());
            respuesta.Add("datosControlAceite", datosControlAceite.Detalle);

            Respuesta<Dictionary<int, DetalleControlAceite>> detalleControlAceite = await _sender.Send(new ObtenerDetallesAceitePorEGyTipo());
            respuesta.Add("detalleControlAceite", detalleControlAceite.Detalle);

            return respuesta;
        }
        public async Task<Dictionary<string, object>> ObtenerDetalleConsumoGas()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            var datoContrato = await _obtenerDetalleConsumoGas.ObtenerContratoGas();
            respuesta.Add("datoContrato", datoContrato.Detalle);

            var datosConsumo = await _obtenerDetalleConsumoGas.ObtenerDatosConsumoGas();
            respuesta.Add("datosConsumo", datosConsumo.Detalle);

            var datodetalleConsumo = datosConsumo.Detalle != null
                                    ? await _obtenerDetalleConsumoGas.ObtenerDetalleDiarioConsumoGas(datosConsumo.Detalle.IdConsumoGas)
                                    : new Respuesta<DetalleConsumoGas>();
            respuesta.Add("datodetalleConsumo", datodetalleConsumo.Detalle);

            return respuesta;

        }

        public async Task<Dictionary<string, object>> ObtenerDatosFiltro()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            MantenimientoComponentes requestCentrifugo = new MantenimientoComponentes()
            {
                TipoComponente = TipoComponente.filtroCentrifugo,
                RequiereId = true,
                Seleccion = string.Empty
            };

            var respuestaCentrifugo = await _sender.Send(requestCentrifugo);

            MantenimientoComponentes requestAutomatico = new MantenimientoComponentes()
            {
                TipoComponente = TipoComponente.filtroAutomatico,
                RequiereId = true,
                Seleccion = string.Empty
            };

            var respuestaAutomatico = await _sender.Send(requestAutomatico);

            respuesta["datosFiltroCentrifugo"] = respuestaCentrifugo.Detalle["datosDashboard"];
            respuesta["datosFiltroAutomatico"] = respuestaAutomatico.Detalle["datosDashboard"];

            return respuesta;
        }
    }
}
