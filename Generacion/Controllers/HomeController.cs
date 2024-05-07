using Generacion.Application.DashBoard;
using Generacion.Application.DashBoard.CalibracionValvula.Command;
using Generacion.Application.DashBoard.CambioAceite.Command;
using Generacion.Application.DashBoard.ControlGAS.Command;
using Generacion.Application.DashBoard.Filtro.Command;
using Generacion.Application.DashBoard.LavadoTurbo.Command;
using Generacion.Application.DashBoard.TKVessel.Command;
using Generacion.Application.DashBoard.TurboCompresor.Command;
using Generacion.Application.DataBase.cache;
using Generacion.Application.DatosConsola.Query;
using Generacion.Application.Funciones;
using Generacion.Application.Usuario.Query;
using Generacion.Infraestructura;
using Generacion.Models;
using Generacion.Models.Aceite;
using Generacion.Models.CalibracionValvula;
using Generacion.Models.ControlGAS;
using Generacion.Models.DashBoard;
using Generacion.Models.DatosConsola;
using Generacion.Models.Mantenimiento;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Stimulsoft.System.Web;

namespace Generacion.Controllers
{
    public class HomeController : ApiControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatosConsola _datosConsola;
        private readonly IConfiguration _configuration;
        private readonly CacheDatos _cacheDatos;
        private readonly ConsultarUsuario _consultarUsuario;
        private readonly Function _function;
        private readonly FotoServidor _fotoServidor;
        public HomeController(
            ILogger<HomeController> logger,
            DatosConsola datosConsola,
            IConfiguration configuration,
            CacheDatos cacheDatos,
            ConsultarUsuario consultarUsuario,
            Function function,
            FotoServidor fotoServidor

            )
        {
            _fotoServidor = fotoServidor;
            _consultarUsuario = consultarUsuario;
            _configuration = configuration;
            _cacheDatos = cacheDatos;
            _logger = logger;
            _datosConsola = datosConsola;
            _function = function;
        }
        public async Task<IActionResult> Index()
        {
            DetalleOperario user = await _function.ObtenerDatosOperario();

            if (user != null)
            {
                Respuesta<Dictionary<string, CabecerasTabla>> datoscabecera = await _datosConsola.ObtenerCabecerasDeTabla();
                HttpContext.Session.SetString("datoscabecera", JsonConvert.SerializeObject(datoscabecera.Detalle));

                int horario = ObtenerTurnoHorario();

                user.IdTurno = horario;

                //GuardarDatosHorario(user, horario);

                ViewData["DetalleOperario"] = user;
                ObtenerListaOperarios();
            }

            ViewBag.MostrarModal = await _function.ValidarModalAdvertencia();

            var respuestaCentrifugo = await Mediator.Send(new ObtenerDatosDashBoard());

            ViewData["datosFiltroCentrifugo"] = respuestaCentrifugo.Detalle["datosFiltroCentrifugo"];
            ViewData["datosFiltroAutomatico"] = respuestaCentrifugo.Detalle["datosFiltroAutomatico"];
            ViewData["datoContrato"] = respuestaCentrifugo.Detalle["datoContrato"];
            ViewData["datosConsumo"] = respuestaCentrifugo.Detalle["datosConsumo"];
            ViewData["datodetalleConsumo"] = respuestaCentrifugo.Detalle["datodetalleConsumo"];
            ViewData["datosControlAceite"] = respuestaCentrifugo.Detalle["datosControlAceite"];
            ViewData["detalleControlAceite"] = respuestaCentrifugo.Detalle["detalleControlAceite"];
            ViewData["datosTurboCompresor"] = respuestaCentrifugo.Detalle["datosTurboCompresor"];
            ViewData["detalleTurboCompresor"] = respuestaCentrifugo.Detalle["detalleTurboCompresor"];
            ViewData["detalleLavadoTurbo"] = respuestaCentrifugo.Detalle["detalleLavadoTurbo"];
            ViewData["datosCalibracionValvula"] = respuestaCentrifugo.Detalle["datosCalibracionValvula"];
            ViewData["datosTKVessel"] = respuestaCentrifugo.Detalle["datosTKVessel"];
            ViewData["datosDuracionBujias"] = respuestaCentrifugo.Detalle["datosDuracionBujias"];

            return View();
        }
        public async void ObtenerListaOperarios()
        {

            Respuesta<List<DetalleOperario>> operarios = await _consultarUsuario.ObtenerOperarios();

            ViewData["ListaOperarios"] = operarios.Detalle;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public int ObtenerTurnoHorario()
        {
            var horarioOperario = _configuration.GetSection("HorarioOperario");
            var manana = horarioOperario.GetSection("mañana").Get<int[]>();
            var tarde = horarioOperario.GetSection("tarde").Get<int[]>();
            DateTime horaActual = DateTime.Now;
            int hora = horaActual.Hour;

            if (hora >= manana[0] && hora < manana[1])
            {
                return 1;
            }
            else if (hora >= tarde[0] && hora < tarde[1])
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        public void GuardarDatosHorario(DetalleOperario operario, int idHorario)
        {
            Dictionary<string, Dictionary<int, List<string>>> horarioOperario =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, List<string>>>>(
                    _cacheDatos.ObtenerContenidoCache("HorarioOperario")
                );
            string jsonConvert = string.Empty;

            if (horarioOperario == null)
            {
                horarioOperario = new Dictionary<string, Dictionary<int, List<string>>>();
            }

            bool operarioExiste = false;
            int horarioActual = 0;

            if (horarioOperario.ContainsKey(operario.IdSitio))
            {
                foreach (var kvp in horarioOperario[operario.IdSitio])
                {
                    if (kvp.Value.Contains($"{operario.IdOperario}: {operario.Apellidos} {operario.Nombre}"))
                    {
                        operarioExiste = true;
                        horarioActual = kvp.Key;
                        break;
                    }
                }
            }

            if (operarioExiste)
            {
                horarioOperario[operario.IdSitio][horarioActual].Remove($"{operario.IdOperario}: {operario.Apellidos} {operario.Nombre}");

                if (!horarioOperario[operario.IdSitio].ContainsKey(idHorario))
                {
                    horarioOperario[operario.IdSitio].Add(idHorario, new List<string>());
                }
                horarioOperario[operario.IdSitio][idHorario].Add($"{operario.IdOperario}: {operario.Apellidos} {operario.Nombre}");
            }
            else
            {
                if (!horarioOperario.ContainsKey(operario.IdSitio))
                {
                    horarioOperario.Add(operario.IdSitio, new Dictionary<int, List<string>>());
                }

                if (!horarioOperario[operario.IdSitio].ContainsKey(idHorario))
                {
                    horarioOperario[operario.IdSitio].Add(idHorario, new List<string>());
                }
                horarioOperario[operario.IdSitio][idHorario].Add($"{operario.IdOperario}: {operario.Apellidos} {operario.Nombre}");
            }
            jsonConvert = JsonConvert.SerializeObject(horarioOperario);

            _cacheDatos.GuardarDatosCache("HorarioOperario", jsonConvert);
        }
        [HttpPost]
        public async Task<JsonResult> GuardarDatosDashBoard([FromBody] List<DashboardDetalleFiltro> detalleFiltro)
        {
            var respuesta = await Mediator.Send(new GuardarDatosDashboard()
            {
                detalleFiltro = detalleFiltro
            });

            return Json(new { respuesta = respuesta });
        }
        [HttpPost]
        public async Task<JsonResult> GuardarContratoGAS([FromBody] ContratoGas datos)
        {
            var respuesta = await Mediator.Send(new GuardarContratoGas()
            {
                contratoGas = datos
            });

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosGAS([FromBody] ConsumoGas datos)
        {
            var respuesta = await Mediator.Send(new GuardarControlGas()
            {
                consumoGas = datos
            });

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDetalleGAS([FromBody] DetalleConsumoGas datos)
        {
            var respuesta = await Mediator.Send(new GuardarDetalleGas()
            {
                detalleConsumoGas = datos
            });

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosControlAceite([FromBody] List<ControlCambioAceite> datos)
        {
            var respuesta = await Mediator.Send(new ListaControlCambioAceite()
            {
                datosControl = datos
            });

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDetallesControlAceite([FromBody] List<DetalleControlAceite> datos)
        {
            var respuesta = await Mediator.Send(new RegistrarDetalleAceite()
            {
                detalleControlAceites = datos
            });

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosTurboCompresor([FromBody] RegistrarDatosTurboCompresor datos)
        {
            var respuesta = await Mediator.Send(datos);

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosLavadoTurbo([FromBody] GuardarDatosLavadoTurbo datos)
        {
            var respuesta = await Mediator.Send(datos);

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosCalibracion([FromBody] List<DetalleCalibracionValvula> datos)
        {
            var respuesta = await Mediator.Send(new GuardarDetalleCalibracionValvula()
            {
                detalles = datos
            });

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosTKVessel([FromBody] List<CilindroAceiteCarter> datos)
        {
            var respuesta = await Mediator.Send(new GuardarDetalleTKVessel()
            {
                cilindroAceiteCarters = datos
            });

            return Json(new { respuesta = respuesta });
        }
    }
}
