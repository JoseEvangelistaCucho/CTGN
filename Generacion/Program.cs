

using Generacion.Application.DataBase;

namespace Generacion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               })
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                   config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                   config.AddJsonFile("ConstantsOptions.json", false, true);
                   config.AddEnvironmentVariables();
               })
               .ConfigureServices((context, services) =>
               {
                   IConfiguration configuration = context.Configuration;
                   string cadenaConexion = configuration.GetConnectionString("DefaultConnection");
                   string cadenaConexionSQL = configuration.GetConnectionString("SQLConnection");
                   services.AddSession(options =>
                   {
                       options.IdleTimeout = TimeSpan.FromHours(8);
                   });
                   services.AddScoped<IConexionBD>(_ => new ConexionBD(cadenaConexion, cadenaConexionSQL));


               });
    }
}

