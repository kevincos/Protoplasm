namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class DropCards : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Cards","Card_CardType");
            DropTable("CardTypes");

            DropForeignKey("CardSets", "Card_CardSets");
            DropColumn("CardSets", "CardID");
            DropForeignKey("CardSets", "CardSet_Player");
            DropColumn("CardSets", "PlayerID");
            DropForeignKey("CardSets", "Deck_CardSets");
            DropColumn("CardSets",  "DeckID");            
            
            DropTable("Decks");
            DropTable("Cards");
        }
        
        public override void Down()
        {
            
        }
    }
}
