namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDecks2 : DbMigration
    {
        public override void Up()
        {
            /*Sql(
                @"ALTER TABLE Seats
                  DROP CONSTRAINT Deck");            */
            //DropTable("Seat_Deck");            
            
            
        }
        
        public override void Down()
        {
            DropColumn("Seats", "DeckId");
        }
    }
}
