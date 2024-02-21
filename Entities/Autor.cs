
namespace AutoresAPI.Entities {
    public class Autor {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
    }
}
