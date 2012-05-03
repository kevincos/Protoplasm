using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;

namespace DeckBuilder.Games
{
    public class MechtonicPlayerContext
    {
        public int playerId { get; set; }
        public string name { get; set; }

        public List<MechtonicCard> hand { get; set; }
        public int money { get; set; }
        public string color { get; set; }
        public bool strongSide { get; set; }

        public int totalLightMechs { get; set; }

        public int totalUnits { get; set; }
        public bool eliminated { get; set; }
    }

    public class MechtonicUpdate
    {
        public int playerId { get; set; }

        public int selectedCard { get; set; }
        public int selectA { get; set; }
        public int selectB { get; set; }

    }

    public class MechtonicPurchaseCard
    {
        public string name { get; set; }
        public string url { get; set; }
        public int cost { get; set; }
        public bool selectable { get; set; }
    }

    public class MechtonicCard
    {
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public bool selectable { get; set; }
    }

    public class MechtonicPiece
    {
        public string name { get; set; }
        public bool strongSide { get; set; }
        public int strength { get; set; }
        public int altStrength { get; set; }
        public string url { get; set; }
        public string altUrl { get; set; }
        public int ownerIndex { get; set; }

        public int hp { get; set; }
        public int moves { get; set; }

        public void Flip()
        {
            int tempStrength = strength;
            string tempUrl = url;

            url = altUrl;
            strength = altStrength;
            altStrength = tempStrength;
            altUrl = tempUrl;
            strongSide = !strongSide;
        }        
    }

    public class MechtonicTile
    {
        public string url { get; set; }
        public string type { get; set; }
        public MechtonicPiece unit { get; set; }
        public bool selectable { get; set; }
    }

    public class MechtonicState
    {
        public List<MechtonicPlayerContext> playerContexts { get; set; }
        public int sourcePlayerId { get; set; }
        public int activePlayerId { get; set; }
        public int activePlayerIndex { get; set; }
        public int tableId { get; set; }
        public bool gameOver { get; set; }
        public List<string> logs { get; set; }

        public int sideLength { get; set; }

        public List<List<MechtonicTile>> board { get; set; }
        public int detonatorA { get; set; }
        public int detonatorB { get; set; }
        public List<MechtonicCard> deck { get; set; }
        public List<MechtonicCard> discard { get; set; }
        public List<MechtonicPurchaseCard> purchaseCards {get; set;}

        public MechtonicCard playedCard { get; set; }
        public MechtonicPurchaseCard purchaseItem { get; set; }
        public int selectA { get; set; }
        public int selectB { get; set; }
        public int targetA { get; set; }
        public int targetB { get; set; }
        public int cardsRemaining { get; set; }

        public string state { get; set; }

        public static System.Runtime.Serialization.Json.DataContractJsonSerializer GetSerializer()
        {
            return new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(MechtonicState), new Type[] { typeof(MechtonicPlayerContext), typeof(MechtonicPiece), typeof(MechtonicTile), typeof(MechtonicUpdate), typeof(MechtonicCard), typeof(MechtonicPurchaseCard) });
        }

