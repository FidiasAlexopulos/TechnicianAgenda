namespace TechnicianAgenda.Models
{
    public class Technician
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Nacionalidad { get; set; } = string.Empty;
        public string RutOPasaporte { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string? FotografiaPath { get; set; } = string.Empty;
        public Region Region { get; set; }
        public string Comuna { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;

        // Contact Information
        public string CorreoElectronico { get; set; } = string.Empty;
        public string NumeroTelefonico { get; set; } = string.Empty;
        public string? NumeroTelefonicoAlternativo { get; set; } = string.Empty;

        // Vehicle
        public string? PatenteVehiculo { get; set; } = string.Empty;

        // Certifications (stored as JSON or comma-separated string)
        public string Certificaciones { get; set; } = string.Empty;

        // Navigation property - trabajos asignados a este técnico
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Navigation property - trabajos asignados a este técnico
        public ICollection<Work> AssignedWorks { get; set; } = new List<Work>();
    }
}
