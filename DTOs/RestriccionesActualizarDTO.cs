using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTO {
    public class RestriccionesActualizarDTO {
        [Required]
        public string Dominio { get; set; }
    }
}
