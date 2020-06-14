namespace WsDualHttpBinding_SqlTableDependency_PoC.WCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patient",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        FirstName = c.String(maxLength: 200),
                        LastName = c.String(maxLength: 200),
                        Email = c.String(maxLength: 500),
                        DateOfBirth = c.DateTime(),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Patient");
        }
    }
}
