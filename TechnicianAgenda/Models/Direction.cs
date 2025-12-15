namespace TechnicianAgenda.Models
{
    public class Work
    {
        public int Id { get; set; }

        public string JobType { get; set; } = string.Empty; // "plumber" or "electricity"

        public DateTime Date { get; set; }

        public bool Status { get; set; } // false = not done, true = done

        // Foreign keys
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public int DirectionId { get; set; }
        public Direction Direction { get; set; } = null!;
    }
}
