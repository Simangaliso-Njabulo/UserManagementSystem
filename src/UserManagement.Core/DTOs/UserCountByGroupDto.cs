namespace UserManagement.Core.DTOs
{
    public class UserCountByGroupDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int UserCount { get; set; }
    }
}