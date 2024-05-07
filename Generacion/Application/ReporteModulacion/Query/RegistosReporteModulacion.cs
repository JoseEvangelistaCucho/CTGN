using Generacion.Application.DataBase;
using Generacion.Models;
using MediatR;

namespace Generacion.Application.ReporteModulacion.Query
{
    public class RegistosReporteModulacion
    {
        private readonly IConexionBD _conexion;
        public RegistosReporteModulacion(IConexionBD conexion)
        {
            _conexion = conexion;
        }





    }
}
