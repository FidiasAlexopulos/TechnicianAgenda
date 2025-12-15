namespace TechnicianAgenda.Models
{
    public class Direction
    {
        public int Id { get; set; }
        public string Address { get; set; } = string.Empty;

        // Foreign key
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
    }
}