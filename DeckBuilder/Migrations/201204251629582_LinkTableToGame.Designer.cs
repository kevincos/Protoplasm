// <auto-generated />
namespace DeckBuilder.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    
    public sealed partial class LinkTableToGame : IMigrationMetadata
    {
        string IMigrationMetadata.Id
        {
            get { return "201204251629582_LinkTableToGame"; }
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