﻿using AutoresAPI.Entities;
using AutoresAPI.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers {
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase {
        private readonly ApplicationDbContext context;

        public AutoresController(
            ApplicationDbContext context
        ) {
            this.context = context;
        }

        [HttpGet("listado")]
        public async Task<ActionResult<List<Autor>>> GetAutores() {
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Autor>> Get(int id) { 
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            
            if(autor is null) {
                return NotFound();
            }

            return autor;
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post([FromBody] Autor autor) {
            var existsName = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);
            
            if(existsName) {
                return BadRequest($"Ya existe un autor con el mismo nombre {autor.Nombre}");
            }

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
