using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DeckBuilder.Models;

namespace DeckBuilder.DAL
{
    
    //public class DeckBuilderInitializer : DeckBuilder.App_Start.DontDropDbJustCreateTablesIfModelChanged<DeckBuilderContext>
    public class DeckBuilderInitializer : DropCreateDatabaseIfModelChanges<DeckBuilderContext>    
    {
        protected override void Seed(DeckBuilderContext context)
        {
            var tables = new List<Table>
            {
                new Table(),
                new Table()
            };
            tables.ForEach(t => context.Tables.Add(t));
            context.SaveChanges();


            var players = new List<Player>
            {
                new Player {Name = "KevinC", ProfileImageUrl="/content/images/Kevin.png"},
                new Player {Name = "RachelS", ProfileImageUrl="/content/images/Rachel.png"},
                new Player {Name = "Minotaur", ProfileImageUrl="/content/images/Minotaur.png"},
            };
            players.ForEach(p => context.Players.Add(p));
            context.SaveChanges();

            var cardTypes = new List<CardType>
            {
                new CardType { Name = "Crystal" },
                new CardType { Name = "Summon" },
                new CardType { Name = "Spell" },
            };
            cardTypes.ForEach(t => context.CardTypes.Add(t));
            context.SaveChanges();

            var cards = new List<Card>
            {
                new Card { Name = "Mana Crystal",   ManaCost = 1, Description="Basic Mana Crystal.", CardArtUrl="/content/images/ManaCrystal.png", CardType = cardTypes.Single(t => t.Name == "Crystal"),  Crystal_Name = "Mana Crystal", Crystal_Url = "/content/images/manacrystal.png" },
                new Card { Name = "Minotaur",   ManaCost = 3,     Description = "Powerful half man half bull melee warrior.", CardArtUrl="/content/images/Minotaur.png", CardType = cardTypes.Single(t => t.Name == "Summon"), Unit_Attack =4, Unit_Awareness = "dad___", Unit_Defense = 2, Unit_MaxHP = 3, Unit_Speed = 2, Unit_Name = "Minotaur", Unit_Url= "/content/images/minotaur.png"},
                new Card { Name = "Hydra",   ManaCost = 6,     Description = "Multiheaded Serpant that can attack in all directions.", CardArtUrl="/content/images/hydra.png", CardType = cardTypes.Single(t => t.Name == "Summon"), Unit_Attack = 6, Unit_Awareness = "aaaaaa", Unit_Defense = 1, Unit_Speed = 1, Unit_MaxHP = 8, Unit_Name = "Hyrda", Unit_Url = "/content/images/hydra.png"},
                new Card { Name = "Raider",   ManaCost = 4,     Description = "Fast and deadly wolf rider.", CardArtUrl="/content/images/raiderportrait.png", CardType = cardTypes.Single(t => t.Name == "Summon"), Unit_Attack = 5, Unit_Awareness = "_a____", Unit_Defense = 0, Unit_Speed = 4, Unit_Name = "Raider", Unit_MaxHP = 3, Unit_Url = "/content/images/raider.png" },
                new Card { Name = "Lightning Bolt",   ManaCost = 1,     Description = "Strikes a creature with a surge of lightning.", CardArtUrl="/content/images/LightningBolt.png", CardType = cardTypes.Single(t => t.Name == "Spell") },
            };
            cards.ForEach(c => context.Cards.Add(c));
            context.SaveChanges();

            var decks = new List<Deck>
            {
                new Deck {PlayerId = 1, Name = "Balanced Deck"},
                new Deck {PlayerId = 2, Name = "Burn Deck"},
                new Deck {PlayerId = 3, Name = "Minotaur Deck"},
            };
            decks.ForEach(d => context.Decks.Add(d));
            context.SaveChanges();

            var cardSets = new List<CardSet>
            {
                new CardSet {PlayerID = 1, CardID = 1, Quantity = 10},
                new CardSet {PlayerID = 1, CardID = 2, Quantity = 10},
                new CardSet {PlayerID = 1, CardID = 3, Quantity = 10},
                new CardSet {PlayerID = 1, CardID = 4, Quantity = 10},
                new CardSet {PlayerID = 1, CardID = 5, Quantity = 10},
                new CardSet {PlayerID = 2, CardID = 1, Quantity = 10},
                new CardSet {PlayerID = 2, CardID = 5, Quantity = 10},
                new CardSet {PlayerID = 3, CardID = 1, Quantity = 10},
                new CardSet {PlayerID = 3, CardID = 4, Quantity = 10},
                new CardSet {DeckID = 1, CardID = 1, Quantity = 10},
                new CardSet {DeckID = 1, CardID = 2, Quantity = 6},
                new CardSet {DeckID = 1, CardID = 3, Quantity = 6},
                new CardSet {DeckID = 1, CardID = 4, Quantity = 4},
                new CardSet {DeckID = 1, CardID = 5, Quantity = 4},
                new CardSet {DeckID = 2, CardID = 1, Quantity = 10},
                new CardSet {DeckID = 2, CardID = 5, Quantity = 10},
                new CardSet {DeckID = 3, CardID = 1, Quantity = 10},
                new CardSet {DeckID = 3, CardID = 2, Quantity = 10}
            };
            cardSets.ForEach(c => context.CardSets.Add(c));
            context.SaveChanges();


            var seats = new List<Seat>
            {
                new Seat {PlayerId = 1, DeckId =1, TableId = 1, Active = true},
                new Seat {PlayerId = 2, DeckId =2, TableId = 1, Active = true},
                new Seat {PlayerId = 1, DeckId =1, TableId = 2},
                new Seat {PlayerId = 3, DeckId =3, TableId = 2}
            };
            seats.ForEach(s => context.Seats.Add(s));
            context.SaveChanges();
            


        }
    }
}