        public void InitializeState(List<Seat> seats)
        {
            tableId = seats[0].TableId;
            playerContexts = new List<MechtonicPlayerContext>();
            foreach (Seat seat in seats)
            {
                MechtonicPlayerContext playerContext = new MechtonicPlayerContext();
                playerContext.playerId = seat.PlayerId;
                playerContext.name = seat.Player.Name;
                playerContext.hand = new List<MechtonicCard>();
                playerContext.money = 0;
                playerContext.eliminated = false;
                playerContext.totalLightMechs = 0;
                playerContexts.Add(playerContext);
                
            }
            logs = new List<string>();
            playerContexts[0].color = "Red";
            playerContexts[1].color = "Blue";
            if (playerContexts.Count > 2) playerContexts[2].color = "Yellow";
            if (playerContexts.Count > 3) playerContexts[3].color = "Green";
            activePlayerIndex = 0;
            activePlayerId = playerContexts[activePlayerIndex].playerId;

            Random r = new Random();

            #region marketSetup
            purchaseCards = new List<MechtonicPurchaseCard>();
            purchaseCards.Add(new MechtonicPurchaseCard { name = "Light Mech", cost = 1, url = "/content/images/mechtonic/buy_lightmech.png" });
            purchaseCards.Add(new MechtonicPurchaseCard { name = "Scout", cost = 2, url = "/content/images/mechtonic/buy_scout.png" });
            purchaseCards.Add(new MechtonicPurchaseCard { name = "Miner", cost = 3, url = "/content/images/mechtonic/buy_miner.png" });
            purchaseCards.Add(new MechtonicPurchaseCard { name = "Heavy Mech", cost = 6, url = "/content/images/mechtonic/buy_heavymech.png" });
            purchaseCards.Add(new MechtonicPurchaseCard { name = "Launcher", cost = 6, url = "/content/images/mechtonic/buy_launcher.png" });
            purchaseCards.Add(new MechtonicPurchaseCard { name = "Hover Tank", cost = 7, url = "/content/images/mechtonic/buy_hover.png" });
            purchaseCards.Add(new MechtonicPurchaseCard { name = "Colossus", cost = 11, url = "/content/images/mechtonic/buy_colossus.png" });
            purchaseCards.Add(new MechtonicPurchaseCard { name = "Card", cost = 4, url = "/content/images/mechtonic/buy_card.png" });
            purchaseCards.Add(new MechtonicPurchaseCard { name = "Skip", cost = 0, url = "/content/images/mechtonic/buy_skip.png" });
            #endregion
            
            #region boardSetup

            sideLength = 6;
            
            detonatorA = 3*(sideLength-1)-1;
            
            int detonatorC = sideLength;
            detonatorB = 6 * (sideLength - 1) - detonatorA - detonatorC;
            
            board = new List<List<MechtonicTile>>();
            for (int a = 0; a <= 3*(sideLength - 1); a++)
            {
                board.Add(new List<MechtonicTile>());
                for (int b = 0; b <= 3*(sideLength - 1); b++)
                {
                    int c = 6*(sideLength-1) - a - b;
                    if (c >= sideLength-1 && c <= 3*(sideLength - 1))
                    {
                        board[a].Add(new MechtonicTile());
                        if (a < sideLength || b < sideLength || c < sideLength || a >= 3 * (sideLength - 1) || b >= 3 * (sideLength - 1) || c >= 3 * (sideLength - 1))                        
                        {
                            board[a][b].type = "Core";
                            board[a][b].url = "/content/images/mechtonic/tile_core.png";
                        }
                        else if (r.Next(15) < 2)
                        {
                            board[a][b].type = "Mountain";
                            if(a == detonatorA && b == detonatorB)
                                board[a][b].url = "/content/images/mechtonic/tile_mountain_detonate.png";
                            else
                                board[a][b].url = "/content/images/mechtonic/tile_mountain.png";
                        }
                        else
                        {
                            board[a][b].type = "Normal";
                            if (a == detonatorA && b == detonatorB)
                                board[a][b].url = "/content/images/mechtonic/tile_normal_detonate.png";
                            else
                                board[a][b].url = "/content/images/mechtonic/tile_normal.png";
                        }
                    }
                    else
                    {
                        board[a].Add(null);
                    }
                }
            }
            #endregion

            #region deckSetup
            MechtonicCard coreSpasm = new MechtonicCard{name = "Core Spasm", description = "Destroy next 4 tiles.", url = "/content/images/mechtonic/card_corespasm.png"};
            MechtonicCard nanoTransformation = new MechtonicCard{name = "Nanotransformation", description = "Transform unit into heavy mech.", url = "/content/images/mechtonic/card_nanotransform.png"};
            MechtonicCard overload = new MechtonicCard{name = "Overload", description = "Destroy friendly unit and all adjacent units.", url = "/content/images/mechtonic/card_overload.png"};
            MechtonicCard tactics = new MechtonicCard { name = "Tactics", description = "Play two cards this turn.", url = "/content/images/mechtonic/card_tactics.png" };
            MechtonicCard advance = new MechtonicCard { name = "Advance", description = "Units may move an extra space this turn.", url = "/content/images/mechtonic/card_advance.png" };
            MechtonicCard blackMarket = new MechtonicCard { name = "Black Market", description = "You may buy a unit immediately.", url = "/content/images/mechtonic/card_blackmarket.png" };

            deck = new List<MechtonicCard>();
            deck.Add(coreSpasm);
            deck.Add(coreSpasm);
            deck.Add(nanoTransformation);
            deck.Add(nanoTransformation);
            deck.Add(overload);
            deck.Add(overload);
            deck.Add(tactics);
            deck.Add(tactics);
            deck.Add(advance);
            deck.Add(advance);
            deck.Add(blackMarket);
            deck.Add(blackMarket);
            
            discard = new List<MechtonicCard>();

            foreach (MechtonicPlayerContext playerContext in playerContexts)
            {
                playerContext.hand.Add(DrawCard(r));
                playerContext.hand.Add(DrawCard(r));
            }
            #endregion

            

            state = "StartPlace";
            MarkEmpty();
        }

