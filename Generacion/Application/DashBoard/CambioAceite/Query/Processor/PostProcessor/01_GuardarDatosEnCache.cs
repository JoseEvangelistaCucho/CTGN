using Generacion.Application.DashBoard.CambioAceite.Command;
using Generacion.Application.DataBase.cache;
using Generacion.Application.Funciones;
using Generacion.Models;
using Generacion.Models.Aceite;
using MediatR;
using MediatR.Pipeline;
using Newtonsoft.Json;

namespace Generacion.Application.DashBoard.CambioAceite.Query.Processor.PostProcessor
{
    public class _01_GuardarDatosEnCache : IRequestPostProcessor<ObtenerDatosAceitePorEGyTipo ,Respuesta<Dictionary<int, Dictionary<string, ControlCambioAceite>>>>
    {
        private readonly CacheDatos _cacheDatos;
        public _01_GuardarDatosEnCache(
            CacheDatos cacheDatos
            )
        {
            _cacheDatos = cacheDatos;
        }
        public async Task Process(ObtenerDatosAceitePorEGyTipo request, Respuesta<Dictionary<int, Dictionary<string, ControlCambioAceite>>> response, CancellationToken cancellationToken)
        {
            string datojson = JsonConvert.SerializeObject(response.Detalle);

            _cacheDatos.GuardarDatosCache("DashBoardControlAceite", datojson);

            Task.CompletedTask.Wait();
        }
    }
}
