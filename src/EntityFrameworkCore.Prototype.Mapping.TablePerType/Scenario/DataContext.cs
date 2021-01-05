using EFCore.Mapping.TPT.Scenario.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFCore.Mapping.TPT.Scenario
{
    public class DataContext : DbContext
    {
        private const string CONNECTION_STRING = "Data Source=TATARU\\SQLEXPRESS;Initial Catalog=Mapping-TPT;Integrated Security=True";

        public virtual DbSet<Animal> Animals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(CONNECTION_STRING)
                .LogTo(System.Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cat>().ToTable("Cats");
            modelBuilder.Entity<Dog>().ToTable("Dogs");
        }
    }
}
