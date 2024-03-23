using AutoresAPI.DTOs;
using AutoresAPI.Entities;
using AutoresAPI.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        private readonly UserManager<Usuario> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<Usuario> signInManager;
        private readonly LlavesService serviceLlaves;

        public UsuariosController(
            UserManager<Usuario> userManager,
            IConfiguration configuration,
            SignInManager<Usuario> signInManager,
            LlavesService serviceLlaves
        ) {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.serviceLlaves = serviceLlaves;
        }

        /// <summary>
        /// Registra un usuario en nuestra API
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [HttpPost("registrar")]
        public async Task<ActionResult<Autenticacion>> registrar(UsuarioDTO usuario) {
            var user = new Usuario { UserName = usuario.Email, Email = usuario.Email };
            var result = await userManager.CreateAsync(user, usuario.Password);

            if (result.Succeeded) {
                await serviceLlaves.CrearLlave(user.Id, TipoLlave.Gratuita);

                return await construirToken(usuario, user.Id);
            } else {
                return BadRequest(result.Errors);
            }
        }

        /// <summary>
        /// Acceso a un usuario a nuestra API
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<Autenticacion>> login(UsuarioDTO usuario) {
            var result = await signInManager.PasswordSignInAsync(usuario.Email, usuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded) {
                var user = await userManager.FindByEmailAsync(usuario.Email);

                return await construirToken(usuario, user.Id);
            } else {
                return BadRequest("Login incorrecto.");
            }
        }

        /// <summary>
        /// Renueva token del usuario para nuestra API. 
        /// </summary>
        /// <returns></returns>
        [HttpGet("renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Autenticacion>> RenovarToken() {
            var emailClaim = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var idClaim = HttpContext.User.Claims.Where(c => c.Type == "id").FirstOrDefault();
            var usuarioId = idClaim.Value;

            var usuario = new UsuarioDTO { Email = email };

            return await construirToken(usuario, usuarioId);
        }

        /// <summary>
        /// Asigna rol a un usuario para acceso a ciertos endpoints en nuestra API.
        /// </summary>
        /// <param name="rolDTO"></param>
        /// <returns></returns>
        [HttpPost("asignarRol")]
        public async Task<ActionResult> asignarRol(RolDTO rolDTO) { 
            var usuario = await userManager.FindByEmailAsync(rolDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("isAdmin", "1"));

            return NoContent();
        }

        /// <summary>
        /// Remueve un rol de un usuario para acceso a nuestra API. 
        /// </summary>
        /// <param name="rolDTO"></param>
        /// <returns></returns>
        [HttpPost("removerRol")]
        public async Task<ActionResult> removerRol(RolDTO rolDTO) {
            var usuario = await userManager.FindByEmailAsync(rolDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("isAdmin", "1"));

            return NoContent();
        }

        private async Task<Autenticacion> construirToken(UsuarioDTO usuario, string usuarioId) {
            var claims = new List<Claim>() {
                new Claim("email", usuario.Email),
                new Claim("id", usuarioId)
            };

            var user = await userManager.FindByEmailAsync(usuario.Email);
            var claimDB = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddMinutes(30);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: exp, signingCredentials: creds);

            return new Autenticacion() { Token = new JwtSecurityTokenHandler().WriteToken(securityToken), Expiracion = exp };
        }
    }
}
