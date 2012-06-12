// <auto-generated />
namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    
    public sealed partial class RemoveDeckIDColumn : IMigrationMetadata
    {
        string IMigrationMetadata.Id
        {
            get { return "201206121952179_RemoveDeckIDColumn"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return "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"; }
        }
    }
}
