using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entities;
using AutoresAPI.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers {
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(
            ApplicationDbContext context,
            IMapper mapper
        ) {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("listado")]
        public async Task<ActionResult<List<AutorDTO>>> GetAutores() {
            var autores = await context.Autores.ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AutorDTOLibros>> Get(int id) { 
            var autor = await context.Autores
                                     .Include(a => a.AutoresLibros)
                                     .ThenInclude(al => al.Libro)
                                     .FirstOrDefaultAsync(x => x.Id == id);
            
            if(autor is null) {
                return NotFound();
            }

            return mapper.Map<AutorDTOLibros>(autor);
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorDTO) {
            var existsName = await context.Autores.AnyAsync(x => x.Nombre == autorDTO.Nombre);
            
            if(existsName) {
                return BadRequest($"Ya existe un autor con el mismo nombre {autorDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorDTO);

            context.Add(autor);
            await context.SaveChangesAsync();  

            return Ok();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(Autor autor, int id) {
            if (autor.Id != id) { 
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe) {
                return NotFound();
            }

            context.Update(autor);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> Delete(int id) {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe) {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
