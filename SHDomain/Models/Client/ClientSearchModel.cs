namespace SHDomain.Models.Client
{
    public class ClientSearchModel
    {
        public int? Id { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? ProjectName { get; set; } = string.Empty;
    }
}
