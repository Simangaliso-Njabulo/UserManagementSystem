using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Core.Entities;

namespace UserManagement.Data.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(g => g.Description)
                .HasMaxLength(500);

            builder.HasIndex(g => g.Name)
                .IsUnique();
        }
    }
}