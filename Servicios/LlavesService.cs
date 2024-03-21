using AutoresAPI.Entities;

namespace AutoresAPI.Servicios {
    public class LlavesService {
        private readonly ApplicationDbContext context;

        public LlavesService(ApplicationDbContext context) {
            this.context = context;
        }

        public async Task CrearLlave(string usuarioId, TipoLlave tipoLlave) {
            var llave = generarLlave();

            var llaveAPI = new LlaveAPI { 
                Activa = true, 
                Llave = llave,
                TipoLlave = tipoLlave,
                UsuarioId = usuarioId
            };

            context.Add(llaveAPI);
            await context.SaveChangesAsync();
        }

        public string generarLlave() { 
            var llave = Guid.NewGuid().ToString().Replace("-", "");

            return llave;
        }
    }
}
