using AutoresAPI.DTO;
using AutoresAPI.DTOs;
using AutoresAPI.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers {
    [ApiController]
    [Route("api/dominio")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesDominioController : CustomBaseController {
        private readonly ApplicationDbContext context;

        public RestriccionesDominioController(ApplicationDbContext context) {
            this.context = context;
        }

        /// <summary>
        /// Crea una restriccion de dominio. 
        /// </summary>
        /// <param name="restriccionesCreacionDTO"></param>
        /// <returns></returns>
        [HttpPost("crear")]
        public async Task<ActionResult> restriccionCreate(RestriccionesCreacionDTO restriccionesCreacionDTO) {
            var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Id == restriccionesCreacionDTO.LlaveId);
            if (llaveDB == null) { return NotFound(); }

            var usuarioId = obtenerUsuarioId();
            if(usuarioId != llaveDB.UsuarioId) { return Forbid(); }

            var restriccion = new RestriccionDominio() { 
                LlaveId = restriccionesCreacionDTO.LlaveId,
                Dominio = restriccionesCreacionDTO.Dominio
            };

            context.Add(restriccion);
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Actualiza una restricción de dominio. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="restriccionesActualizarDTO"></param>
        /// <returns></returns>
        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> restriccionUpdate(int id, RestriccionesActualizarDTO restriccionesActualizarDTO) {
            var restriccionDB = await context.RestriccionesDominio.Include(x => x.Llave).FirstOrDefaultAsync(x => x.Id == id);
            if(restriccionDB is null) { return NotFound(); }

            var usuarioId = obtenerUsuarioId();
            if (usuarioId != restriccionDB.Llave.UsuarioId) { return Forbid(); }

            restriccionDB.Dominio = restriccionesActualizarDTO.Dominio;
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Elimina una restricción de dominio por su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> restriccionDelete(int id) {
            var restriccionDB = await context.RestriccionesDominio.Include(x => x.Llave).FirstOrDefaultAsync(x => x.Id == id);
            if (restriccionDB is null) { return NotFound(); }

            var usuarioId = obtenerUsuarioId();
            if (usuarioId != restriccionDB.Llave.UsuarioId) { return Forbid(); }

            context.Remove(restriccionDB);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
