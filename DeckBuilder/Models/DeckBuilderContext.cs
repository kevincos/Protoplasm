using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DeckBuilder.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Migrations;
using DeckBuilder.Migrations;

namespace DeckBuilder.Models
{
    public class DeckBuilderContext: DbContext
    {
        public DbSet<Post> Posts { get; set; }

        public DbSet<Player> Players { get; set; }
        public DbSet<Card> Cards { get; set; }
        
        public DbSet<Table> Tables { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<CardSet> CardSets { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<MatchRequest> MatchRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Seat>()
                .HasRequired(s => s.Deck)
                .WithMany()
                .WillCascadeOnDelete(false);*/


            base.OnModelCreating(modelBuilder);
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DeckBuilderContext, Configuration>());
        }

        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Seat>()
                    .HasRequired(s => s.Deck)
                    .WithMany()
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Card>()
                    .HasRequired(c => c.CardType);
                    
            
                
                

            base.OnModelCreating(modelBuilder);
        }*/

        public DbSet<Game> Games { get; set; }
        public DbSet<GameVersion> Versions { get; set; }
        
    }
}