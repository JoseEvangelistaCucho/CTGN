using Generacion.Application.DataBase;
using Generacion.Application.DataBase.cache;
using Generacion.Application.DatosConsola;
using Generacion.Application.DatosConsola.Command;
using Generacion.Application.DatosConsola.Query;
using Generacion.Application.LecturaCampo.Command;
using Generacion.Application.LecturaCampo;
using Generacion.Application.LecturaCampo.Query;
using Generacion.Application.Funciones;
using Generacion.Application.ION.Query;
using Generacion.Application.Mantenimiento;
using Generacion.Application.Mantenimiento.Command;
using Generacion.Application.MGD;
using Generacion.Application.MGD.Command;
using Generacion.Application.MGD.Query;
using Generacion.Application.ReporteDiarioGAS;
using Generacion.Application.ReporteDiarioGAS.Query;
using Generacion.Application.ReporteGAS.Command;
using Generacion.Application.Usuario;
using Generacion.Application.Usuario.Command;
using Generacion.Application.Usuario.Query;
using WebApi.Application.Common.Interfaces;
using WebApi.Application.ValidatePassword.Queries;
using System.DirectoryServices.AccountManagement;
using Generacion.Application.Usuario.Session;
using Generacion.Application.ValidationSession.Login;
using Generacion.Application.ION;
using Generacion.Application.ION.Command;
using Generacion.Application.Mantenimiento.Query;
using Generacion.Application.ReporteProduccion;
using Generacion.Application.ReporteProduccion.Command;
using Generacion.Application.ReporteProduccion.Query;
using Generacion.Application.Almacen.Query;
using Generacion.Application.Almacen;
using Generacion.Application.Almacen.Command;
using Generacion.Application.Bujias;
using Generacion.Application.Bujias.Command;
using Generacion.Application.Bujias.Query;
using Generacion.Application.Usuario.Session.SessionStatus;
using Generacion.Application.FiltroCentrifugo.Query;
using Generacion.Application.FiltroCentrifugo;
using Generacion.Application.FiltroCentrifugo.Command;
using MediatR;
using Generacion.Application.Common;
using Generacion.Application.DashBoard.Filtro.Query;
using Generacion.Application.DashBoard.Filtro.Command;
using Generacion.Application.DashBoard.Filtro;
using Generacion.Application.DashBoard.ControlGAS;
using Generacion.Application.DashBoard.ControlGAS.Command;
using Generacion.Application.DashBoard.ControlGAS.Query;
using Generacion.Application.DashBoard.CambioAceite;
using Generacion.Application.DashBoard.CambioAceite.Command;
using Generacion.Application.DashBoard.CambioAceite.Query;
using Generacion.Application.DashBoard.TurboCompresor;
using Generacion.Application.DashBoard.TurboCompresor.Command;
using Generacion.Application.DashBoard.TurboCompresor.Query;
using Generacion.Application.DashBoard.LavadoTurbo;
using Generacion.Application.DashBoard.LavadoTurbo.Command;
using Generacion.Application.DashBoard.LavadoTurbo.Query;
using Generacion.Application.DashBoard.CalibracionValvula;
using Generacion.Application.DashBoard.CalibracionValvula.Command;
using Generacion.Application.DashBoard.CalibracionValvula.Query;
using Generacion.Application.Usuario.Session.Command;
using OfficeOpenXml;
using Generacion.Application.InformePerturbacion;
using Generacion.Application.InformePerturbacion.Command;
using Stimulsoft.Base;
using Generacion.Application.PruebasSemanales.BackStart.Command;
using Generacion.Application.PruebasSemanales.BackStart;
using Generacion.Application.PruebasSemanales.BackStart.Query;
using Generacion.Application.ReportesGenerales;
using Generacion.Application.ReportesGenerales.DataCoes.Command;
using Generacion.Application.BessLlipata.Query;
using Generacion.Application.BessLlipata.ReporteProduccion;
using Generacion.Application.BessLlipata.ReporteProduccion.Command;
using Generacion.Application.ReporteModulacion.Command;
using Generacion.Application.Eventos.Query;
using Generacion.Application.RAM.Query;
using Generacion.Application.RAM;
using Generacion.Application.RAM.Command;
using Generacion.Application.InformePerturbacion.Query;
using Generacion.Application.ReportesGenerales.DataBess.Command;
using Generacion.Application.Fotovoltaica;
using Generacion.Application.Fotovoltaica.Command;
using Generacion.Application.Fotovoltaica.Query;

