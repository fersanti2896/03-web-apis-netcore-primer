using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.Entities {
    public class Libro {
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        public int AutorId { get; set; }
        public Autor Autor { get; set; }
        public List<Comentario> Comentarios { get; set; }
    }
}
