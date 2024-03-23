using Microsoft.AspNetCore.Identity;

namespace AutoresAPI.Entities {
    public class Usuario : IdentityUser {
        public bool EstadoCuenta { get; set; }
    }
}