        public void AdvanceDetonator()
        {
            board[detonatorA][detonatorB].type = "Core";
            board[detonatorA][detonatorB].url = "/content/images/mechtonic/tile_core.png";
            board[detonatorA][detonatorB].unit = null;

            if (board[detonatorA - 1][detonatorB + 1].type != "Core" && board[detonatorA][detonatorB + 1].type == "Core")
            {
                detonatorA--;
                detonatorB++;
            }
            else if (board[detonatorA - 1][detonatorB].type != "Core" && board[detonatorA - 1][detonatorB + 1].type == "Core")
            {
                detonatorA--;
            }
            else if (board[detonatorA][detonatorB - 1].type != "Core" && board[detonatorA - 1][detonatorB].type == "Core")
            {
                detonatorB--;
            }
            else if (board[detonatorA + 1][detonatorB - 1].type != "Core" && board[detonatorA][detonatorB - 1].type == "Core")
            {
                detonatorA++;
                detonatorB--;
            }
            else if (board[detonatorA + 1][detonatorB].type != "Core" && board[detonatorA + 1][detonatorB - 1].type == "Core")
            {
                detonatorA++;
            }
            else if (board[detonatorA][detonatorB + 1].type != "Core" && board[detonatorA + 1][detonatorB].type == "Core")
            {
                detonatorB++;
            }


            if (board[detonatorA][detonatorB].type == "Normal")
                board[detonatorA][detonatorB].url = "/content/images/mechtonic/tile_normal_detonate.png";
            if (board[detonatorA][detonatorB].type == "Mountain")
                board[detonatorA][detonatorB].url = "/content/images/mechtonic/tile_mountain_detonate.png";
        }

        public void AdvanceTurnOrder()
        {
            if (state != "StartPlace")
                playerContexts[activePlayerIndex].strongSide = !playerContexts[activePlayerIndex].strongSide;
            activePlayerIndex++;
            activePlayerIndex %= playerContexts.Count;
            activePlayerId = playerContexts[activePlayerIndex].playerId;

            if (state != "StartPlace")
            {
                playerContexts[activePlayerIndex].money += 5;
                // Advance detonator
                AdvanceDetonator();
            }

            // Refresh Units (moves + hp)
            for (int a = 0; a <= 3 * (sideLength - 1); a++)
            {
                board.Add(new List<MechtonicTile>());
                for (int b = 0; b <= 3 * (sideLength - 1); b++)
                {
                    int c = 6 * (sideLength - 1) - a - b;
                    if (c >= sideLength - 1 && c <= 3 * (sideLength - 1))
                    {
                        if (board[a][b].unit != null)
                        {
                            board[a][b].unit.hp = board[a][b].unit.strength;
                            board[a][b].unit.moves = 1;
                            if (board[a][b].unit.name == "Scout" || board[a][b].unit.name == "Hover Tank")
                                board[a][b].unit.moves = 3;
                        }
                    }
                }
            }
            cardsRemaining = 1;
        }

