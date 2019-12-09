namespace EntityFrameworkIncludeSelfReference.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class SelfReference : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.Activity",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    Activity_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Activity", t => t.Activity_Id)
                .Index(t => t.Activity_Id);

            CreateTable(
                "public.Document",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    Activity_Id = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.Activity", t => t.Activity_Id)
                .Index(t => t.Activity_Id);
        }

        public override void Down()
        {
            DropForeignKey("public.Document", "Activity_Id", "public.Activity");
            DropForeignKey("public.Activity", "Activity_Id", "public.Activity");
            DropIndex("public.Document", new[] { "Activity_Id" });
            DropIndex("public.Activity", new[] { "Activity_Id" });
            DropTable("public.Document");
            DropTable("public.Activity");
        }
    }
}