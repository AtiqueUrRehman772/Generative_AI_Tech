namespace Generative_AI_Tech.Models
{
    public class UserModal
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public List<SiteModal>? GenAiSites { get; set; }
    }
}