        public MechtonicCard DrawCard(Random r)
        {
            if (deck.Count == 0)
            {
                deck = discard;
                discard = new List<MechtonicCard>();
            }
            if (deck.Count == 0)
                return null;
            int randomIndex = r.Next(0, deck.Count);
            MechtonicCard card = deck.ElementAt(randomIndex);
            deck.RemoveAt(randomIndex);
            return card;
        }

        public void ClearSelection()
        {
            for (int a = 0; a <= 3 * (sideLength - 1); a++)
            {
                board.Add(new List<MechtonicTile>());
                for (int b = 0; b <= 3 * (sideLength - 1); b++)
                {
                    int c = 6 * (sideLength - 1) - a - b;
                    if (c >= sideLength - 1 && c <= 3 * (sideLength - 1))
                    {
                        board[a][b].selectable = false;
                    }
                }
            }
            foreach (MechtonicPurchaseCard purchase in purchaseCards)
            {
                if (playerContexts[activePlayerIndex].money >= purchase.cost)
                {
                    purchase.selectable = false;
                }
            }
            foreach (MechtonicPlayerContext context in playerContexts)
            {
                foreach (MechtonicCard card in context.hand)
                {
                    card.selectable = false;
                }
            }
        }

        public void MarkEmpty()
        {
            ClearSelection();

            for (int a = 0; a <= 3 * (sideLength - 1); a++)
            {
                board.Add(new List<MechtonicTile>());
                for (int b = 0; b <= 3 * (sideLength - 1); b++)
                {
                    int c = 6 * (sideLength - 1) - a - b;
                    if (c >= sideLength - 1 && c <= 3 * (sideLength - 1))
                    {
                        if (board[a][b].type == "Normal" && board[a][b].unit == null)
                        {
                            board[a][b].selectable = true;
                        }
                    }
                }
            }
        }

        public bool HasAdjacentFriendlyUnit(int a, int b, int index)
        {
            if (board[a][b - 1].unit != null && board[a][b - 1].unit.ownerIndex == index) return true;
            if (board[a][b + 1].unit != null && board[a][b + 1].unit.ownerIndex == index) return true;
            if (board[a-1][b].unit != null && board[a-1][b].unit.ownerIndex == index) return true;
            if (board[a+1][b].unit != null && board[a+1][b].unit.ownerIndex == index) return true;
            if (board[a+1][b - 1].unit != null && board[a+1][b - 1].unit.ownerIndex == index) return true;
            if (board[a-1][b + 1].unit != null && board[a-1][b + 1].unit.ownerIndex == index) return true;
            return false;
        }

        public void MarkSummon()
        {
            ClearSelection();

            for (int a = 0; a <= 3 * (sideLength - 1); a++)
            {
                board.Add(new List<MechtonicTile>());
                for (int b = 0; b <= 3 * (sideLength - 1); b++)
                {
                    int c = 6 * (sideLength - 1) - a - b;
                    if (c >= sideLength - 1 && c <= 3 * (sideLength - 1))
                    {
                        if (board[a][b].type == "Normal" && board[a][b].unit == null && HasAdjacentFriendlyUnit(a,b,activePlayerIndex))
                        {                            
                            board[a][b].selectable = true;
                        }
                    }
                }
            }
        }

