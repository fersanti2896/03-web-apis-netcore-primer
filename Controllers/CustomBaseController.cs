using AutoMapper;
using AutoresAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI.Controllers {
    public class CustomBaseController : ControllerBase {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(
            ApplicationDbContext context,
            IMapper mapper
        ) {
            this.context = context;
            this.mapper = mapper;
        }

        protected string obtenerUsuarioId() {
            var usuarioClaim = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
            var usuarioId = usuarioClaim.Value;

            return usuarioId;
        }

        /// <summary>
        /// Método que devuelve un listado de una entidad.
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <returns></returns>
        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class {
            var ent = await context.Set<TEntidad>()
                                   .AsNoTracking() 
                                   .ToListAsync();

            var dto = mapper.Map<List<TDTO>>(ent);

            return dto;
        }


        /// <summary>
        /// Método que devuelve un elemento por su id de una entidad.
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<ActionResult<TDTO>> GetById<TEntidad, TDTO>(int id) where TEntidad : class, IId {
            var ent = await context.Set<TEntidad>()
                                   .AsNoTracking() 
                                   .FirstOrDefaultAsync(x => x.Id == id);

            if (ent == null) { return NotFound($"No existe el elemento con el id {id}"); }

            var dto = mapper.Map<TDTO>(ent);

            return dto;
        }

        /// <summary>
        /// Método para creación de un elemento de una entidad.
        /// </summary>
        /// <typeparam name="TCreacion"></typeparam>
        /// <typeparam name="TEntidad"></typeparam>
        /// <typeparam name="TLectura"></typeparam>
        /// <param name="creacion"></param>
        /// <param name="ruta"></param>
        /// <returns></returns>
        protected async Task<ActionResult> Post<TCreacion, TEntidad, TLectura>(TCreacion creacion, string ruta) where TEntidad : class, IId {
            var entidad = mapper.Map<TEntidad>(creacion);
            context.Add(entidad);

            await context.SaveChangesAsync();

            var dtoLectura = mapper.Map<TLectura>(entidad);

            return new CreatedAtRouteResult(ruta, new { Id = entidad.Id }, dtoLectura);
        }

        /// <summary>
        /// Método que actualiza un elemento de una entidad.
        /// </summary>
        /// <typeparam name="TCreacion"></typeparam>
        /// <typeparam name="TEntidad"></typeparam>
        /// <param name="id"></param>
        /// <param name="creacion"></param>
        /// <returns></returns>
        protected async Task<ActionResult> Put<TCreacion, TEntidad>(int id, TCreacion creacion) where TEntidad : class, IId {
            var elemExiste = await context.Set<TEntidad>()
                                          .AnyAsync(g => g.Id == id);

            if (!elemExiste) { return NotFound("El elemento no existe."); }

            var entidad = mapper.Map<TEntidad>(creacion);
            entidad.Id = id;

            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Método que elimina un elemento de una entidad.
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad : class, IId, new() {
            var elemExite = await context.Set<TEntidad>()
                                            .AnyAsync(g => g.Id == id);

            if (!elemExite) { return NotFound("El elemento no existe."); }

            context.Remove(new TEntidad() { Id = id });

            await context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Método para paginar un listado de una entidad.
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <param name="paginacion"></param>
        /// <returns></returns>
        //protected async Task<List<TDTO>> GetPagination<TEntidad, TDTO>(PaginacionDTO paginacion) where TEntidad : class {
        //    var query = context.Set<TEntidad>()
        //                       .AsQueryable();

        //    await HttpContext.paginacionCabecera(query);

        //    var entidad = await query.Paginar(paginacion)
        //                             .ToListAsync();

        //    return mapper.Map<List<TDTO>>(entidad);
        //}

        /// <summary>
        /// Método para paginar un listado de una entidad.
        /// </summary>
        /// <typeparam name="TEntidad"></typeparam>
        /// <typeparam name="TDTO"></typeparam>
        /// <param name="paginacion"></param>
        /// <returns></returns>
        //protected async Task<List<TDTO>> GetPagination<TEntidad, TDTO>(PaginacionDTO paginacion, IQueryable<TEntidad> query) where TEntidad : class {
        //    await HttpContext.paginacionCabecera(query);

        //    var entidad = await query.Paginar(paginacion)
        //                             .ToListAsync();

        //    return mapper.Map<List<TDTO>>(entidad);
        //}
    }
}
