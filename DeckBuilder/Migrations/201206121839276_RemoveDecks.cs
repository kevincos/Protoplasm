namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDecks : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("CardSets", "CardID", "Cards");
            //DropForeignKey("CardSets", "PlayerID", "Players");
            //DropForeignKey("CardSets", "DeckID", "Decks");
            //DropForeignKey("Decks", "PlayerId", "Players");
            //DropIndex("CardSets", new[] { "CardID" });
            //DropIndex("CardSets", new[] { "PlayerID" });
            //DropIndex("CardSets", new[] { "DeckID" });
            //DropIndex("Decks", new[] { "PlayerId" });
            //DropTable("Cards");
            //DropTable("CardSets");
            //DropTable("Decks");
        }
        
        public override void Down()
        {
            CreateTable(
                "Decks",
                c => new
                    {
                        DeckID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 160),
                        PlayerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DeckID);
            
            CreateTable(
                "CardSets",
                c => new
                    {
                        CardSetID = c.Int(nullable: false, identity: true),
                        CardID = c.Int(nullable: false),
                        PlayerID = c.Int(),
                        DeckID = c.Int(),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CardSetID);
            
            CreateTable(
                "Cards",
                c => new
                    {
                        CardID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ManaCost = c.Int(nullable: false),
                        Description = c.String(maxLength: 1024),
                        CardArtUrl = c.String(maxLength: 1024),
                        Crystal_Name = c.String(maxLength: 50),
                        Crystal_Url = c.String(maxLength: 1024),
                        Crystal_Range = c.Int(nullable: false),
                        Crystal_Mana = c.Int(nullable: false),
                        Unit_Name = c.String(maxLength: 50),
                        Unit_Url = c.String(maxLength: 1024),
                        Unit_MaxHP = c.Int(nullable: false),
                        Unit_Attack = c.Int(nullable: false),
                        Unit_Defense = c.Int(nullable: false),
                        Unit_Speed = c.Int(nullable: false),
                        Unit_Awareness = c.String(),
                        CardTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CardID);
            
            CreateIndex("Decks", "PlayerId");
            CreateIndex("CardSets", "DeckID");
            CreateIndex("CardSets", "PlayerID");
            CreateIndex("CardSets", "CardID");
            AddForeignKey("Decks", "PlayerId", "Players", "PlayerID", cascadeDelete: true);
            AddForeignKey("CardSets", "DeckID", "Decks", "DeckID");
            AddForeignKey("CardSets", "PlayerID", "Players", "PlayerID");
            AddForeignKey("CardSets", "CardID", "Cards", "CardID", cascadeDelete: true);
        }
    }
}
