using AutoresAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.Entities {
    public class Autor {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [ValidacionAttribute]
        public string Apellido { get; set; }
        public List<Libro> Libros { get; set; }
    }
}
