namespace WsDualHttpBinding_SqlTableDependency_PoC.WCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using WsDualHttpBinding_SqlTableDependency_PoC.WCF.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DatabaseContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.Patients.AddOrUpdate(new Patient[]
            {
                new Patient { SortOrder = 1, ID = Guid.NewGuid(), FirstName = "Aragorn", LastName = "Elessar", Email = "aragorn.elessar@lotr.com", DateOfBirth = new DateTime(1825, 11, 21) },
                new Patient { SortOrder = 2, ID = Guid.NewGuid(), FirstName = "Tony", LastName = "Stark", Email = "tony.stark@avengers.com", DateOfBirth = new DateTime(1980, 6, 3) },
                new Patient { SortOrder = 3, ID = Guid.NewGuid(), FirstName = "Beric", LastName = "Dondarrion", Email = "beric.dondarrion@got.com", DateOfBirth = new DateTime(1800, 7, 11) },
                new Patient { SortOrder = 4, ID = Guid.NewGuid(), FirstName = "Stephen", LastName = "Strange", Email = "stephen.strange@avengers.com", DateOfBirth = new DateTime(1923, 3, 31) },
                new Patient { SortOrder = 5, ID = Guid.NewGuid(), FirstName = "Sherlock", LastName = "Holmes", Email = "sherlock.holmes@sh.com", DateOfBirth = new DateTime(1928, 9, 9) },
                new Patient { SortOrder = 6, ID = Guid.NewGuid(), FirstName = "Hakuna", LastName = "Matata", Email = "hakunamatata@lionking.com.sg", DateOfBirth = new DateTime(1823, 6, 15) },
                new Patient { SortOrder = 7, ID = Guid.NewGuid(), FirstName = "James", LastName = "Kirk", Email = "james.kirk@startrek.com", DateOfBirth = new DateTime(1954, 7, 1) }
            });
        }
    }
}
