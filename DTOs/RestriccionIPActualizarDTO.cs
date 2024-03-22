using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs {
    public class RestriccionIPActualizarDTO {
        [Required]
        public string IP { get; set; }
    }
}
