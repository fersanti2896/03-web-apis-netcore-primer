using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.DTOs {
    public class RolDTO {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
