﻿using EFCore.Mapping.M2M.Scenarios.First.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFCore.Mapping.M2M.Scenarios.First
{
    public class DataContext : DbContext
    {
        private const string CONNECTION_STRING = "Data Source=TATARU\\SQLEXPRESS;Initial Catalog=Mapping-M2M-S1;Integrated Security=True";

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Group> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(CONNECTION_STRING)
                .LogTo(System.Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .EnableSensitiveDataLogging();
        }
    }
}
