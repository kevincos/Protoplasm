namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddCrystalData : DbMigration
    {
        public override void Up()
        {
            AddColumn("Cards", "Crystal_Range", c => c.Int(nullable: false, defaultValue:2));
            AddColumn("Cards", "Crystal_Mana", c => c.Int(nullable: false, defaultValue:1));
        }
        
        public override void Down()
        {
            DropColumn("Cards", "Crystal_Mana");
            DropColumn("Cards", "Crystal_Range");
        }
    }
}
