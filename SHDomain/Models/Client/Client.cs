namespace SHDomain.Models.Client
{
    public class Client : BaseEntity
    {
        public string ClientName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
