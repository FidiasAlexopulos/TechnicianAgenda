namespace TechnicianAgenda.Models
{
    public class Work
    {
        public int Id { get; set; }

        // Reemplaza JobType string por estas foreign keys:
        public int JobCategoryId { get; set; }
        public JobCategory JobCategory { get; set; } = null!;

        public int JobSubcategoryId { get; set; }
        public JobSubcategory JobSubcategory { get; set; } = null!;

        public string Detalles { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool Status { get; set; }

        public decimal Costos { get; set; } = 0;
        public decimal TotalACobrar { get; set; } = 0;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pendiente;

        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public int DirectionId { get; set; }
        public Direction Direction { get; set; } = null!;

        // Technician Assignment
        public int? TechnicianId { get; set; }
        public Technician? Technician { get; set; }

        // Payment to Technician
        public decimal PorPagarATecnico { get; set; } = 0;
        public bool PagoATecnicoRealizado { get; set; } = false;

        public List<WorkFile> Files { get; set; } = new();
    }
}