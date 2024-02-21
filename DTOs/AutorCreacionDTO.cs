using AutoresAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs {
    public class AutorCreacionDTO {

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [ValidacionAttribute]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [ValidacionAttribute]
        public string Apellido { get; set; }
    }
}
