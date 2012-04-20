namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddGameToTableModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("Tables", "Game", c => c.String());
            DropColumn("Tables", "TotalTurns");
            DropColumn("Tables", "Draws");
            DropColumn("Tables", "Results");
            DropColumn("Tables", "FinalResults");
        }
        
        public override void Down()
        {
            AddColumn("Tables", "FinalResults", c => c.String());
            AddColumn("Tables", "Results", c => c.String());
            AddColumn("Tables", "Draws", c => c.Int(nullable: false));
            AddColumn("Tables", "TotalTurns", c => c.Int(nullable: false));
            DropColumn("Tables", "Game");
        }
    }
}
