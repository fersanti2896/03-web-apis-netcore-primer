using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers {
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(
            ApplicationDbContext context,
            IMapper mapper
        ) {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDTO>> Get(int id) {
            var libros = await context.Libros.Include(x => x.Comentarios)
                                             .FirstOrDefaultAsync(x => x.Id == id);

            return mapper.Map<LibroDTO>(libros);
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroDTO) {
            //var autor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            //if (!autor) {
            //    return BadRequest($"No existe el autor con el id: { libro.AutorId }");
            //}

            var libro = mapper.Map<Libro>(libroDTO);

            context.Add(libro);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
