using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers {
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentarioController : ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ComentarioController(
            ApplicationDbContext context,
            IMapper mapper
        ) {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> ComentarioById(int id) {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(c => c.Id == id);

            if(comentario == null) return NotFound();

            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost("crear")]
        public async Task<ActionResult> CrearComentario(int libroId, ComentarioCreacionDTO comentarioCreacionDTO) {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro) return NotFound();

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;

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
