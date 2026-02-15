using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Core.Entities;

namespace UserManagement.Data.Configurations
{
    public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
    {
        public void Configure(EntityTypeBuilder<UserGroup> builder)
        {
            // Composite primary key
            builder.HasKey(ug => new { ug.UserId, ug.GroupId });

            // Configure relationship to User
            builder.HasOne(ug => ug.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship to Group
            builder.HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(ug => ug.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}