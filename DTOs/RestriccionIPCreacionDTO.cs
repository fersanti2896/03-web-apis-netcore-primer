using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs {
    public class RestriccionIPCreacionDTO {
        public int LlaveId { get; set; }
        [Required]
        public string IP { get; set; }
    }
}
