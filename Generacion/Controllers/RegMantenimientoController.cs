using Generacion.Application.DashBoard.TKVessel.Command;
using Generacion.Application.DataBase.cache;
using Generacion.Application.Funciones;
using Generacion.Application.Mantenimiento;
using Generacion.Application.Mantenimiento.Query;
using Generacion.Application.Usuario.Query;
using Generacion.Infraestructura;
using Generacion.Models;
using Generacion.Models.Mantenimiento;
using Generacion.Models.Session;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Generacion.Controllers
{
    public class RegMantenimientoController : ApiControllerBase
    {
        private readonly IMantenimiento _mantenimiento;
        private readonly FotoServidor _fotoServidor;
        private readonly CacheDatos _cacheDatos;
        private readonly ConsultarUsuario _consultarUsuario;

        public RegMantenimientoController(IMantenimiento mantenimiento , FotoServidor fotoServidor, CacheDatos cacheDatos, ConsultarUsuario consultarUsuario)
        {
            _mantenimiento = mantenimiento;
            _fotoServidor = fotoServidor;
            _cacheDatos = cacheDatos;
            _consultarUsuario = consultarUsuario;
        }
        public async Task<IActionResult> Index()
        {

            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            /* Dictionary<string, Dictionary<int, List<string>>> horarioOperarios = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, List<string>>>>(_cacheDatos.ObtenerContenidoCache("HorarioOperario"));
             var llavesAEliminar = horarioOperarios.Keys.Where(llave => llave != user.IdSitio).ToList();*/
            DateTime fechaActual = DateTime.Now;//string.IsNullOrEmpty(fecha) ? DateTime.Now : DateTime.Parse(fecha);


            Respuesta<List<SessionOperario>> datosSessionOperarios = await _consultarUsuario.ObtenerSessionOperarios(fechaActual.ToString("dd/MM/yyyy"));


            Dictionary<string, List<SessionOperario>> horarioOperarios = datosSessionOperarios.Detalle
                                                                        .GroupBy(x => x.Horario)
                                                                            .ToDictionary(
                                                                                    outerGroup => outerGroup.Key,
                                                                                    outerGroup => outerGroup.ToList());

            ViewData["horarioOperarios"] = horarioOperarios;


            var respuesta = await Mediator.Send(new ObtenerDatosDeRegistoMantenimiento());
            ViewData["DetalleOperario"] = user;


            ViewData["datosTKVessel"] = respuesta.Detalle["datosTKVessel"];
            ViewData["RuningHours"] = respuesta.Detalle["RuningHours"];
            
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Index([FromBody] DatosAEnviar datosAEnviar)
        {
            _mantenimiento.GuardarDatosMotoGenerador(datosAEnviar.MotoGeneradores);
            _mantenimiento.GuardarDatosServ(datosAEnviar.MantenimientoServicios);

            var respuesta = await Mediator.Send(new GuardarDetalleTKVessel()
            {
                cilindroAceiteCarters = datosAEnviar.CilindroAceiteCarter
            });

            _mantenimiento.GuardarExpansionVessel(datosAEnviar.ListaExpansionVersel);
            _mantenimiento.GuardarReporteDiario(datosAEnviar.reporteDiarioMantenimiento);

            Dictionary<int, List<string>> horarioOperarios = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(_cacheDatos.ObtenerContenidoCache("HorarioOperario"));


            var respuestaRegistro = await Mediator.Send(new ObtenerDatosDeRegistoMantenimiento());


            ViewData["datosTKVessel"] = respuestaRegistro.Detalle["datosTKVessel"];
            ViewData["RuningHours"] = respuestaRegistro.Detalle["RuningHours"];
            ViewData["horarioOperarios"] = horarioOperarios;
            return Json(new{ respuesta = respuesta });
        }
        [HttpPost]
        public async Task<IActionResult> GuardarImagen([FromBody] List<ImagenModel> imagenesBase64)
        {
                Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                int indexImagen = 0;
                foreach (ImagenModel imagenBase64 in imagenesBase64)
                {
                    indexImagen++;
                    string base64Data = Regex.Match(imagenBase64.base64, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;

                    byte[] imageBytes = Convert.FromBase64String(base64Data);

                    _fotoServidor.GuardarArchivos(imageBytes, indexImagen, imagenBase64.casillaid, $"Reporte-Mantenimiento/Fotos/{DateTime.Now.ToString("yyyy")}/{DateTime.Now.ToString("MM")}", "jpg",DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss"));
                }
                respuesta.IdRespuesta = 0;
                respuesta.Mensaje = "Imágenes recibidas correctamente";
                return Json(new { respuesta = respuesta });
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = "Error al recibir las imágenes";
                return Json(new{ respuesta = respuesta });
            }
        }
    }

    public class DatosAEnviar
    {
        public List<MotoGenerador> MotoGeneradores { get; set; }
        public List<ExpansionVersel> ListaExpansionVersel { get; set; }
        public List<MantenimientoServicios> MantenimientoServicios { get; set; }
        public List<CilindroAceiteCarter> CilindroAceiteCarter { get; set; }
        public ReporteDiarioMantenimiento reporteDiarioMantenimiento { get; set; }
        

    }
}
