using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entities;
using AutoresAPI.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers {
    [ApiController]
    [Route("api/llaves")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LlavesAPIController : CustomBaseController {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly LlavesService llavesService;

        public LlavesAPIController(
            ApplicationDbContext context,
            IMapper mapper,
            LlavesService llavesService
        ) {
            this.context = context;
            this.mapper = mapper;
            this.llavesService = llavesService;
        }

        /// <summary>
        /// Listado de llaves de un usuario.
        /// </summary>
        /// <returns></returns>
        [HttpGet("listado")]
        public async Task<List<LlaveDTO>> llavesAll() {
            var usuarioId = obtenerUsuarioId();
            var llaves = await context.LlavesAPI
                                      .Include(x => x.RestriccionesDominio)
                                      .Include(x => x.RestriccionesIP)
                                      .Where(x => x.UsuarioId == usuarioId).ToListAsync();

            return mapper.Map<List<LlaveDTO>>(llaves);
        }

        /// <summary>
        /// Crea una llave de acceso a nuestra API a un usuario. 
        /// </summary>
        /// <param name="llaveCreacionDTO"></param>
        /// <returns></returns>
        [HttpPost("crear")]
        public async Task<ActionResult> llaveCreate(LlaveCreacionDTO llaveCreacionDTO) {
            var usuarioId = obtenerUsuarioId();

            if (llaveCreacionDTO.TipoLlave == TipoLlave.Gratuita) {
                var asignadaLlave = await context.LlavesAPI.AnyAsync(x => x.UsuarioId == usuarioId && x.TipoLlave == TipoLlave.Gratuita);

                if(asignadaLlave) { return BadRequest("El usuario ya tiene una llave gratuita."); }
            }

            await llavesService.CrearLlave(usuarioId, llaveCreacionDTO.TipoLlave);

            return NoContent();
        }

        /// <summary>
        /// Actualiza una llave de acceso de un usuario a nuestra API.
        /// </summary>
        /// <param name="llaveActualizarDTO"></param>
        /// <returns></returns>
        [HttpPut("actualizar")]
        public async Task<ActionResult> llaveUpdate(LlaveActualizarDTO llaveActualizarDTO) {
            var usuarioId = obtenerUsuarioId();
            var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Id == llaveActualizarDTO.LlaveId);

            if(llaveDB is null) { return NotFound("No se encontró la llave"); }

            if (usuarioId != llaveDB.UsuarioId) { return Forbid(); }

            if (llaveActualizarDTO.ActualizarLlave) {
                llaveDB.Llave = llavesService.generarLlave();
            }

            llaveDB.Activa = llaveActualizarDTO.Activa;
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
