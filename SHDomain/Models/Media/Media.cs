namespace SHDomain.Models.Media
{
    public class Media
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public byte[] FileData { get; set; } = [];
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }
}
