using Generacion.Infraestructura;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

namespace Generacion
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string cadenaConexion = Configuration.GetConnectionString("DefaultConnection");

            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 20971520; // 20 MB en bytes
            });

            services.AddInfrastructure(Configuration);

            Log.Logger = new LoggerConfiguration()
           .WriteTo.File("Logs/mylogfile.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();

            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            services.AddLogging(logging =>
            {
                logging.AddSerilog(); // Agrega el proveedor de Serilog
                logging.AddEventLog();
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {

                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseCors();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}");
            });
        }
    }
}
