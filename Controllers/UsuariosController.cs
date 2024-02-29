using AutoresAPI.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutoresAPI.Controllers {
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;

        public UsuariosController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager
        ) {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<Autenticacion>> registrar(Usuario usuario) {
            var user = new IdentityUser { UserName = usuario.Email, Email = usuario.Email };
            var result = await userManager.CreateAsync(user, usuario.Password);

            if (result.Succeeded) { 
                return construirToken(usuario);
            } else {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<Autenticacion>> login(Usuario usuario) {
            var result = await signInManager.PasswordSignInAsync(usuario.Email, usuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded) {
                return construirToken(usuario);
            } else {
                return BadRequest("Login incorrecto.");
            }
        }

        private Autenticacion construirToken(Usuario usuario) {
            var claims = new List<Claim>() {
                new Claim("email", usuario.Email)
            };

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: exp, signingCredentials: creds);

            return new Autenticacion() { Token = new JwtSecurityTokenHandler().WriteToken(securityToken), Expiracion = exp };
        }
    }
}
