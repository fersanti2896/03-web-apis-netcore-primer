using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs {
    public class RestriccionesCreacionDTO {
        public int LlaveId { get; set; }
        [Required]
        public string Dominio { get; set; }
    }
}
