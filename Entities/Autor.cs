using AutoresAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.Entities {
    public class Autor : IValidatableObject {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [ValidacionAttribute]
        public string Apellido { get; set; }
        public List<Libro> Libros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre)) {
                var primeraLetra = Nombre[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper()) {
                    yield return new ValidationResult("La primera letra debe ser mayúscula", new string[] { nameof(Nombre) });
                }
            }
        }
    }
}
