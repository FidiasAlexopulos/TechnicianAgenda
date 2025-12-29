namespace TechnicianAgenda.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Direction> Directions { get; set; } = new List<Direction>();
        public ICollection<Work> Works { get; set; } = new List<Work>();
        public bool IsActive { get; set; } = true;

    }
}