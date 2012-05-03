namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LinkTableToGame : DbMigration
    {
        public override void Up()
        {
            AddColumn("Tables", "GameId", c => c.Int(nullable: false));
            AddForeignKey("Tables", "GameId", "Games", "GameID", cascadeDelete: true);
            CreateIndex("Tables", "GameId");
            DropColumn("Tables", "Game");
        }
        
        public override void Down()
        {
            AddColumn("Tables", "Game", c => c.String());
            DropIndex("Tables", new[] { "GameId" });
            DropForeignKey("Tables", "GameId", "Games");
            DropColumn("Tables", "GameId");
        }
    }
}
