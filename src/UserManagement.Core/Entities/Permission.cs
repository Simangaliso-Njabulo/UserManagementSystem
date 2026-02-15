namespace UserManagement.Core.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
    }
}
