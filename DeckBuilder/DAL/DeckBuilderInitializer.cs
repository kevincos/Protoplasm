using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DeckBuilder.Models;

namespace DeckBuilder.DAL
{
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
                new Player {Name = "KevinC", ProfileImageUrl="Content/Images/Kevin.png"},
                new Player {Name = "RachelS", ProfileImageUrl="Content/Images/Rachel.png"},
                new Player {Name = "Minotaur", ProfileImageUrl="Content/Images/Minotaur.png"},
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
                new Card { Name = "Mana Crystal",   ManaCost = 0, Description="Basic Mana Crystal.", CardArtUrl="Content/Images/ManaCrystal.png", CardType = cardTypes.Single(t => t.Name == "Crystal") },
                new Card { Name = "Amplifier Crystal", ManaCost = 0,    Description = "Crystal with long spell range but no mana generation.", CardArtUrl="Content/Images/AmplifierCrystal.png", CardType = cardTypes.Single(t => t.Name == "Crystal") },
                new Card { Name = "Power Crystal",   ManaCost = 0,     Description = "Crystal with high mana generation, but no spell range.", CardArtUrl="Content/Images/PowerCrystal.png", CardType = cardTypes.Single(t => t.Name == "Crystal") },
                new Card { Name = "Minotaur",   ManaCost = 2,     Description = "Powerful melee warrior.", CardArtUrl="Content/Images/Minotaur.png", CardType = cardTypes.Single(t => t.Name == "Summon") },
                new Card { Name = "Lightning Bolt",   ManaCost = 1,     Description = "Strikes a creature with a surge of lightning.", CardArtUrl="Content/Images/LightningBolt.png", CardType = cardTypes.Single(t => t.Name == "Spell") },
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
                new CardSet {DeckID = 2, CardID = 5, Quantity = 4},
                new CardSet {DeckID = 3, CardID = 1, Quantity = 10},
                new CardSet {DeckID = 3, CardID = 4, Quantity = 4}
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