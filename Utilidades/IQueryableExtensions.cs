using AutoresAPI.DTOs;

namespace AutoresAPI.Utilidades {
    public static class IQueryableExtensions {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> query, PaginacionDTO paginacionDTO) {
            return query.Skip((paginacionDTO.Pagina - 1) * paginacionDTO.Elementos)
                        .Take(paginacionDTO.Elementos);
        }
    }
}
