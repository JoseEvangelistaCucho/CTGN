using Generacion.Models.ReporteProduccion;
using Generacion.Models;

namespace Generacion.Application.ReporteProduccion
{
    public interface IReporteProduccion
    {
        Task<Respuesta<string>> GuardarDatosEnergiaProducida(EnergiaProducida datoEnergiaProducida);
        Task<int> InsertOrUpdateProduccionStatus(List<ReporteProduccionStatus> reportes);
        Task<Respuesta<string>> InsertOrUpdateDetalleProduccion(List<DetalleProduccion> detalle);
        Task<Respuesta<string>> InsertOrUpdateArranqueSincro(List<ArranqueSincronizacion> detalle);
        Task<Respuesta<string>> InsertOrUpdateRegistroEventos(List<RegistroEventos> detalle);
        Task<Respuesta<string>> GuardarDatosLevelOilCarter(LevelLubeOilCartel datoLevelOilCarter);
        Task<Respuesta<string>> GuardarDatosRefCarter(RefrescamientoCartel datoRefCarter);
        Task<Respuesta<string>> GuardarDatosCityGate(CityGateFlow datoCityGate);
        Task<Respuesta<string>> GuardarDatosManttoVessel(ManttoVessel datoManttoVessel);
        Task<Respuesta<string>> GuardarDatosTkCleanLube(TkCleanLube datoTkCleanLube);
        Task<Respuesta<string>> GuardarDatosReporteProduccion(ReporteProducion datos);

    }

}
