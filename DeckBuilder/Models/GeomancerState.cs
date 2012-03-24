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

    public class GeomancerState
    {
        public List<List<GeomancerTile>> tileList { get; set; }

        public List<GeomancerCard> hand {get; set;}
        public int deckCount {get; set;}

        public int opponentHandCount { get; set; }
        public int opponentDeckCount { get; set; }

        public void InitializeState()
        {
            tileList = new List<List<GeomancerTile>>();
            hand = new List<GeomancerCard>();
            deckCount = 20;
            for (int a = 0; a < 14; a++)
            {
                tileList.Add(new List<GeomancerTile>());
                for (int b = 0; b < 11; b++)
                {
                    int c = 18 - a - b;                    
                    if (c < 14 && c >=0)
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
            tileList[2][5].unit = new GeomancerUnit { name = "HomeCrystal", direction = 0, playerId = 0, hp = 17, maxHP = 20, url = "/content/images/homecrystalportrait.png", awareness = "______"};
            tileList[11][5].unit = new GeomancerUnit { name = "HomeCrystal", direction = 0, playerId = 1, hp = 20, maxHP = 20, url = "/content/images/homecrystalportrait.png", awareness = "______"};
            tileList[4][4].crystal = new GeomancerCrystal { name = "ManaCrystal", charged = false, playerId = 0, url = "/content/images/manacrystalportrait.png" };
            tileList[3][6].crystal = new GeomancerCrystal { name = "ManaCrystal", charged = false, playerId = 0, url = "/content/images/manacrystalportrait.png" };
            tileList[9][5].crystal = new GeomancerCrystal { name = "ManaCrystal", charged = true, playerId = 1, url = "/content/images/manacrystalportrait.png" };
            tileList[5][3].unit = new GeomancerUnit { name = "Minotaur", direction = 2, playerId = 0, hp = 2, maxHP = 3, attack = 4, defense = 2, speed = 2, url = "/content/images/minotaurportrait.png", awareness = "dad___" };
            tileList[4][5].unit = new GeomancerUnit { name = "Minotaur", direction = 3, playerId = 0, hp = 3, maxHP = 3, attack = 4, defense = 2, speed = 2, url = "/content/images/minotaurportrait.png", awareness = "dad___" };
            tileList[9][5].unit = new GeomancerUnit { name = "Hydra", direction = 3, playerId = 1, hp = 8, maxHP = 8, attack = 6, defense = 1, speed = 1, url = "/content/images/hydraportrait.png", awareness = "aaaaaa" };
            tileList[7][7].unit = new GeomancerUnit { name = "Raider", direction = 5, playerId = 1, hp = 3, maxHP = 3, attack = 5, defense = 0, speed = 4, url = "/content/images/raiderportrait.png" ,awareness = "_a____"};

            hand.Add(new GeomancerCard { name = "Summon Minotaur", description = "Summons a fearsome minotaur to the battlefield.", type = "Summon", cost = 3, castUnit = new GeomancerUnit { name = "Minotaur", direction = 3, playerId = 0, hp = 3, maxHP = 3, attack = 4, defense = 2, speed = 2, url = "/content/images/minotaurportrait.png", awareness = "dad___" }, url = "/content/images/minotaurportrait.png" });
            hand.Add(new GeomancerCard { name = "Summon Hydra", description = "Summons a powerful multiheaded serpant to the battlefield.", type = "Summon", cost = 3, castUnit = new GeomancerUnit { name = "Hydra", direction = 3, playerId = 1, hp = 8, maxHP = 8, attack = 6, defense = 1, speed = 1, url = "/content/images/hydraportrait.png", awareness = "aaaaaa" }, url = "/content/images/hydraportrait.png" });
            hand.Add(new GeomancerCard { name = "Lightning Bolt", description = "Blast a unit with a burst of lightning.", type = "Spell", cost = 2, url = "/content/images/lightningboltportrait.png" });
            hand.Add(new GeomancerCard { name = "Mana Crystal", description = "Conjures a mana crystal from the ground.", type = "Crystal", cost = 1, url = "/content/images/manacrystalportrait.png", castCrystal = new GeomancerCrystal { name = "ManaCrystal", charged = false, playerId = 0, url = "/content/images/manacrystalportrait.png" } });
            hand.Add(new GeomancerCard { name = "Mana Crystal", description = "Conjures a mana crystal from the ground.", type = "Crystal", cost = 1, url = "/content/images/manacrystalportrait.png", castCrystal = new GeomancerCrystal { name = "ManaCrystal", charged = false, playerId = 0, url = "/content/images/manacrystalportrait.png" } });

            opponentDeckCount = 30;
            opponentHandCount = 4;
        }
    }
}