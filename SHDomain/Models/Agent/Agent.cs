namespace SHDomain.Models.Agent
{
    public class Agent : BaseEntity
    {
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Dealer { get; set; }
        public string? Type { get; set; }
        public string? Address { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
