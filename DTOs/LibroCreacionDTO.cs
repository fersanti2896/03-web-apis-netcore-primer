using AutoresAPI.Entities;
using AutoresAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs {
    public class LibroCreacionDTO {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [ValidacionAttribute]
        public string Titulo { get; set; }
        public List<int> AutoresIds { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
