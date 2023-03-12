namespace hack_together_groups_manager.Models
{
    public class M365Group
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Visibility { get; set; }

        public List<string>? Owners { get; set; }
        public List<string>? Members { get; set; }
    }
}
