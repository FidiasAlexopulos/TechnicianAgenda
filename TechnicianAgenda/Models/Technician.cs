namespace TechnicianAgenda.Models
{
    public class Technician
    {
        public int Id { get; set; }

        // Personal Information
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Nacionalidad { get; set; } = string.Empty;
        public string RutOPasaporte { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }

        // Photo
        public string? FotografiaPath { get; set; } = string.Empty;

        // Address
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
        public List<Work> AssignedWorks { get; set; } = new();
    }
}
