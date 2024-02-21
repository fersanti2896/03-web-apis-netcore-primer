using System.ComponentModel.DataAnnotations;

namespace AutoresAPI.Entities {
    public class Libro {
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
    }
}
