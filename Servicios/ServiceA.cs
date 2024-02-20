namespace AutoresAPI.Servicios {
    public class ServiceA : IService
    {
        private readonly ILogger<ServiceA> logger;
        private readonly ServiceTransient serviceTransient;
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;

        public ServiceA(
            ILogger<ServiceA> logger,
            ServiceTransient serviceTransient,
            ServiceScoped serviceScoped,
            ServiceSingleton serviceSingleton
        ) {
            this.logger = logger;
            this.serviceTransient = serviceTransient;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
        }

        public Guid obtenerTransient() { return serviceTransient.guid; }
        public Guid obtenerScoped() { return serviceScoped.guid; }
        public Guid obtenerSingleton() { return serviceSingleton.guid; }

        public void RealizaTarea()
        {
            throw new NotImplementedException();
        }
    }
}
