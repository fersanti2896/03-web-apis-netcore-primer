using AutoresAPI.DTOs;
using AutoresAPI.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers {
    [ApiController]
    [Route("api/ip")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesIPController : CustomBaseController {
        private readonly ApplicationDbContext context;

        public RestriccionesIPController(ApplicationDbContext context) {
            this.context = context;
        }

        /// <summary>
        /// Crea una restricción de IP.
        /// </summary>
        /// <param name="restriccionIPCreacionDTO"></param>
        /// <returns></returns>
        [HttpPost("crear")]
        public async Task<ActionResult> restriccionIPCreate(RestriccionIPCreacionDTO restriccionIPCreacionDTO) {
            var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Id == restriccionIPCreacionDTO.LlaveId);
            if (llaveDB == null) { return NotFound($"No se encontró la LlaveId"); }

            var usuarioId = obtenerUsuarioId();
            if (llaveDB.UsuarioId != usuarioId) { return Forbid(); }

            var restriccion = new RestriccionIP() { 
                LlaveId = llaveDB.Id,
                IP = restriccionIPCreacionDTO.IP
            };

            context.Add(restriccion);
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Actualiza una restricción de IP por su id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="restriccionIPActualizarDTO"></param>
        /// <returns></returns>
        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> restriccionIPUpdate(int id, RestriccionIPActualizarDTO restriccionIPActualizarDTO) {
            var restriccionDB = await context.RestriccionesIP.Include(x => x.Llave).FirstOrDefaultAsync(x => x.Id == id);
            if (restriccionDB is null) { return NotFound(); }

            var usuarioId = obtenerUsuarioId();
            if (restriccionDB.Llave.UsuarioId != usuarioId) { return Forbid(); }

            restriccionDB.IP = restriccionIPActualizarDTO.IP;
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Elimina una restricción de IP por su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> restriccionIPDelete(int id) {
            var restriccionDB = await context.RestriccionesIP.Include(x => x.Llave).FirstOrDefaultAsync(x => x.Id == id);
            if (restriccionDB is null) { return NotFound(); }

            var usuarioId = obtenerUsuarioId();
            if (restriccionDB.Llave.UsuarioId != usuarioId) { return Forbid(); }

            context.Remove(restriccionDB);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
