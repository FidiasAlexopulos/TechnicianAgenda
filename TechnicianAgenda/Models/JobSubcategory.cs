namespace TechnicianAgenda.Models
{
    public class JobSubcategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        // Foreign key
        public int JobCategoryId { get; set; }
        public JobCategory JobCategory { get; set; } = null!;
    }
}