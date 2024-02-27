using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entities;

namespace AutoresAPI.Utilidades {
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<AutorDTO, AutorDTO>();
            CreateMap<Autor, AutorDTOLibros>().ForMember(a => a.Libros, opc => opc.MapFrom(MapAutorDTOLibros));
            CreateMap<LibroCreacionDTO, Libro>().ForMember(l => l.AutoresLibros, opc => opc.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOAutores>().ForMember(l => l.Autores, opc => opc.MapFrom(MapLibroDTOAutores));
            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
         
        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO ) { 
            var result = new List<LibroDTO>();
            
            if(autor.AutoresLibros == null) { return result; }

            foreach (var libro in autor.AutoresLibros) { 
                result.Add(new LibroDTO() { 
                    Id = libro.LibroId,
                    Titulo = libro.Libro.Titulo
                });
            }

            return result;
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro) { 
            var result = new List<AutorLibro>();    

            if(libroCreacionDTO.AutoresIds == null) { return result; }

            foreach (var id in libroCreacionDTO.AutoresIds) {
                result.Add(new AutorLibro() { AutorId = id });
            }

            return result;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO) {
            var result = new List<AutorDTO>();

            if(libro.AutoresLibros == null) { return result; }

            foreach (var autorLibro in libro.AutoresLibros)
            {
                result.Add(new AutorDTO() { 
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre,
                    Apellido = autorLibro.Autor.Apellido
                });
            }

            return result;
        }
    }
}
