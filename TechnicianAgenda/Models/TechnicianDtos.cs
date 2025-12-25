namespace TechnicianAgenda.DTOs
{
    // DTO para mostrar información resumida del técnico en la asignación de trabajos
    public class TechnicianSummaryDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string RutOPasaporte { get; set; } = string.Empty;

        // Nombre completo para facilitar el display
        public string NombreCompleto => $"{Nombre} {Apellidos}";
    }

    // DTO completo para crear/editar técnico
    public class TechnicianDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Nacionalidad { get; set; } = string.Empty;
        public string RutOPasaporte { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string? FotografiaPath { get; set; }
        public int Region { get; set; }
        public string Comuna { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string NumeroTelefonico { get; set; } = string.Empty;
        public string? NumeroTelefonicoAlternativo { get; set; }
        public string? PatenteVehiculo { get; set; }
        public string Certificaciones { get; set; } = string.Empty;
    }
}