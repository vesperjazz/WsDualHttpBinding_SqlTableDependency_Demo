using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WsDualHttpBinding_SqlTableDependency_PoC.WCF.Entities;

namespace WsDualHttpBinding_SqlTableDependency_PoC.WCF
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }

        public DatabaseContext() : base("name=DatabaseContext") { }
        public DatabaseContext(string connectionString) : base(connectionString) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .ToTable(nameof(Patient));
            modelBuilder.Entity<Patient>()
                .Property(p => p.LastName).HasMaxLength(200);
            modelBuilder.Entity<Patient>()
                .Property(p => p.FirstName).HasMaxLength(200);
            modelBuilder.Entity<Patient>()
                .Property(p => p.Email).HasMaxLength(500);
        }
    }
}