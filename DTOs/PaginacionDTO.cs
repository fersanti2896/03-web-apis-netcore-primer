namespace AutoresAPI.DTOs {
    public class PaginacionDTO {
        public int Pagina { get; set; }
        private int elementos = 10;
        private readonly int limite = 50;

        public int Elementos { get { return elementos; } set { elementos = (value > limite) ? limite : value; } }
    }
}
