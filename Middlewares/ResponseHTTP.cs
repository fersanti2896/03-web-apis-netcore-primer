namespace AutoresAPI.Middlewares {

    public static class ResponseHTTPExtensions {
        public static IApplicationBuilder UseLogueoHTTP(this IApplicationBuilder app) {
            return app.UseMiddleware<ResponseHTTP>();
        }
    }

    public class ResponseHTTP {
        private readonly RequestDelegate next;
        private readonly ILogger<ResponseHTTP> logger;

        public ResponseHTTP(RequestDelegate next, ILogger<ResponseHTTP> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) {
            using (var ms = new MemoryStream())
            {
                var body = context.Response.Body;
                context.Response.Body = ms;

                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string resp = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(body);
                context.Response.Body = body;

                logger.LogInformation(resp);
            };
        }
    }
}
