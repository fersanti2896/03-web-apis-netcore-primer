namespace AutoresAPI.Servicios {
    public interface IService {
        void RealizaTarea();
        Guid obtenerTransient();
        Guid obtenerScoped();
        Guid obtenerSingleton();
    }

    public class ServiceTransient { 
        public Guid guid = Guid.NewGuid();
    }

    public class ServiceScoped {
        public Guid guid = Guid.NewGuid();
    }

    public class ServiceSingleton { 
        public Guid guid = Guid.NewGuid();
    }
}
