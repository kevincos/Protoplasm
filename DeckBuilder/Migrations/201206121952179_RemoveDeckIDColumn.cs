namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDeckIDColumn : DbMigration
    {
        public override void Up()
        {
            
        }
        
        public override void Down()
        {
            AddColumn("Seats", "DeckId", c => c.Int(nullable: false));
        }
    }
}
