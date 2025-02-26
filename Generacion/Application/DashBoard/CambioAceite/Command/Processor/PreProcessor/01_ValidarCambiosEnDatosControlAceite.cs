using Generacion.Application.Common;
using Generacion.Application.DataBase.cache;
using Generacion.Application.Funciones;
using Generacion.Models.Aceite;
using MediatR.Pipeline;
using Newtonsoft.Json;

namespace Generacion.Application.DashBoard.CambioAceite.Command.Processor.PreProcessor
{
    public class _01_ValidarCambiosEnDatosControlAceite : IRequestPreProcessor<ListaControlCambioAceite>
    {
        private readonly Logger _logger;
        private readonly CacheDatos _cacheDatos;
        private readonly Function _function;
        private readonly ProcessExecutionContextExtensions _executionContextExtensions;
        public _01_ValidarCambiosEnDatosControlAceite(
            ProcessExecutionContextExtensions executionContextExtensions,
            Function function,
            CacheDatos cacheDatos,
            Logger logger
            )
        {
            _executionContextExtensions = executionContextExtensions;
            _function = function;
            _cacheDatos = cacheDatos;
            _logger = logger;
        }
        public async Task Process(ListaControlCambioAceite request, CancellationToken cancellationToken)
        {
            try
            {
                string datojson = _cacheDatos.ObtenerContenidoCache("DashBoardControlAceite");

                if (!string.IsNullOrEmpty(datojson))
                {
                    Dictionary<int, Dictionary<string, ControlCambioAceite>> datosPorGEyTipoActual = await _function.ConvertirDiccionarioControlPorGEyTipo(request.datosControl);

                    var datosPorGEyTipoAnterior = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, ControlCambioAceite>>>(datojson);

                    var datosActualizados = new List<ControlCambioAceite>();
                    foreach (var kvpActual in datosPorGEyTipoActual)
                    {
                        int numeroGenerador = kvpActual.Key;

                        if (datosPorGEyTipoAnterior.TryGetValue(numeroGenerador, out var diccionarioAnterior))
                        {
                            foreach (var kvpTipoActual in kvpActual.Value)
                            {
                                string tipo = kvpTipoActual.Key;
                                ControlCambioAceite controlActual = kvpTipoActual.Value;

                                if (diccionarioAnterior.TryGetValue(tipo, out var controlAnterior))
                                {
                                    if (controlActual.HorasCambio != controlAnterior.HorasCambio)
                                     //   || (controlActual.FechaCambio != controlAnterior.FechaCambio))
                                    {
                                        ControlCambioAceite datos = controlAnterior;
                                        datos.Activo = 1;
                                        datosActualizados.Add(datos);
                                    }
                                }
                            }
                        }
                    }
                    if (datosActualizados.Count>0)
                    {
                        _executionContextExtensions.TryAdd("DatosModificados", datosActualizados);
                    }
                }
            }
            catch (Exception ex) { 
                _logger.LogError("Error ObtenerDatosBESSQueryHandler : " + ex.Message.ToString());
            }
        }
    }
}
