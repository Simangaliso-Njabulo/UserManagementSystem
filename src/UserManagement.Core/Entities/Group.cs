namespace UserManagement.Core.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        public ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
    }
}