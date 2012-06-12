namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDeck : DbMigration
    {
        public override void Up()
        {
            /*DropForeignKey("Cards", "CardTypeId", "CardTypes");
            DropForeignKey("Seats", "DeckId", "Decks");
            DropIndex("Cards", new[] { "CardTypeId" });
            DropIndex("Seats", new[] { "DeckId" });
            AlterColumn("Cards", "CardTypeId", c => c.Int());
            DropColumn("Seats", "DeckId");
            DropTable("CardTypes");*/
        }
        
        public override void Down()
        {
            CreateTable(
                "CardTypes",
                c => new
                    {
                        CardTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.CardTypeId);
            
            AddColumn("Seats", "DeckId", c => c.Int(nullable: false));
            AlterColumn("Cards", "CardTypeId", c => c.Int(nullable: false));
            CreateIndex("Seats", "DeckId");
            CreateIndex("Cards", "CardTypeId");
            AddForeignKey("Seats", "DeckId", "Decks", "DeckID");
            AddForeignKey("Cards", "CardTypeId", "CardTypes", "CardTypeId", cascadeDelete: true);
        }
    }
}