        public int CountMovableUnits()
        {
            int unitCount = 0;
            for (int a = 0; a <= 3 * (sideLength - 1); a++)
            {
                board.Add(new List<MechtonicTile>());
                for (int b = 0; b <= 3 * (sideLength - 1); b++)
                {
                    int c = 6 * (sideLength - 1) - a - b;
                    if (c >= sideLength - 1 && c <= 3 * (sideLength - 1))
                    {
                        if (board[a][b].unit != null && board[a][b].unit.ownerIndex == activePlayerIndex && board[a][b].unit.strongSide == playerContexts[activePlayerIndex].strongSide)
                        {
                            unitCount++;
                        }
                    }
                }
            }
            return unitCount;
        }

        public void MarkTarget()
        {
            ClearSelection();
            board[selectA][selectB].selectable = true;
            for (int a = 0; a <= 3 * (sideLength - 1); a++)
            {
                board.Add(new List<MechtonicTile>());
                for (int b = 0; b <= 3 * (sideLength - 1); b++)
                {
                    int c = 6 * (sideLength - 1) - a - b;
                    if (c >= sideLength - 1 && c <= 3 * (sideLength - 1))
                    {
                        int selectC = 6 * (sideLength - 1) - selectA - selectB;
                        int distance = (Math.Abs(selectC - c) + Math.Abs(selectA - a) + Math.Abs(selectB - b))/2;
                        if (distance <= 1 && board[a][b].type == "Normal" && (board[a][b].unit == null || board[a][b].unit.ownerIndex != activePlayerIndex))
                        {
                            board[a][b].selectable = true;
                        }
                        if (distance <= 3 && board[selectA][selectB].unit.name == "Launcher" && board[a][b].unit != null && board[a][b].unit.ownerIndex != activePlayerIndex)
                        {
                            board[a][b].selectable = true;
                        }
                    }
                }
            }
        }

        public void MarkSelect()
        {
            ClearSelection();
            for (int a = 0; a <= 3 * (sideLength - 1); a++)
            {
                board.Add(new List<MechtonicTile>());
                for (int b = 0; b <= 3 * (sideLength - 1); b++)
                {
                    int c = 6 * (sideLength - 1) - a - b;
                    if (c >= sideLength - 1 && c <= 3 * (sideLength - 1))
                    {
                        if (board[a][b].unit != null && board[a][b].unit.ownerIndex == activePlayerIndex && board[a][b].unit.strongSide == playerContexts[activePlayerIndex].strongSide)
                        {
                            board[a][b].selectable = true;
                        }
                    }
                }
            }
            if (cardsRemaining > 0)
            {                
                foreach (MechtonicCard card in playerContexts[activePlayerIndex].hand)
                {
                    card.selectable = true;
                }
            }
        }

        public void MarkUnits()
        {
            ClearSelection();
            for (int a = 0; a <= 3 * (sideLength - 1); a++)
            {
                board.Add(new List<MechtonicTile>());
                for (int b = 0; b <= 3 * (sideLength - 1); b++)
                {
                    int c = 6 * (sideLength - 1) - a - b;
                    if (c >= sideLength - 1 && c <= 3 * (sideLength - 1))
                    {
                        if (board[a][b].unit != null)
                        {
                            board[a][b].selectable = true;
                        }
                    }
                }
            }
        }

        public void MarkFriendlyUnits()
        {
            ClearSelection();
            for (int a = 0; a <= 3 * (sideLength - 1); a++)
            {
                board.Add(new List<MechtonicTile>());
                for (int b = 0; b <= 3 * (sideLength - 1); b++)
                {
                    int c = 6 * (sideLength - 1) - a - b;
                    if (c >= sideLength - 1 && c <= 3 * (sideLength - 1))
                    {
                        if (board[a][b].unit != null && board[a][b].unit.ownerIndex == activePlayerIndex)
                        {
                            board[a][b].selectable = true;
                        }
                    }
                }
            }
        }

        public void MarkBuys()
        {
            ClearSelection();
            foreach (MechtonicPurchaseCard purchase in purchaseCards)
            {
                if (playerContexts[activePlayerIndex].money >= purchase.cost)
                {
                    purchase.selectable = true;
                }
            }
        }

