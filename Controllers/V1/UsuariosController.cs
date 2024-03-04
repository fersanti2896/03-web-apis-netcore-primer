using AutoresAPI.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AutoresAPI.Controllers.V1 {
    [ApiController]
    [Route("api/v1/usuarios")]
    public class UsuariosController : ControllerBase {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IDataProtector dataProtector;

        public UsuariosController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider
        ) {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            dataProtector = dataProtectionProvider.CreateProtector("fersa2896");
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<Autenticacion>> registrar(Usuario usuario) {
            var user = new IdentityUser { UserName = usuario.Email, Email = usuario.Email };
            var result = await userManager.CreateAsync(user, usuario.Password);

            if (result.Succeeded) { 
                return await construirToken(usuario);
            } else {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<Autenticacion>> login(Usuario usuario) {
            var result = await signInManager.PasswordSignInAsync(usuario.Email, usuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded) {
                return await construirToken(usuario);
            } else {
                return BadRequest("Login incorrecto.");
            }
        }

        [HttpGet("renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Autenticacion>> RenovarToken() {
            var emailClaim = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var usuario = new Usuario { Email = email };

            return await construirToken(usuario);
        }

        [HttpPost("asignarRol")]
        public async Task<ActionResult> asignarRol(RolDTO rolDTO) { 
            var usuario = await userManager.FindByEmailAsync(rolDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("isAdmin", "1"));

            return NoContent();
        }

        [HttpPost("removerRol")]
        public async Task<ActionResult> removerRol(RolDTO rolDTO) {
            var usuario = await userManager.FindByEmailAsync(rolDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("isAdmin", "1"));

            return NoContent();
        }

        private async Task<Autenticacion> construirToken(Usuario usuario) {
            var claims = new List<Claim>() {
                new Claim("email", usuario.Email)
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

        #region Encriptacion - Ejemplos
        
        [HttpGet("encriptar")]
        public ActionResult encriptacion() {
            var txtPlano = "Fernando Santiago";
            var txtCifrado = dataProtector.Protect(txtPlano);
            var txtDesc = dataProtector.Unprotect(txtCifrado);

            return Ok(new { 
                txtPlano = txtPlano,
                txtCifrado = txtCifrado,
                txtDesc = txtDesc
            });
        }

        [HttpGet("encriptarTiempo")]
        public ActionResult encriptacionTiempo() {
            var porTiempo = dataProtector.ToTimeLimitedDataProtector();

            var txtPlano = "Fernando Santiago";
            var txtCifrado = porTiempo.Protect(txtPlano, lifetime: TimeSpan.FromSeconds(5));
            var txtDesc = porTiempo.Unprotect(txtCifrado);

            return Ok(new { 
                txtPlano = txtPlano,
                txtCifrado = txtCifrado,
                txtDesc = txtDesc
            });
        }
        #endregion
    }
}
