using AutoresAPI.Filtros;
using AutoresAPI.Middlewares;
using AutoresAPI.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace AutoresAPI{
    public class Startup {
        public Startup(IConfiguration configuration) {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            // Add services to the container.

            services.AddControllers(opc => { 
                        opc.Filters.Add(typeof(FilterException));
                        opc.Conventions.Add(new SwaggerVersion());
                    })
                    .AddJsonOptions(x => 
                                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")
            ));

            services.AddTransient<FilterAction>();

            services.AddResponseCaching();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opc => opc.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt"])),
                        ClockSkew = TimeSpan.Zero
                    }) ;

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Autores", Version = "v1", Description = "Web API de Autores y Libros con Comentarios", Contact = new OpenApiContact { Email = "fersanti2896@gmail.com" } });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "API Autores", Version = "v2", Description = "Web API de Autores y Libros con Comentarios", Contact = new OpenApiContact { Email = "fersanti2896@gmail.com" } });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

                var fileXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var routeXML = Path.Combine(AppContext.BaseDirectory, fileXML);
                c.IncludeXmlComments(routeXML);
            });

            services.AddAutoMapper(typeof(Startup));

            // Configurando Identity
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            // Autorizacion por Claims
            services.AddAuthorization(opc => {
                opc.AddPolicy("isAdmin", pol => pol.RequireClaim("isAdmin"));
            });

            services.AddDataProtection();

            // CORS
            services.AddCors(opc => { 
                opc.AddDefaultPolicy(p => {
                    p.WithOrigins("").AllowAnyMethod().AllowAnyHeader();
                });
            });
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
                app.UseSwaggerUI( c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Autores v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "API Autores v2");
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(end => {
                end.MapControllers();
            });
        } 
    }
}
