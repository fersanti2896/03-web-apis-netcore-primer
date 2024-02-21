using AutoresAPI.Filtros;
using AutoresAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            services.AddControllers(opc => { 
                        opc.Filters.Add(typeof(FilterException));
                    })
                    .AddJsonOptions(x => 
                                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")
            ));

            services.AddTransient<FilterAction>();

            services.AddResponseCaching();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger) {
            // Middlewares

            app.UseLogueoHTTP();

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
