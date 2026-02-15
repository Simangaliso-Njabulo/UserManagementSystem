using System.ComponentModel.DataAnnotations;

namespace UserManagement.Core.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        public List<int> GroupIds { get; set; } = new List<int>();
    }
}