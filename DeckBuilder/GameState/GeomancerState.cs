using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;

namespace DeckBuilder.Games
{
    public static class HexHelpers
    {
        public static int Distance(int a1, int b1, int a2, int b2, int constraint)
        {
            int c1 = constraint - a1 - b1;
            int c2 = constraint - a2 - b2;
            return (Math.Abs(a1 - a2) + Math.Abs(b1 - b2) + Math.Abs(c1 - c2))/2;
        }
    }

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

        public bool attacking { get; set; }
        
        public bool used { get; set; }
    }

    public class GeomancerCrystal
    {
        public string name { get; set; }
        public int mana { get; set; }
        public int range { get; set; }
        public int playerId { get; set; }

        public string url { get; set; }

        public bool used { get; set; }
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
        public bool defeated { get; set; }

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
        public int sourcePlayerId { get; set; }
        public int tableId { get; set; }
        public bool gameOver { get; set; }

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
            
        }

        public void DrawCard(Random r, GeomancerPlayerContext playerContext)
        {
            int randomIndex = r.Next(0, playerContext.deck.Count);
            playerContext.hand.Add(playerContext.deck.ElementAt(randomIndex));
            playerContext.deck.RemoveAt(randomIndex);
            playerContext.deckCount--;
            playerContext.handCount++;
        }

        public void InitializeState(List<Seat> seats)
        {
            tableId = seats[0].TableId;
            // Initialize Home Crystals
            if (seats.Count == 2)
            {
                tileList[2][5].unit = new GeomancerUnit { name = "HomeCrystal", direction = 0, awareness = "dddddd", playerId = seats[0].PlayerId, hp = 17, maxHP = 20, url = "/content/images/homecrystal.png"};
                tileList[11][5].unit = new GeomancerUnit { name = "HomeCrystal", direction = 0, awareness = "dddddd", playerId = seats[1].PlayerId, hp = 20, maxHP = 20, url = "/content/images/homecrystal.png"};
            }
            else
            {
                throw new Exception("Unsupported number of players");
            }

            // Populate initial player decks
            Random r = new Random();
            foreach (Seat seat in seats)
            {
                Deck deck = seat.Deck;
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
            tileList[4][4].crystal = new GeomancerCrystal { name = "ManaCrystal", mana = 1, playerId = 0, url = "/content/images/manacrystal.png" };
            tileList[3][6].crystal = new GeomancerCrystal { name = "ManaCrystal", mana = 1, playerId = 0, url = "/content/images/manacrystal.png" };
            tileList[9][5].crystal = new GeomancerCrystal { name = "ManaCrystal", mana = 1, playerId = 1, url = "/content/images/manacrystal.png" };
            tileList[5][3].unit = new GeomancerUnit { name = "Minotaur", direction = 2, playerId = 0, hp = 2, maxHP = 3, attack = 4, defense = 2, speed = 2, url = "/content/images/minotaur.png", awareness = "dad___" };
            tileList[4][5].unit = new GeomancerUnit { name = "Minotaur", direction = 3, playerId = 0, hp = 3, maxHP = 3, attack = 4, defense = 2, speed = 2, url = "/content/images/minotaur.png", awareness = "dad___" };
            tileList[9][5].unit = new GeomancerUnit { name = "Hydra", direction = 3, playerId = 1, hp = 8, maxHP = 8, attack = 6, defense = 1, speed = 1, url = "/content/images/hydra.png", awareness = "aaaaaa" };
            tileList[7][7].unit = new GeomancerUnit { name = "Raider", direction = 5, playerId = 1, hp = 3, maxHP = 3, attack = 5, defense = 0, speed = 4, url = "/content/images/raider.png" ,awareness = "_a____"};

            player1Context.hand.Add(new GeomancerCard { name = "Summon Minotaur", description = "Summons a fearsome minotaur to the battlefield.", type = "Summon", cost = 3, castUnit = new GeomancerUnit { name = "Minotaur", direction = 3, playerId = 0, hp = 3, maxHP = 3, attack = 4, defense = 2, speed = 2, url = "/content/images/minotaur.png", awareness = "dad___" }, url = "/content/images/minotaur.png" });
            player1Context.hand.Add(new GeomancerCard { name = "Summon Hydra", description = "Summons a powerful multiheaded serpant to the battlefield.", type = "Summon", cost = 3, castUnit = new GeomancerUnit { name = "Hydra", direction = 3, playerId = 1, hp = 8, maxHP = 8, attack = 6, defense = 1, speed = 1, url = "/content/images/hydra.png", awareness = "aaaaaa" }, url = "/content/images/hydra.png" });
            player1Context.hand.Add(new GeomancerCard { name = "Lightning Bolt", description = "Blast a unit with a burst of lightning.", type = "Spell", cost = 2, url = "/content/images/lightningbolt.png" });
            player1Context.hand.Add(new GeomancerCard { name = "Mana Crystal", description = "Conjures a mana crystal from the ground.", type = "Crystal", cost = 1, url = "/content/images/manacrystal.png", castCrystal = new GeomancerCrystal { name = "ManaCrystal", mana = 1, playerId = 0, url = "/content/images/manacrystal.png" } });
            player1Context.hand.Add(new GeomancerCard { name = "Mana Crystal", description = "Conjures a mana crystal from the ground.", type = "Crystal", cost = 1, url = "/content/images/manacrystal.png", castCrystal = new GeomancerCrystal { name = "ManaCrystal", mana = 1, playerId = 0, url = "/content/images/manacrystal.png" } });
            player1Context.handCount = 5;

            player2Context.hand.Add(new GeomancerCard { name = "Mana Crystal", description = "Conjures a mana crystal from the ground.", type = "Crystal", cost = 1, url = "/content/images/manacrystal.png", castCrystal = new GeomancerCrystal { name = "ManaCrystal", mana = 1, playerId = 0, url = "/content/images/manacrystal.png" } });
            player2Context.hand.Add(new GeomancerCard { name = "Mana Crystal", description = "Conjures a mana crystal from the ground.", type = "Crystal", cost = 1, url = "/content/images/manacrystal.png", castCrystal = new GeomancerCrystal { name = "ManaCrystal", mana = 1, playerId = 0, url = "/content/images/manacrystal.png" } });
            player2Context.handCount = 2;

            playerContexts.Add(player1Context);
            playerContexts.Add(player2Context);
            activePlayerIndex = 0;
        }

        public void VerifyValidSpellLocation(int castA, int castB, int playerId)
        {
            bool atLeastOneFriendlyCrystalInRange = false;
            for (int a = 0; a < this.tileList.Count(); a++)
            {
                for (int b = 0; b < this.tileList[a].Count(); b++)
                {
                    if (this.tileList[a][b] != null)
                    {
                        bool containsFriendlyCrystal = false;
                        int crystalRange = 2;

                        if (this.tileList[a][b].crystal != null)
                        {
                            if (this.tileList[a][b].crystal.playerId == playerId)
                                containsFriendlyCrystal = true;
                            crystalRange = this.tileList[a][b].crystal.range;
                        }
                        if (this.tileList[a][b].unit != null && this.tileList[a][b].unit.name == "HomeCrystal")
                        {
                            if (this.tileList[a][b].unit.playerId == playerId)
                                containsFriendlyCrystal = true;
                        }
                        if (containsFriendlyCrystal && HexHelpers.Distance(castA, castB, a, b, 18) <= crystalRange)
                        {
                            atLeastOneFriendlyCrystalInRange = true;
                        }
                    }
                }
            }
            if (atLeastOneFriendlyCrystalInRange == false)
            {
                throw new Exception("ERROR: No friendly crystal in range");
            }
        }

        public void VerifyValidSummonLocation(int castA, int castB, int playerId)
        {
            if (this.tileList[castA][castB].unit != null || (this.tileList[castA][castB].crystal != null && this.tileList[castA][castB].crystal.playerId != playerId))
            {
                throw new Exception("ERROR: Tile already occupied");
            }
            bool atLeastOneFriendlyCrystalInRange = false;
            for (int a = 0; a < this.tileList.Count(); a++)
            {
                for (int b = 0; b < this.tileList[a].Count(); b++)
                {
                    if (this.tileList[a][b] != null)
                    {
                        bool containsFriendlyCrystal = false;
                        int crystalRange = 2;

                        if (this.tileList[a][b].crystal != null)
                        {
                            if (this.tileList[a][b].crystal.playerId == playerId)
                                containsFriendlyCrystal = true;
                            crystalRange = this.tileList[a][b].crystal.range;
                        }
                        if (this.tileList[a][b].unit != null && this.tileList[a][b].unit.name == "HomeCrystal")
                        {
                            if (this.tileList[a][b].unit.playerId == playerId)
                                containsFriendlyCrystal = true;
                        }
                        if (containsFriendlyCrystal && HexHelpers.Distance(castA, castB, a, b, 18) <= crystalRange)
                        {
                            atLeastOneFriendlyCrystalInRange = true;
                        }
                    }
                }
            }
            if (atLeastOneFriendlyCrystalInRange == false)
            {
                throw new Exception("ERROR: No friendly crystal in range");
            }

        }

        public void VerifyValidCrystalLocation(int castA, int castB, int playerId)
        {
            bool atLeastOneFriendlyCrystalInRange = false;
            bool noCrystalsAdjacent = true;
            for (int a = 0; a < this.tileList.Count(); a++)
            {
                for (int b = 0; b < this.tileList[a].Count(); b++)
                {
                    if (this.tileList[a][b] != null)
                    {
                        bool containsCrystal = false;
                        bool containsFriendlyCrystal = false;
                        int crystalRange = 2;

                        if (this.tileList[a][b].crystal != null)
                        {
                            containsCrystal = true;
                            if (this.tileList[a][b].crystal.playerId == playerId)
                                containsFriendlyCrystal = true;
                            crystalRange = this.tileList[a][b].crystal.range;
                        }
                        if(this.tileList[a][b].unit != null && this.tileList[a][b].unit.name == "HomeCrystal")
                        {
                            containsCrystal = true;
                            if (this.tileList[a][b].unit.playerId == playerId)
                                containsFriendlyCrystal = true;
                        }


                        if (containsCrystal)
                        {
                            if (HexHelpers.Distance(a, b, castA, castB, 18) < 2)
                            {
                                noCrystalsAdjacent = false;                                
                            }
                            if (containsFriendlyCrystal && HexHelpers.Distance(a, b, castA, castB, 18) <= crystalRange)
                            {
                                atLeastOneFriendlyCrystalInRange = true;
                            }
                        }
                    }
                }
            }
            if (noCrystalsAdjacent == false)
                throw new Exception("ERROR: Adjacent crystal.");
            if (atLeastOneFriendlyCrystalInRange == false)
                throw new Exception("ERROR: No friendly crystal in range.");
        }

        public List<List<string>> GenerateOverlay()
        {
            List<List<string>> overlay = new List<List<string>>();
            for (int a = 0; a < 14; a++)
            {
                overlay.Add(new List<string>());
                for (int b = 0; b < 11; b++)
                {
                    int c = 18 - a - b;
                    if (c < 14 && c >= 0)
                    {
                        overlay[a].Add("");
                    }
                    else
                        overlay[a].Add(null);
                }
            }
            return overlay;
        }

        public int[] AwarenessMap(int a, int b, int dir) {
            dir = dir % 6;
            var tileCoords = new int[2];
            tileCoords[0] = a;
            tileCoords[1] = b;
            if (dir == 3) {
                tileCoords[0] += 1;
            }
            if (dir == 4) {
                tileCoords[1] += 1;
            }
            if (dir == 5) {
                tileCoords[0] -= 1;
                tileCoords[1] += 1;
            }
            if (dir == 0) {
                tileCoords[0] -= 1;
            }
            if (dir == 1) {
                tileCoords[1] -= 1;
            }
            if (dir == 2) {
                tileCoords[1] -= 1;
                tileCoords[0] += 1;
            }
            return tileCoords;
        }

        public void MoveMark(List<List<string>> overlay, List<List<string>> enemyOverlay, int a, int b, int startA, int startB, int range, int playerId) {
            if (a < 0 || a >= 14 || b < 0 || b >= 11 || overlay[a][b]==null)
                return;

            if (enemyOverlay[a][b] == "R") {
                overlay[a][b] = "R";
                return;
            }
            overlay[a][b] = "B";
            if (!(a==startA && b==startB) && enemyOverlay[a][b] == "Y") {
                return;
            }
            if (range > 0) {
                range--;
                MoveMark(overlay, enemyOverlay, a + 1, b, startA, startB, range, playerId);
                MoveMark(overlay, enemyOverlay, a - 1, b, startA, startB, range, playerId);
                MoveMark(overlay, enemyOverlay, a, b + 1, startA, startB, range, playerId);
                MoveMark(overlay, enemyOverlay, a, b - 1, startA, startB, range, playerId);
                MoveMark(overlay, enemyOverlay, a + 1, b - 1, startA, startB, range, playerId);
                MoveMark(overlay, enemyOverlay, a - 1, b + 1, startA, startB, range, playerId);
            }
        }

        public bool IsMoveValid(GeomancerUnit unit, int srcA, int srcB, int destA, int destB)
        {
            //Generate enemy profile overlay
            List<List<string>> enemyOverlay = GenerateOverlay();
            for (int a = 0; a < 14; a++)
            {
                for (int b = 0; b < 11; b++)
                {
                    if (enemyOverlay[a][b] != null)
                    {
                        if (this.tileList[a][b].unit != null && this.tileList[a][b].unit.playerId != unit.playerId)
                        {
                            enemyOverlay[a][b] = "R";
                            for(int i = 0; i < this.tileList[a][b].unit.awareness.Length; i++)
                            {
                                if (this.tileList[a][b].unit.awareness[i] == 'd' || this.tileList[a][b].unit.awareness[i] == 'a')
                                {
                                    int[] awarenessCoords = AwarenessMap(a, b, (this.tileList[a][b].unit.direction + i) % 6);
                                    if(enemyOverlay[awarenessCoords[0]][awarenessCoords[1]] != "R")
                                        enemyOverlay[awarenessCoords[0]][awarenessCoords[1]] = "Y";
                                }
                            }
                        }
                    }
                }
            }
            //Generate move overlay
            List<List<string>> moveOverlay = GenerateOverlay();
            MoveMark(moveOverlay, enemyOverlay, srcA, srcB, srcA, srcB, unit.speed, unit.playerId);
            for (int a = 0; a < 14; a++)
            {
                for (int b = 0; b < 11; b++)
                {
                    if (this.tileList[a][b] != null && this.tileList[a][b].unit != null && !(a == srcA && b == srcB))
                        moveOverlay[a][b] = "R";
                }
            }
            return moveOverlay[destA][destB]=="B";
        }

        public void Update(GeomancerState inputState)
        {
            if (inputState.sourcePlayerId != this.playerContexts[activePlayerIndex].playerId || this.gameOver == true)
                return;

            int totalMana = 1;
            int activePlayerId = playerContexts[activePlayerIndex].playerId;

            // First pass - calculate mana
            for (int a = 0; a < inputState.tileList.Count(); a++)
            {
                for (int b = 0; b < inputState.tileList[a].Count(); b++)
                {
                    GeomancerTile inputTile = inputState.tileList[a][b];
                    GeomancerTile masterTile = this.tileList[a][b];
                    if (inputTile != null)
                    {
                        if (inputTile.crystal != null && inputTile.crystal.used == true && inputTile.crystal.playerId == activePlayerId)
                        {
                            // Verify that master state contains same crystal tile
                            if (masterTile.crystal != null && masterTile.crystal.playerId == activePlayerId)
                            {
                                if (masterTile.crystal.used == true)
                                {
                                    throw new Exception("INVALID MOVE: CRYSTAL AT " + a + "," + b + " owned by player " + activePlayerId + " has already been used.");
                                }
                                masterTile.crystal.used = true;
                                totalMana += masterTile.crystal.mana;
                            }
                            else
                            {
                                throw new Exception("INVALID MOVE: CRYSTAL AT " + a + "," + b + " owned by player " + activePlayerId + " not found in master state.");
                            }
                        }
                    }
                }
            }
            // Second pass move units
            for (int a = 0; a < inputState.tileList.Count(); a++)
            {
                for (int b = 0; b < inputState.tileList[a].Count(); b++)
                {
                    GeomancerTile inputTile = inputState.tileList[a][b];
                    GeomancerTile masterTile = this.tileList[a][b];
                    if (inputTile != null)
                    {
                        if (inputTile.moveUnit != null)
                        {
                            if (this.tileList[inputTile.moveUnit.moveA][inputTile.moveUnit.moveB].unit.name != inputState.tileList[inputTile.moveUnit.moveA][inputTile.moveUnit.moveB].unit.name)
                            {
                                throw new Exception("INVALID MOVE: Unit not found in master state.");
                            }

                            if (false == IsMoveValid(this.tileList[inputTile.moveUnit.moveA][inputTile.moveUnit.moveB].unit, inputTile.moveUnit.moveA, inputTile.moveUnit.moveB, a, b))
                            {
                                throw new Exception("INVALID MOVE: Destination not accessible from source.");
                            }


                            masterTile.unit = inputTile.moveUnit;
                            if (inputState.tileList[inputTile.moveUnit.moveA][inputTile.moveUnit.moveB].unit.used == true && this.tileList[inputTile.moveUnit.moveA][inputTile.moveUnit.moveB] != masterTile)
                                this.tileList[inputTile.moveUnit.moveA][inputTile.moveUnit.moveB].unit = null;
                            masterTile.unit.used = false;
                            masterTile.moveUnit = null;
                        }

                    }
                }
            }
            // Pass 3: Cast spells
            for (int a = 0; a < inputState.tileList.Count(); a++)
            {
                for (int b = 0; b < inputState.tileList[a].Count(); b++)
                {
                    GeomancerTile inputTile = inputState.tileList[a][b];
                    GeomancerTile masterTile = this.tileList[a][b];
                    if (inputTile != null)
                    {
                        if (inputTile.spell != null)
                        {
                            GeomancerCard sourceCard = inputState.playerContexts[inputState.activePlayerIndex].hand[inputTile.spell.sourceCardIndex];
                            GeomancerCard masterSourceCard = this.playerContexts[this.activePlayerIndex].hand[inputTile.spell.sourceCardIndex];
                            if (masterSourceCard.used == false)
                            {
                                masterSourceCard.used = true;
                                if (totalMana < masterSourceCard.cost)
                                {
                                    throw new Exception("ERROR: Insufficient mana");
                                }
                                totalMana -= masterSourceCard.cost;
                            }
                            else
                            {
                                throw new Exception("ERROR: Card " + inputTile.spell.sourceCardIndex + " already used.");
                            }

                            if (sourceCard.type == "Summon")
                            {
                                VerifyValidSummonLocation(a, b, activePlayerId);
                                masterTile.unit = inputState.playerContexts[inputState.activePlayerIndex].hand[inputTile.spell.sourceCardIndex].castUnit;
                                masterTile.unit.hp = inputState.playerContexts[inputState.activePlayerIndex].hand[inputTile.spell.sourceCardIndex].castUnit.maxHP;
                                masterTile.unit.direction = inputTile.spell.direction;
                                masterTile.unit.playerId = inputTile.spell.playerId;
                            }
                            if (sourceCard.type == "Spell")
                            {
                                VerifyValidSpellLocation(a, b, activePlayerId);
                                if (sourceCard.name == "Lightning Bolt")
                                {
                                    if (masterTile.unit != null)
                                    {
                                        masterTile.unit.hp -= 3;
                                    }
                                }
                            }
                            if (sourceCard.type == "Crystal")
                            {
                                VerifyValidCrystalLocation(a, b, activePlayerId);
                                masterTile.crystal = inputState.playerContexts[inputState.activePlayerIndex].hand[inputTile.spell.sourceCardIndex].castCrystal;
                                masterTile.crystal.playerId = inputTile.spell.playerId;
                            }
                            masterTile.spell = null;
                        }
                    }
                }
            }
            // Pass 4 Combat
            for (int a = 0; a < this.tileList.Count(); a++)
            {
                for (int b = 0; b < this.tileList[a].Count(); b++)
                {
                    GeomancerTile masterTile = this.tileList[a][b];
                    if (masterTile != null)
                    {
                        if (masterTile.unit != null && masterTile.unit.attacking == true)
                        {
                            GeomancerUnit attacker = masterTile.unit;
                            for (int i = 0; i < 6; i++)
                            {
                                if (attacker.awareness[i] == 'a')
                                {
                                    int[] target = AwarenessMap(a, b, attacker.direction + i);
                                    if(this.tileList[target[0]] != null && this.tileList[target[0]][target[1]]!= null && this.tileList[target[0]][target[1]].unit != null)
                                    {
                                        GeomancerUnit defender = this.tileList[target[0]][target[1]].unit;
                                        int damage = attacker.attack - defender.defense;
                                        if (damage < 0) damage = 0;
                                        defender.hp -= damage;

                                        for (int j = 0; j < 6; j++)
                                        {
                                            if (defender.awareness[j] != '_')
                                            {
                                                int[] responseTarget = AwarenessMap(target[0], target[1], defender.direction + j);
                                                if (responseTarget[0] == a && responseTarget[1] == b)
                                                {
                                                    int retaliationDamage = defender.attack - attacker.defense;
                                                    if (retaliationDamage < 0) retaliationDamage = 0;
                                                    attacker.hp -= retaliationDamage;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // Pass 5 Reset State
            for (int a = 0; a < this.tileList.Count(); a++)
            {
                for (int b = 0; b < this.tileList[a].Count(); b++)
                {
                    GeomancerTile masterTile = this.tileList[a][b];
                    if (masterTile != null)
                    {
                        if (masterTile.crystal != null)
                            masterTile.crystal.used = false;
                        if (masterTile.unit != null)
                        {
                            if (masterTile.unit.hp <= 0)
                            {
                                if (masterTile.unit.name == "HomeCrystal")
                                {
                                    foreach (GeomancerPlayerContext context in playerContexts)
                                    {
                                        if (context.playerId == masterTile.unit.playerId)
                                        {
                                            context.defeated = true;
                                            this.gameOver = true;
                                        }
                                    }
                                }
                                     
                                masterTile.unit = null;
                            }
                            else
                            {
                                masterTile.unit.used = false;
                                masterTile.unit.attacking = false;
                            }
                        }
                    }

                }
            }
            

            for (int i = 0; i < this.playerContexts[activePlayerIndex].hand.Count(); i++)
            {
                this.playerContexts[activePlayerIndex].hand[i].used = false;
            }

            this.playerContexts[inputState.activePlayerIndex].hand = inputState.playerContexts[inputState.activePlayerIndex].hand.Where(c => c.used == false).ToList();
            Random r = new Random();
            this.DrawCard(r, this.playerContexts[this.activePlayerIndex]);

            this.activePlayerIndex++;
            this.activePlayerIndex %= this.playerContexts.Count;
        }

        public GeomancerState GetClientState(int playerId)
        {
            GeomancerState clientState = new GeomancerState();
            clientState.tileList = this.tileList;
            clientState.tableId = this.tableId;
            clientState.playerContexts = new List<GeomancerPlayerContext>();
            clientState.activePlayerIndex = this.activePlayerIndex;
            clientState.sourcePlayerId = playerId;
            foreach (GeomancerPlayerContext playerContext in this.playerContexts)
            {
                GeomancerPlayerContext clientContext = new GeomancerPlayerContext();
                clientContext.deck = null;
                clientContext.hand = null;
                clientContext.deckCount = playerContext.deckCount;
                clientContext.handCount = playerContext.handCount;
                clientContext.playerId = playerContext.playerId;
                if (playerId == playerContext.playerId)
                    clientContext.hand = playerContext.hand;
                clientState.playerContexts.Add(clientContext);
            }
            return clientState;
        }
    }
}