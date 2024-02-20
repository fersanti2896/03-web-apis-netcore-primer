using AutoresAPI.Entities;
using AutoresAPI.Filtros;
using AutoresAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers {
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IService service;
        private readonly ServiceTransient serviceTransient;
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;
        private readonly ILogger<AutoresController> logger;

        public AutoresController(
            ApplicationDbContext context,
            IService service,
            ServiceTransient serviceTransient,
            ServiceScoped serviceScoped,
            ServiceSingleton serviceSingleton,
            ILogger<AutoresController> logger
        ) {
            this.context = context;
            this.service = service;
            this.serviceTransient = serviceTransient;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
            this.logger = logger;
        }

        [HttpGet("guid")]
        //[ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(FilterAction))]
        public ActionResult obtenerGuids() { 
            return Ok(new { 
                AutoresController_Transient = serviceTransient.guid,
                AutoresController_Scoped = serviceScoped.guid,
                AutoresController_Singleton = serviceSingleton.guid,
                ServicioA_Transient = service.obtenerTransient(),
                ServiceA_Scoped = service.obtenerScoped(),
                ServiceA_Singleton = service.obtenerSingleton()
            }); 
        }

        [HttpGet("listado")]
        [ServiceFilter(typeof(FilterAction))]
        //[Authorize]
        public async Task<ActionResult<List<Autor>>> GetAutores() {
            logger.LogInformation("Estamos obteniendo información de Autores");
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpGet("listadoSincrona")]
        public List<Autor> GetAutoresSincrona() {
            return context.Autores.Include(x => x.Libros).ToList();
        }

        [HttpGet("primerAutor")]
        public async Task<ActionResult<Autor>> primerAutor() {
            return await context.Autores.FirstOrDefaultAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Autor>> Get(int id) { 
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            
            if(autor is null) {
                return NotFound();
            }

            return autor;
        }

        [HttpPost("createAutor")]
        public async Task<ActionResult> Post([FromBody] Autor autor) {
            var existsName = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);
            
            if(existsName) {
                return BadRequest($"Ya existe un autor con el mismo nombre {autor.Nombre}");
            }

            context.Add(autor);
            await context.SaveChangesAsync();  

            return Ok();
        }

        [HttpPut("{id:int}")]
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

        [HttpDelete("{id:int}")]
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
