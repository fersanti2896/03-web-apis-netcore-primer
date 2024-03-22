namespace AutoresAPI.Entities {
    public class PeticionAPI {
        public int Id { get; set; }
        public int LlaveId { get; set; }
        public DateTime FechaPeticion { get; set; }
        public LlaveAPI Llave { get; set; }
    }
}