namespace Generacion.Infraestructura
{
    public static class DependencyInyection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection idDatosCampo = configuration.GetSection("RutaReportes");
            string pathLicence = idDatosCampo["pathLicence"];

            StiLicense.LoadFromFile(Path.Combine(pathLicence, "license.key"));

            services.AddMediatR(typeof(Startup));

            services.AddScoped<ValidarSesion>();
            services.AddScoped<IMantenimiento, Mantenimiento>();
            services.AddScoped<IUsuario, DatosUsuario>();

            services.AddHttpContextAccessor();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            services.AddScoped<IRegistroRAM, RegistrarDatosRAM>();
            services.AddScoped<IValidateUser, ValidateUser>();
            services.AddScoped<IDatosRegistroConsola, DatosRegistroConsola>();
            services.AddScoped<IRegistroDatosGAS, RegistroDatosGAS>();
            services.AddScoped<IDatosMGD, RegistroDatosMGD>();
            services.AddScoped<ILecturaCampo, DatosRegistroCampo>();
            services.AddScoped<IDatosION, RegistroDatosION>();
            services.AddScoped<IReporteProduccion, RegistrarProduccion>();
            services.AddScoped<IAlmacen, RegistrosAlmacen>();
            services.AddScoped<IBujias, RegistoBujias>();
            services.AddScoped<IDashBoard, RegistroDashBoard>();
            services.AddScoped<IRegistroFiltroCentrifugo, RegistroFiltroCentrifugo>();
            services.AddScoped<IRegistroControlGas, RegistroControlGas>();
            services.AddScoped<IRegistroControlAceite, RegistroControlAceite>();
            services.AddScoped<IRegistroTurboCompresor, RegistroTurboCompresor>();
            services.AddScoped<IRegistroLavadoTurbo, RegistroLavadoTurbo>();
            services.AddScoped<IRegistroCalibracionValvula, RegistroCalibracionValvula>();
            services.AddScoped<IRegistroDatosPerturbacion, RegistroDatosPerturbacion>();
            services.AddScoped<IRegistroDatosPruebaSemanal, RegistroDatosPruebaSemanal>();
            services.AddScoped<IRegistroDataCoes, RegistroDataCoes>();
            services.AddScoped<IReporteBessLlipata, RegistroDatosBessLlipata>();
            services.AddScoped<IRegistroDataBess, RegistroDataBess>();
            services.AddScoped<IGuardarDatosFotovoltaica, GuardarDatosFotovoltaica>();

            
            services.AddScoped(typeof(DatosFotovoltaica));
            services.AddScoped(typeof(DatosPerturbacion));
            services.AddScoped(typeof(ObtenerRegistrosRAM));
            services.AddScoped(typeof(DatosEventos));
            services.AddScoped(typeof(GuardarDatosModulacion));
            services.AddScoped(typeof(ObtenerDatosBessLlipata));
            services.AddScoped(typeof(ObtenerRegistrosPruebaSemanal));
            services.AddScoped(typeof(FotoServidor));
            services.AddScoped(typeof(ConsultarUsuario));
            services.AddScoped(typeof(DatosConsola));
            services.AddScoped(typeof(ObtenerDatosReporteGAS));
            services.AddScoped(typeof(ConsultarION));
            services.AddScoped(typeof(ConsultarDatosMGD));
            services.AddScoped(typeof(CacheDatos));
            services.AddScoped(typeof(LecturaCampo));
            services.AddScoped(typeof(DatosMantenimiento));
            services.AddScoped(typeof(ConsultarProduccion));
            services.AddScoped(typeof(CacheDatos));
            services.AddScoped(typeof(ConsultasAlmacen));
            services.AddScoped(typeof(ConsultaBujias));
            services.AddScoped(typeof(DatosFiltro));
            services.AddScoped(typeof(Function));
            services.AddScoped(typeof(DatosFiltroCentrifugo));
            services.AddScoped(typeof(ProcessExecutionContextExtensions));
            services.AddScoped(typeof(ObtenerDetalleConsumoGas));
            services.AddScoped(typeof(DatosControlRegistroAceite));
            services.AddScoped(typeof(ConsultaDatosTurboCompresor));
            services.AddScoped(typeof(DatosLavadoTurbo));
            services.AddScoped(typeof(DatosCalibracionValvula));
            services.AddScoped(typeof(GuardarDatosDeSession));


            services.AddSingleton<IActiveDirectoryProvider, ActiveDirectoryProvider>();
            var principalContext = new PrincipalContext(ContextType.Domain);

            services.AddSingleton(principalContext);

            return services;
        }
    }
}
