using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers.V1 {
    [ApiController]
    [Route("api/v1/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
    public class AutoresController : ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController(
            ApplicationDbContext context,
            IMapper mapper,
            IConfiguration configuration
        ) {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        /// <summary>
        /// Listado de Autores
        /// </summary>
        /// <returns></returns>
        [HttpGet("listado", Name = "ObtenerAutores")]
        public async Task<ActionResult<List<AutorDTOLibros>>> GetAutores() {
            var autores = await context.Autores
                                       .Include(a => a.AutoresLibros)
                                       .ThenInclude(al => al.Libro)
                                       .ToListAsync();

            return mapper.Map<List<AutorDTOLibros>>(autores);
        }

        /// <summary>
        /// Obtiene un Autor por su Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "ObtieneAutor")]
        [AllowAnonymous]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200)]
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

        [HttpPost("crear", Name = "CrearAutor")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO) {
            var existsName = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
            
            if(existsName) {
                return BadRequest($"Ya existe un autor con el mismo nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTOLibros>(autor);

            return CreatedAtRoute("ObtieneAutor", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("actualizar/{id:int}", Name = "ActualizarAutor")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorDTO, int id) {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe) {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorDTO);
            autor.Id = id;  

            context.Update(autor);
            await context.SaveChangesAsync();            

            return NoContent();
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
