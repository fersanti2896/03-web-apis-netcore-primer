using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Utilidades {
    public static class HttpContextExtensions {
        public async static Task paginacionCabecera<T>(this HttpContext httpContext, IQueryable<T> qr) { 
            if(httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }

            double cantidad = await qr.CountAsync();
            httpContext.Response.Headers.Add("totalRegistros", cantidad.ToString());
        }
    }
}
