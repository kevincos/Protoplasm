namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddGameModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Games",
                c => new
                    {
                        GameID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PythonScript = c.String(),
                        MaxPlayers = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GameID);
            
        }
        
        public override void Down()
        {
            DropTable("Games");
        }
    }
}