        public void PlayCard()
        {
            cardsRemaining--;

            if (playedCard.name == "Overload")
            {
                state = "CardTarget";
                MarkFriendlyUnits();
            }
            else if (playedCard.name == "Tactics")
            {
                cardsRemaining += 2;
                state = "Select";
                MarkSelect();
            }
            else if (playedCard.name == "Advance")
            {
                for (int a = 0; a <= 3 * (sideLength - 1); a++)
                {
                    board.Add(new List<MechtonicTile>());
                    for (int b = 0; b <= 3 * (sideLength - 1); b++)
                    {
                        int c = 6 * (sideLength - 1) - a - b;
                        if (c >= sideLength - 1 && c <= 3 * (sideLength - 1))
                        {
                            if (board[a][b].unit != null && board[a][b].unit.ownerIndex == activePlayerIndex)
                            {
                                board[a][b].unit.moves++;
                            }
                        }
                    }
                }
                state = "Select";
                MarkSelect();
            }
            else if (playedCard.name == "Core Spasm")
            {
                for (int i = 0; i < 4; i++)
                {
                    AdvanceDetonator();
                }
                state = "Select";
                MarkSelect();
            }
            else if (playedCard.name == "Nanotransformation")
            {
                state = "CardTarget";
                MarkUnits();
            }
            else if (playedCard.name == "Black Market")
            {
                state = "SpecialBuy";
                MarkBuys();
            }            
        }


        public void PerformCardAction()
        {
            if (playedCard.name == "Overload")
            {
                board[targetA][targetB].unit = null;
                board[targetA - 1][targetB].unit = null;
                board[targetA+1][targetB].unit = null;
                board[targetA][targetB-1].unit = null;
                board[targetA][targetB+1].unit = null;
                board[targetA-1][targetB+1].unit = null;
                board[targetA+1][targetB-1].unit = null;
                state = "Select";
                MarkSelect();
            }
            else if (playedCard.name == "Nanotransform")
            {
                MechtonicPiece transformUnit = board[targetA][targetB].unit;
                transformUnit.name = "Heavy Mech";
                if (transformUnit.strongSide)
                {
                    transformUnit.strength = 6;
                    transformUnit.altStrength = 5;
                    transformUnit.url = "/content/images/mechtonic/heavymech_strong.png";
                    transformUnit.altUrl = "/content/images/mechtonic/heavymech_weak.png";
                }
                else
                {
                    transformUnit.strength = 5;
                    transformUnit.altStrength = 6;
                    transformUnit.url = "/content/images/mechtonic/heavymech_weak.png";
                    transformUnit.altUrl = "/content/images/mechtonic/heavymech_strong.png";
                }
                state = "Select";
                MarkSelect();
            }
        }


        public void PerformUnitAction()
        {            
            MechtonicPiece unit = board[selectA][selectB].unit;                        
            
            int selectC = 6 * (sideLength - 1) - selectA - selectB;
            int targetC = 6 * (sideLength - 1) - targetA - targetB;
            int distance = (Math.Abs(selectC - targetC) + Math.Abs(selectA - targetA) + Math.Abs(selectB - targetB))/2;

            if (unit.name == "Miner" && selectA == targetA && selectB == targetB)
            {
                playerContexts[unit.ownerIndex].money++;
            }
            else if (unit.name == "Launcher" && distance > 1)
            {
                board[targetA][targetB].unit.hp -= unit.strength;
                if (board[targetA][targetB].unit.hp <= 0)
                {
                    board[targetA][targetB].unit = null;
                    playerContexts[unit.ownerIndex].money++;
                }
            }
            else if ((targetA != selectA || targetB != selectB) && board[targetA][targetB].unit != null)
            {
                board[selectA][selectB].unit = null;
                MechtonicPiece defender = board[targetA][targetB].unit;
                if (unit.strength > defender.hp)
                {
                    board[targetA][targetB].unit = unit;
                    unit.hp -= defender.strength;
                    if (unit.hp <= 0)
                    {
                        board[targetA][targetB].unit = null;
                    }
                    else
                    {
                        selectA = targetA;
                        selectB = targetB;
                    }
                }
                else
                {
                    defender.hp -= unit.strength;
                    if (defender.hp <= 0)
                        board[targetA][targetB].unit = null;
                }
            }
            else
            {
                board[selectA][selectB].unit = null;
                board[targetA][targetB].unit = unit;
                selectA = targetA;
                selectB = targetB;
            }

            unit.moves--;
            if (unit.moves == 0)
            {
                unit.Flip();
            }

        }

