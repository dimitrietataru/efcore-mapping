using EFCore.Mapping.M2M.Scenarios.Third.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFCore.Mapping.M2M.Scenarios.Third
{
    public class DataContext : DbContext
    {
        private const string CONNECTION_STRING = "Data Source=TATARU\\SQLEXPRESS;Initial Catalog=Mapping;Integrated Security=True";

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Group> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(CONNECTION_STRING)
                .LogTo(System.Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasMany(user => user.Groups)
                .WithMany(group => group.Users)
                    .UsingEntity<Membership>(
                        rightBuilder => rightBuilder
                            .HasOne(membership => membership.Group)
                            .WithMany(group => group.Memberships),
                        leftBuilder => leftBuilder
                            .HasOne(membership => membership.User)
                            .WithMany(user => user.Memberships))
                    .ToTable("GroupUser");
        }
    }
}
