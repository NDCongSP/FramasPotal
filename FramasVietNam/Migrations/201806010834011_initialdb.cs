namespace FramasVietNam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialdb : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Todoes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Todoes",
                c => new
                    {
                        todoId = c.Int(nullable: false, identity: true),
                        todoName = c.String(),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.todoId);
            
        }
    }
}
