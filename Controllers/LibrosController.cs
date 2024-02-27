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
        public async Task<ActionResult<LibroDTOAutores>> Get(int id) {
            var libros = await context.Libros
                                      .Include(l => l.AutoresLibros)
                                      .ThenInclude(a => a.Autor)
                                      .Include(x => x.Comentarios)
                                      .FirstOrDefaultAsync(x => x.Id == id);

            if (libros == null) { 
                return NotFound($"No existe un libro con el id: {id}");
            }

            libros.AutoresLibros = libros.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOAutores>(libros);
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroDTO) {
            if (libroDTO.AutoresIds == null) {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresIds = await context.Autores.Where(x => libroDTO.AutoresIds.Contains(x.Id))
                                               .Select(x => x.Id).ToListAsync();

            if (libroDTO.AutoresIds.Count != autoresIds.Count) {
                return BadRequest("No existe uno de los autores enviados.");
            }

            var libro = mapper.Map<Libro>(libroDTO);

            if (libro.AutoresLibros != null) {
                for (int i = 0; i < libro.AutoresLibros.Count; i++) {
                    libro.AutoresLibros[i].Orden = i;
                }
            }

            context.Add(libro);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
