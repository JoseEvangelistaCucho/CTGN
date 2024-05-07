using Generacion.Models;
using Generacion.Models.Almacen;

namespace Generacion.Application.Almacen
{
    public interface IAlmacen
    {
        Task<Respuesta<string>> GuardarDatosAlmacen(Componentes componentes,string IdSitio);
        Task<Respuesta<string>> GuardarDatosPrestados(Prestamo prestamo);
        Task<Respuesta<string>> GuardarBujiasUtilizadas(int cantidad);
    }
}