        public void PlaceUnit(int a, int b, string name, int ownerIndex)
        {
            MechtonicPiece newUnit = new MechtonicPiece{name = name, ownerIndex = ownerIndex};
            if (name == "Light Mech")
            {
                newUnit.url = "/content/images/mechtonic/lightmech_weak.png";
                newUnit.altUrl = "/content/images/mechtonic/lightmech_strong.png";
                newUnit.strength = 1;
                newUnit.altStrength = 2;
                playerContexts[ownerIndex].totalLightMechs++;
            }
            if (name == "Miner")
            {
                newUnit.url = "/content/images/mechtonic/miner.png";
                newUnit.altUrl = "/content/images/mechtonic/miner.png";
                newUnit.strength = 0;
                newUnit.altStrength = 0;                
            }
            if (name == "Scout")
            {
                newUnit.url = "/content/images/mechtonic/scout_weak.png";
                newUnit.altUrl = "/content/images/mechtonic/scout_strong.png";
                newUnit.strength = 2;
                newUnit.altStrength = 3;
            }
            if (name == "Heavy Mech")
            {
                newUnit.url = "/content/images/mechtonic/heavymech_weak.png";
                newUnit.altUrl = "/content/images/mechtonic/heavymech_strong.png";
                newUnit.strength = 5;
                newUnit.altStrength = 6;
            }
            if (name == "Colossus")
            {
                newUnit.url = "/content/images/mechtonic/colossus_weak.png";
                newUnit.altUrl = "/content/images/mechtonic/colossus_strong.png";
                newUnit.strength = 10;
                newUnit.altStrength = 11;
            }
            if (name == "Hover Tank")
            {
                newUnit.url = "/content/images/mechtonic/hover_weak.png";
                newUnit.altUrl = "/content/images/mechtonic/hover_strong.png";
                newUnit.strength = 6;
                newUnit.altStrength = 7;
            }
            if (name == "Launcher")
            {
                newUnit.url = "/content/images/mechtonic/launcher_weak.png";
                newUnit.altUrl = "/content/images/mechtonic/launcher_strong.png";
                newUnit.strength = 4;
                newUnit.altStrength = 5;
            }
            if (state != "StartPlace")
            {
                if (newUnit.strongSide == playerContexts[ownerIndex].strongSide)
                    newUnit.Flip();
            }
            board[a][b].unit = newUnit;
        }

