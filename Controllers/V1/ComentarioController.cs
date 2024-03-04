using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers.V1 {
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentarios")]
    public class ComentarioController : ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentarioController(
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager
        ) {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet("{id:int}", Name = "ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> ComentarioById(int id) {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);

            if(comentario == null) return NotFound();

            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost("crear")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CrearComentario(int libroId, ComentarioCreacionDTO comentarioCreacionDTO) {
            var emailClaim = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro) return NotFound();

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;

            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);
        }

        [HttpPut("actualizar/{comId:int}")]
        public async Task<ActionResult> ActComentario(int libroId, int comId, ComentarioCreacionDTO comentarioDTO) {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro) return NotFound();

            var existeCom = await context.Comentarios.AnyAsync(c => c.Id == comId);

            if(!existeCom) return NotFound();

            var comentario = mapper.Map<Comentario>(comentarioDTO);
            comentario.Id = comId;
            comentario.LibroId = libroId;

            context.Update(comentario);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}