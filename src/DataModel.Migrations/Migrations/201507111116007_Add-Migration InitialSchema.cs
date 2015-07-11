namespace EFDDD.DataModel.Migrations.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMigrationInitialSchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Homes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DateBuilt = c.DateTime(),
                        Address = c.String(),
                        Neighborhood_Name = c.String(),
                        Neighborhood_HasHomeownersAssociation = c.Boolean(nullable: false),
                        Rooms = c.String(),
                        HomeOwnerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HomeOwners",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HomeOwners");
            DropTable("dbo.Homes");
        }
    }
}
