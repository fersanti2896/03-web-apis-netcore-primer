namespace AutoresAPI.Entities {
    public class Factura {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public bool Pago { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaLimitePago { get; set; }
    }
}
