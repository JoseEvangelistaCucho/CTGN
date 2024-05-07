using System.Runtime.Caching;

namespace Generacion.Application.DataBase.cache
{
    public class CacheDatos
    { 
        private static MemoryCache cache = MemoryCache.Default;

        public string ObtenerContenidoCache(string id)
        {
            var datosJsonEnCache = cache[id] as string;

            return datosJsonEnCache ?? string.Empty;
        }

        public void GuardarDatosCache(string id, string json)
        {
            cache.Remove(id);
            cache.Add(id, json, DateTimeOffset.Now.AddHours(8));
        }
        public void BorrarDatosCache(string id)
        {
            cache.Remove(id);
        }

        public void GuardarDatosCacheOracle()
        {

        }
    }
}