        public void Update(MechtonicUpdate update)
        {
            if (update.playerId != activePlayerId)
            {
                return;
            }
            if (state == "StartPlace")
            {
                PlaceUnit(update.selectA, update.selectB, "Light Mech", activePlayerIndex);
                AdvanceTurnOrder();

                if (playerContexts[activePlayerIndex].totalLightMechs == 2)
                {
                    state = "Select";
                    playerContexts[activePlayerIndex].money += 5;
                    MarkSelect();
                }
                else
                {                    
                    MarkEmpty();
                }
            }
            else if (state == "Select")
            {
                if (update.selectedCard != -1)
                {
                    playedCard = playerContexts[activePlayerIndex].hand[update.selectedCard];
                    playerContexts[activePlayerIndex].hand.RemoveAt(update.selectedCard);

                    PlayCard();
                }
                else
                {
                    selectA = update.selectA;
                    selectB = update.selectB;
                    MarkTarget();
                    state = "Target";
                }
            }
            else if (state == "CardTarget")
            {
                targetA = update.selectA;
                targetB = update.selectB;
                PerformCardAction();

                if (CountMovableUnits() == 0)
                {
                    state = "Buy";
                    MarkBuys();
                }
                else
                {
                    state = "Select";
                    MarkSelect();
                }
            }
            else if (state == "Target")
            {
                targetA = update.selectA;
                targetB = update.selectB;
                PerformUnitAction();

                if (board[selectA][selectB].unit != null && board[selectA][selectB].unit.moves > 0)
                {
                    MarkTarget();
                }
                else if (CountMovableUnits() == 0)
                {
                    state = "Buy";
                    MarkBuys();
                }
                else
                {
                    state = "Select";
                    MarkSelect();
                }
            }
            else if (state == "Buy")
            {
                purchaseItem = purchaseCards[update.selectedCard];
                if (purchaseItem.name == "Skip")
                {
                    AdvanceTurnOrder();
                    state = "Select";
                    MarkSelect();
                }
                else if (purchaseItem.name == "Card")
                {
                    playerContexts[activePlayerIndex].hand.Add(DrawCard(new Random()));
                    AdvanceTurnOrder();
                    state = "Select";
                    MarkSelect();
                }
                else
                {
                    state = "Summon";
                    MarkSummon();
                }
            }
            else if (state == "Summon")
            {
                PlaceUnit(update.selectA, update.selectB, purchaseItem.name, activePlayerIndex);
                AdvanceTurnOrder();
                state = "Select";
                MarkSelect();
            }
            else if (state == "SpecialBuy")
            {
                purchaseItem = purchaseCards[update.selectedCard];
                if (purchaseItem.name == "Skip")
                {
                    state = "Select";
                    MarkSelect();
                }
                else if (purchaseItem.name == "Card")
                {
                    playerContexts[activePlayerIndex].hand.Add(DrawCard(new Random()));
                    state = "Select";
                    MarkSelect();
                }
                else
                {
                    state = "SpecialSummon";
                    MarkSummon();
                }
            }
            else if (state == "SpecialSummon")
            {
                PlaceUnit(update.selectA, update.selectB, purchaseItem.name, activePlayerIndex);
                state = "Select";
                MarkSelect();
            }
            
        }

        public MechtonicState GetClientState(int playerId)
        {
            this.sourcePlayerId = playerId;
            MechtonicState clientState = new MechtonicState();
            clientState.board = this.board;
            clientState.activePlayerId = this.activePlayerId;
            clientState.activePlayerIndex = this.activePlayerIndex;
            clientState.deck = null;
            clientState.discard = null;
            clientState.gameOver = this.gameOver;
            clientState.playedCard = this.playedCard;
            clientState.purchaseItem = this.purchaseItem;
            clientState.sideLength = this.sideLength;

            clientState.selectA = this.selectA;
            clientState.selectB = this.selectB;
            clientState.state = this.state;
            clientState.tableId = this.tableId;
            clientState.sourcePlayerId = playerId;
            clientState.targetA = this.targetA;
            clientState.targetB = this.targetB;
            clientState.purchaseCards = this.purchaseCards;
            clientState.playerContexts = new List<MechtonicPlayerContext>();
            clientState.logs = this.logs;
            foreach (MechtonicPlayerContext context in playerContexts)
            {
                MechtonicPlayerContext clientContext = new MechtonicPlayerContext();                
                clientContext.playerId = context.playerId;
                clientContext.name = context.name;
                clientContext.color = context.color;
                clientContext.strongSide = context.strongSide;
                clientContext.money = context.money;
                if (clientContext.playerId == playerId)
                {
                    clientContext.hand = context.hand;
                    
                }
                clientState.playerContexts.Add(clientContext);
            }
            return clientState;
        }
    }

}