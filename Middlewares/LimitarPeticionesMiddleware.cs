using AutoresAPI.DTOs;
using AutoresAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Middlewares {
    public class LimitarPeticionesMiddleware {
        private readonly RequestDelegate requestDelegate;
        private readonly IConfiguration configuration;

        public LimitarPeticionesMiddleware(RequestDelegate requestDelegate, IConfiguration configuration) {
            this.requestDelegate = requestDelegate;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context) {
            var limitePeticiones = new LimitesPeticionesDTO();
            configuration.GetRequiredSection("limitePeticiones").Bind(limitePeticiones);

            var ruta = httpContext.Request.Path.ToString();
            var rutaBlanca = limitePeticiones.ListaBlancaRutas.Any(x => ruta.Contains(x));

            if (rutaBlanca) { 
                await requestDelegate(httpContext);
                return;
            }

            var llaveStringValues = httpContext.Request.Headers["X-Api-Key"];

            if(llaveStringValues.Count == 0) { 
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Debe proveer la llave en la cabecera X-Api-Key");

                return;
            }

            if(llaveStringValues.Count > 1) { 
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Solo una llave debe estar presente.");

                return;
            }

            var llave = llaveStringValues[0];
            var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Llave == llave);

            if(llaveDB is null) {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("La llave es inexistente.");

                return;
            }

            if(llaveDB is null) {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("La llave se encuentra inactiva.");

                return;
            }

            if (llaveDB.TipoLlave == TipoLlave.Gratuita) {
                var hoy = DateTime.Today;
                var siguienteDia = hoy.AddDays(1);
                var conteoPeticiones = await context.PeticionesAPI
                                                    .CountAsync(x => x.LlaveId == llaveDB.Id && x.FechaPeticion >= hoy && x.FechaPeticion < siguienteDia);

                if (conteoPeticiones >= limitePeticiones.PeticionesGratuitas) {
                    httpContext.Response.StatusCode = 429;
                    await httpContext.Response
                                     .WriteAsync("Ha excedido el límite de peticiones por día. Si desea realizar mas peticiones, actualice su cuenta a profesional.");

                    return;
                }
            }

            var peticion = new PeticionAPI() { LlaveId = llaveDB.Id, FechaPeticion = DateTime.UtcNow };
            context.Add(peticion);
            await context.SaveChangesAsync();

            await requestDelegate(httpContext);
        }
    }
}
