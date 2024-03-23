using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Servicios {
    public class FacturaHostedService : IHostedService {
        private readonly IServiceProvider serviceProvider;
        private Timer timer;

        public FacturaHostedService(IServiceProvider serviceProvider) {
            this.serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            timer = new Timer(ProcesarFacturas, null, TimeSpan.Zero, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            timer.Dispose();

            return Task.CompletedTask;
        }

        private void ProcesarFacturas(object state) {
            using (var scoped = serviceProvider.CreateScope()) {
                var context = scoped.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                ActualizaEstadoCuenta(context);
                EmitirFacturas(context);
            }
        }

        private static void ActualizaEstadoCuenta(ApplicationDbContext context) { 
            context.Database.ExecuteSqlInterpolated($"EXEC ActualizaEstadoCuentaUsuario");
        }

        private static void EmitirFacturas(ApplicationDbContext context) {
            var today = DateTime.Today;
            var dateCompare = today.AddMonths(-1);
            var facturaEmitida = context.FacturasEmitidas.Any(x => x.Anio == dateCompare.Year && x.Mes == dateCompare.Month);

            if (!facturaEmitida) {
                var fechaInicio = new DateTime(dateCompare.Year, dateCompare.Month, 1);
                var fechaFin = fechaInicio.AddMonths(1);

                context.Database.ExecuteSqlInterpolated($"EXEC CreacionFacturas {fechaInicio.ToString("yyyy-MM-dd")}, {fechaFin.ToString("yyyy-MM-dd")}");
            }
        }
    }
}
