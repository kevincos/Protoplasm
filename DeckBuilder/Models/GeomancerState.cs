using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Models
{
    public class GeomancerUnit
    {
        public string name { get; set; }
        public int direction { get; set; }
        public int playerId { get; set; }
        public int hp { get; set; }
        public int maxHP { get; set; }

        public int attack { get; set; }
        public int defense { get; set; }
        public int speed { get; set; }

        public string awareness { get; set; }

        public string url { get; set; }

        public int moveA { get; set; }
        public int moveB { get; set; }

        public bool used { get; set; }
    }

    public class GeomancerCrystal
    {
        public string name { get; set; }
        public bool charged { get; set; }
        public int playerId { get; set; }

        public string url { get; set; }
    }

    public class GeomancerSpell
    {
        public string name { get; set; }
        public int direction { get; set; }
        public int playerId { get; set; }

        public string url { get; set; }

        public int sourceCardIndex { get; set; }
    }

    public class GeomancerTile
    {
        public string type { get; set; }
        public GeomancerUnit unit { get; set; }
        public GeomancerUnit moveUnit { get; set; }
        public GeomancerCrystal crystal { get; set; }
        public GeomancerSpell spell { get; set; }

        public int elevation { get; set; }

    }

    public class GeomancerCard
    {
        public string name { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public int cost { get; set; }

        public string url { get; set; }

        public GeomancerUnit castUnit { get; set; }
        public GeomancerCrystal castCrystal { get; set; }
        
        public int castA { get; set; }
        public int castB { get; set; }

        public bool used { get; set; }

        public GeomancerCard()
        {            
        }
    }

    public class GeomancerPlayerContext
    {
        public int playerId { get; set; }
        public List<GeomancerCard> hand { get; set; }
        public List<GeomancerCard> deck { get; set; }
        public int handCount { get; set; }
        public int deckCount { get; set; }

        public GeomancerPlayerContext()
        {
        }

        public GeomancerPlayerContext(int newPlayerId)
        {
            hand = new List<GeomancerCard>();
            deck = new List<GeomancerCard>();
            handCount = 0;
            deckCount = 0;
            this.playerId = newPlayerId;
        }
    }

    public class GeomancerState
    {
        public List<List<GeomancerTile>> tileList { get; set; }

        public List<GeomancerPlayerContext> playerContexts { get; set; }
        public int activePlayerIndex { get; set; }

        public GeomancerState()
        {
            tileList = new List<List<GeomancerTile>>();

            playerContexts = new List<GeomancerPlayerContext>();
            activePlayerIndex = 0;
            for (int a = 0; a < 14; a++)
            {
                tileList.Add(new List<GeomancerTile>());
                for (int b = 0; b < 11; b++)
                {
                    int c = 18 - a - b;
                    if (c < 14 && c >= 0)
                    {
                        if (a > 4 && a < 9 && b > 3 && b < 7 && c > 4 && c < 9)
                        {
                            tileList[a].Add(new GeomancerTile { type = "Barren", elevation = 1, unit = null });
                        }
                        else
                        {
                            tileList[a].Add(new GeomancerTile { type = "Normal", elevation = 1, unit = null });
                        }
                    }
                    else
                    {
                        tileList[a].Add(null);
                    }
                }
            }
        }

        public void AddCardSet(CardSet set, GeomancerPlayerContext playerContext)
        {
            
            for (int i = 0; i < set.Quantity; i++)
            {
                GeomancerCard newCard = new GeomancerCard { name = set.Card.Name, url = set.Card.CardArtUrl, description = set.Card.Description, cost = set.Card.ManaCost };
                if (set.Card.CardType.Name == "Summon")
                {
                    newCard.type = "Summon";
                    newCard.castUnit = new GeomancerUnit { name = set.Card.Unit_Name, awareness = set.Card.Unit_Awareness, attack = set.Card.Unit_Attack, defense = set.Card.Unit_Defense, maxHP = set.Card.Unit_MaxHP, speed = set.Card.Unit_Speed, url = set.Card.Unit_Url, playerId = 0 };
                }
                if (set.Card.CardType.Name == "Crystal")
                {
                    newCard.type = "Crystal";
                    newCard.castCrystal = new GeomancerCrystal { name = set.Card.Crystal_Name, url = set.Card.Crystal_Url };
                }
                playerContext.deck.Add(newCard);
                playerContext.deckCount++;
            }
        }

        public void DrawCard(Random r, GeomancerPlayerContext playerContext)
        {
            int randomIndex = r.Next(0, playerContext.deck.Count);
            playerContext.hand.Add(playerContext.deck.ElementAt(randomIndex));
            playerContext.deck.RemoveAt(randomIndex);
            playerContext.deckCount--;
            playerContext.handCount++;
        }

        public void InitializeState(List<Deck> decks)
        {
            // Initialize Home Crystals
            if (decks.Count == 2)
            {
                tileList[2][5].unit = new GeomancerUnit { name = "HomeCrystal", direction = 0, playerId = decks[0].PlayerId, hp = 17, maxHP = 20, url = "/content/images/homecrystal.png", awareness = "______" };
                tileList[11][5].unit = new GeomancerUnit { name = "HomeCrystal", direction = 0, playerId = decks[1].PlayerId, hp = 20, maxHP = 20, url = "/content/images/homecrystal.png", awareness = "______" };
            }
            else
            {
                throw new Exception("Unsupported number of players");
            }

            // Populate initial player decks
            Random r = new Random();
            foreach (Deck deck in decks)
            {
                GeomancerPlayerContext playerContext = new GeomancerPlayerContext(deck.PlayerId);
                
                foreach (CardSet set in deck.CardSets)
                {
                    AddCardSet(set, playerContext);
                }

                for (int i = 0; i < 5; i++)
                {
                    DrawCard(r, playerContext);
                }

                playerContexts.Add(playerContext);
            }
        }

        public void InitializeState()
        {
            GeomancerPlayerContext player1Context = new GeomancerPlayerContext(0);
            GeomancerPlayerContext player2Context = new GeomancerPlayerContext(1);

            player1Context.playerId = 0;
            player2Context.playerId = 1;
            
            tileList[2][5].unit = new GeomancerUnit { name = "HomeCrystal", direction = 0, playerId = 0, hp = 17, maxHP = 20, url = "/content/images/homecrystal.png", awareness = "______"};
            tileList[11][5].unit = new GeomancerUnit { name = "HomeCrystal", direction = 0, playerId = 1, hp = 20, maxHP = 20, url = "/content/images/homecrystal.png", awareness = "______"};
            tileList[4][4].crystal = new GeomancerCrystal { name = "ManaCrystal", charged = false, playerId = 0, url = "/content/images/manacrystal.png" };
            tileList[3][6].crystal = new GeomancerCrystal { name = "ManaCrystal", charged = false, playerId = 0, url = "/content/images/manacrystal.png" };
            tileList[9][5].crystal = new GeomancerCrystal { name = "ManaCrystal", charged = true, playerId = 1, url = "/content/images/manacrystal.png" };
            tileList[5][3].unit = new GeomancerUnit { name = "Minotaur", direction = 2, playerId = 0, hp = 2, maxHP = 3, attack = 4, defense = 2, speed = 2, url = "/content/images/minotaur.png", awareness = "dad___" };
            tileList[4][5].unit = new GeomancerUnit { name = "Minotaur", direction = 3, playerId = 0, hp = 3, maxHP = 3, attack = 4, defense = 2, speed = 2, url = "/content/images/minotaur.png", awareness = "dad___" };
            tileList[9][5].unit = new GeomancerUnit { name = "Hydra", direction = 3, playerId = 1, hp = 8, maxHP = 8, attack = 6, defense = 1, speed = 1, url = "/content/images/hydra.png", awareness = "aaaaaa" };
            tileList[7][7].unit = new GeomancerUnit { name = "Raider", direction = 5, playerId = 1, hp = 3, maxHP = 3, attack = 5, defense = 0, speed = 4, url = "/content/images/raider.png" ,awareness = "_a____"};

            player1Context.hand.Add(new GeomancerCard { name = "Summon Minotaur", description = "Summons a fearsome minotaur to the battlefield.", type = "Summon", cost = 3, castUnit = new GeomancerUnit { name = "Minotaur", direction = 3, playerId = 0, hp = 3, maxHP = 3, attack = 4, defense = 2, speed = 2, url = "/content/images/minotaur.png", awareness = "dad___" }, url = "/content/images/minotaur.png" });
            player1Context.hand.Add(new GeomancerCard { name = "Summon Hydra", description = "Summons a powerful multiheaded serpant to the battlefield.", type = "Summon", cost = 3, castUnit = new GeomancerUnit { name = "Hydra", direction = 3, playerId = 1, hp = 8, maxHP = 8, attack = 6, defense = 1, speed = 1, url = "/content/images/hydra.png", awareness = "aaaaaa" }, url = "/content/images/hydra.png" });
            player1Context.hand.Add(new GeomancerCard { name = "Lightning Bolt", description = "Blast a unit with a burst of lightning.", type = "Spell", cost = 2, url = "/content/images/lightningbolt.png" });
            player1Context.hand.Add(new GeomancerCard { name = "Mana Crystal", description = "Conjures a mana crystal from the ground.", type = "Crystal", cost = 1, url = "/content/images/manacrystal.png", castCrystal = new GeomancerCrystal { name = "ManaCrystal", charged = false, playerId = 0, url = "/content/images/manacrystal.png" } });
            player1Context.hand.Add(new GeomancerCard { name = "Mana Crystal", description = "Conjures a mana crystal from the ground.", type = "Crystal", cost = 1, url = "/content/images/manacrystal.png", castCrystal = new GeomancerCrystal { name = "ManaCrystal", charged = false, playerId = 0, url = "/content/images/manacrystal.png" } });
            player1Context.handCount = 5;

            player2Context.hand.Add(new GeomancerCard { name = "Mana Crystal", description = "Conjures a mana crystal from the ground.", type = "Crystal", cost = 1, url = "/content/images/manacrystal.png", castCrystal = new GeomancerCrystal { name = "ManaCrystal", charged = false, playerId = 0, url = "/content/images/manacrystal.png" } });
            player2Context.hand.Add(new GeomancerCard { name = "Mana Crystal", description = "Conjures a mana crystal from the ground.", type = "Crystal", cost = 1, url = "/content/images/manacrystal.png", castCrystal = new GeomancerCrystal { name = "ManaCrystal", charged = false, playerId = 0, url = "/content/images/manacrystal.png" } });
            player2Context.handCount = 2;

            playerContexts.Add(player1Context);
            playerContexts.Add(player2Context);
            activePlayerIndex = 0;
        }
    }
}