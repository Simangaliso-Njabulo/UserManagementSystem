using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Core.Entities;

namespace UserManagement.Data.Configurations
{
    public class GroupPermissionConfiguration : IEntityTypeConfiguration<GroupPermission>
    {
        public void Configure(EntityTypeBuilder<GroupPermission> builder)
        {
            // Composite primary key
            builder.HasKey(gp => new { gp.GroupId, gp.PermissionId });

            // Configure relationship to Group
            builder.HasOne(gp => gp.Group)
                .WithMany(g => g.GroupPermissions)
                .HasForeignKey(gp => gp.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship to Permission
            builder.HasOne(gp => gp.Permission)
                .WithMany(p => p.GroupPermissions)
                .HasForeignKey(gp => gp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}