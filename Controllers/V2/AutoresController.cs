﻿using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers.V2 {
    [ApiController]
    [Route("api/v2/autores")]
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

        [HttpGet("listado", Name = "ObtenerAutoresv2")]
        public async Task<ActionResult<List<AutorDTOLibros>>> GetAutores() {
            var autores = await context.Autores
                                       .Include(a => a.AutoresLibros)
                                       .ThenInclude(al => al.Libro)
                                       .ToListAsync();

            autores.ForEach(x => x.Nombre = x.Nombre.ToUpper());
            autores.ForEach(x => x.Apellido = x.Apellido.ToUpper());

            return mapper.Map<List<AutorDTOLibros>>(autores);
        }

        [HttpGet("{id:int}", Name = "ObtieneAutorv2")]
        [AllowAnonymous]
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

        [HttpPost("crear", Name = "CrearAutorv2")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO) {
            var existsName = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
            
            if(existsName) {
                return BadRequest($"Ya existe un autor con el mismo nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTOLibros>(autor);

            return CreatedAtRoute("ObtieneAutorv2", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("actualizar/{id:int}", Name = "ActualizarAutorv2")]
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