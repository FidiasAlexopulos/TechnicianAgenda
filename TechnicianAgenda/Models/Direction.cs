namespace TechnicianAgenda.Models
{
    public class Direction
    {
        public int Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public Region Region { get; set; }
        public string Comuna { get; set; } = string.Empty; //Lista de Comunas de las Regiones Chile
        public string Referencia { get; set; } = string.Empty;

        // Foreign key
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
    }
}