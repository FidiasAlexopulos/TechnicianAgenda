namespace TechnicianAgenda.Models
{
    public class JobCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        // Navigation property
        public List<JobSubcategory> Subcategories { get; set; } = new();
    }
}
