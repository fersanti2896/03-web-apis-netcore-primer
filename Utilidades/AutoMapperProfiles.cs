using AutoMapper;
using AutoresAPI.DTOs;
using AutoresAPI.Entities;

namespace AutoresAPI.Utilidades {
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<LibroCreacionDTO, Libro>();
            CreateMap<Libro, LibroDTO>();
        }
    }
}
