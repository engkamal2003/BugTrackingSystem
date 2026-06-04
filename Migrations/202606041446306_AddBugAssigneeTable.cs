namespace BugTrackingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBugAssigneeTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bugs", "AssignedTo", "dbo.Users");
            DropIndex("dbo.Bugs", new[] { "AssignedTo" });
            CreateTable(
                "dbo.BugAssignees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BugId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedBy = c.Int(),
                        UpdatedAt = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedBy = c.Int(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bugs", t => t.BugId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.BugId)
                .Index(t => t.UserId);
            
            DropColumn("dbo.Bugs", "AssignedTo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bugs", "AssignedTo", c => c.Int());
            DropForeignKey("dbo.BugAssignees", "UserId", "dbo.Users");
            DropForeignKey("dbo.BugAssignees", "BugId", "dbo.Bugs");
            DropIndex("dbo.BugAssignees", new[] { "UserId" });
            DropIndex("dbo.BugAssignees", new[] { "BugId" });
            DropTable("dbo.BugAssignees");
            CreateIndex("dbo.Bugs", "AssignedTo");
            AddForeignKey("dbo.Bugs", "AssignedTo", "dbo.Users", "Id");
        }
    }
}
