namespace TechnicianAgenda.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;

        // Navigation property - a client can have multiple addresses
        public List<Direction> Directions { get; set; } = new();
    }
}
