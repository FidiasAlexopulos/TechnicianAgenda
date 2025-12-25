namespace TechnicianAgenda.Models
{
    public class WorkFile
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty; // "image" or "video"
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // Foreign key
        public int WorkId { get; set; }
        public Work Work { get; set; } = null!;
    }
}
