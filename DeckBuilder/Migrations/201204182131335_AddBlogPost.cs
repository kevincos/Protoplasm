namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddBlogPost : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Posts",
                c => new
                    {
                        PostID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        PlayerId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.PostID)
                .ForeignKey("Players", t => t.PlayerId, cascadeDelete: true)
                .Index(t => t.PlayerId);
            
        }
        
        public override void Down()
        {
            DropIndex("Posts", new[] { "PlayerId" });
            DropForeignKey("Posts", "PlayerId", "Players");
            DropTable("Posts");
        }
    }
}
