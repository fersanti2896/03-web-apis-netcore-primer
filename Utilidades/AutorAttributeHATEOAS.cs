using AutoresAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoresAPI.Utilidades {
    public class AutorAttributeHATEOAS : AttributeHATEOAS {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var exists = incluirHateoas(context);

            if (!exists) { 
                await next();
                return;
            }

            var result = context.Result as ObjectResult;
            var model = result.Value as AutorDTO ?? throw new ArgumentException("Se esperaba una instancia de AutorDTO");
            
            await next();
        }
    }
}
