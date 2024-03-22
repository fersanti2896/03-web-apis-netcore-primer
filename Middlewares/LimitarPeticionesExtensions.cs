namespace AutoresAPI.Middlewares {
    public static class LimitarPeticionesExtensions {
        public static IApplicationBuilder UseLimitarPeticiones(this IApplicationBuilder app) {
            return app.UseMiddleware<LimitarPeticionesMiddleware>();
        }
    }
}
