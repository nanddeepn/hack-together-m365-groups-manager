namespace hack_together_groups_manager.Models
{
    public class M365Group
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Visibility { get; set; }
        public string? Url { get; set; }
        public string? Thumbnail { get; set; }
        public string? UserRole { get; set; }
        public bool TeamsConnected { get; set; }
    }
}
