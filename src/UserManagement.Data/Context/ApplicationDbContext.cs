using Microsoft.EntityFrameworkCore;
using UserManagement.Core.Entities;
using UserManagement.Data.Configurations;

namespace UserManagement.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<GroupPermission> GroupPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new UserGroupConfiguration());
            modelBuilder.ApplyConfiguration(new GroupPermissionConfiguration());

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Groups
            modelBuilder.Entity<Group>().HasData(
                new Group { Id = 1, Name = "Admin", Description = "Administrator group with full access" },
                new Group { Id = 2, Name = "Level 1", Description = "Basic user access level" },
                new Group { Id = 3, Name = "Level 2", Description = "Intermediate user access level" }
            );

            // Seed Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "Read", Description = "Read access to resources" },
                new Permission { Id = 2, Name = "Write", Description = "Write access to resources" },
                new Permission { Id = 3, Name = "Delete", Description = "Delete access to resources" },
                new Permission { Id = 4, Name = "Manage Users", Description = "Manage user accounts" }
            );

            // Seed GroupPermissions
            modelBuilder.Entity<GroupPermission>().HasData(
                // Admin has all permissions
                new GroupPermission { GroupId = 1, PermissionId = 1 },
                new GroupPermission { GroupId = 1, PermissionId = 2 },
                new GroupPermission { GroupId = 1, PermissionId = 3 },
                new GroupPermission { GroupId = 1, PermissionId = 4 },
                // Level 1 has Read only
                new GroupPermission { GroupId = 2, PermissionId = 1 },
                // Level 2 has Read and Write
                new GroupPermission { GroupId = 3, PermissionId = 1 },
                new GroupPermission { GroupId = 3, PermissionId = 2 }
            );

            // Seed sample Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@example.com",
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 3,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed UserGroups
            modelBuilder.Entity<UserGroup>().HasData(
                new UserGroup { UserId = 1, GroupId = 1 }, // Admin User -> Admin Group
                new UserGroup { UserId = 2, GroupId = 2 }, // John -> Level 1
                new UserGroup { UserId = 3, GroupId = 3 }  // Jane -> Level 2
            );
        }
    }
}