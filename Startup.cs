using AutoresAPI.Servicios;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace AutoresAPI{
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            // Add services to the container.

            services.AddControllers()
                    .AddJsonOptions(x => 
                                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")
            ));

            services.AddTransient<IService, ServiceA>();

            services.AddTransient<ServiceTransient>();
            services.AddScoped<ServiceScoped>();
            services.AddSingleton<ServiceSingleton>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger) {
            // Middlewares

            app.Use(async (context, next) => {
                using (var ms = new MemoryStream()) {
                    var body = context.Response.Body;
                    context.Response.Body = ms;

                    await next.Invoke();

                    ms.Seek(0, SeekOrigin.Begin);
                    string resp = new StreamReader(ms).ReadToEnd();
                    ms.Seek(0, SeekOrigin.Begin);

                    await ms.CopyToAsync(body);
                    context.Response.Body = body;

                    logger.LogInformation(resp);
                };
            });
            
            app.Map("/ruta1", app => {
                app.Run(async context => {
                    await context.Response.WriteAsync("Interceptando la tuberia");
                });
            });            

            if (env.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(end => {
                end.MapControllers();
            });
        } 
    }
}
