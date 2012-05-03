using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeckBuilder.Models;

namespace DeckBuilder.Games
{
    public class ConvoyPlayerContext
    {
        public string side { get; set; }
        public int playerId { get; set; }
        public string name { get; set; }

        public List<ConvoyPiece> hand { get; set; }
        public List<ConvoyPiece> draftHand { get; set; }
        public int draftIndex { get; set; }
        public bool draftReady { get; set; }

        public int score { get; set; }
    }

    public class ConvoyUpdate
    {
        public int playerId { get; set; }
        public int selectedTileX { get; set; }
        public int selectedTileY { get; set; }
        public int selectedCardIndex { get; set; }

    }

    public class ConvoyPiece
    {
        public string name {get;set;}
        public string url {get;set;}
        public int hits {get;set;}
        public string targetType { get; set; }
        public string side { get; set; }
        public int direction { get; set; }
    }

    public class ConvoyTile
    {
        public string type {get;set;}
        public string url {get;set;}
        public ConvoyPiece unit { get; set; }
        public bool selectable { get; set; }        
    }
    
    public class ConvoyState
    {
        public List<ConvoyPlayerContext> playerContexts { get; set; }
        public int sourcePlayerId { get; set; }
        public int activePlayerId { get; set; }
        public int activePlayerIndex { get; set; }
        public int tableId { get; set; }
        public List<string> logs { get; set; }

        public bool gameOver { get; set; }
        public string state { get; set; }
        public int primaryPlayerId { get; set; }

        public List<List<ConvoyTile>> board { get; set; }
        public List<ConvoyPiece> deck { get; set; }
        public List<ConvoyPiece> discard { get; set; }

        
        public ConvoyPiece playedPiece { get; set; }
        public bool dudPiece { get; set; }
        public int selectX { get; set; }
        public int selectY { get; set; }
        public int targetX { get; set; }
        public int targetY { get; set; }


        public void InitializeState(List<Seat> seats)
        {
            tableId = seats[0].TableId;
            playerContexts = new List<ConvoyPlayerContext>();
            foreach (Seat seat in seats)
            {
                ConvoyPlayerContext playerContext = new ConvoyPlayerContext();
                playerContext.playerId = seat.PlayerId;
                playerContext.name = seat.Player.Name;
                playerContexts.Add(playerContext);
                playerContext.hand = new List<ConvoyPiece>();
                playerContext.draftHand = new List<ConvoyPiece>();
            }
            playerContexts[0].side = "Left";
            playerContexts[1].side = "Right";
            activePlayerIndex = 0;
            activePlayerId = playerContexts[activePlayerIndex].playerId;

            logs = new List<string>();
            logs.Add("Convoy Canyon Initialized at table " + tableId);
            logs.Add(playerContexts[0].name + " vs " + playerContexts[1].name);

            primaryPlayerId = activePlayerId;
            
            #region intialize_board
            ConvoyPiece rock = new ConvoyPiece { name = "Rock", url = "/content/images/canyon/unit_rock.png" };
            board = new List<List<ConvoyTile>>();
            for (int x = 0; x < 11; x++)
            {
                board.Add(new List<ConvoyTile>());
                for (int y = 0; y < 12; y++)
                {
                    if(x == 4)
                        board[x].Add(new ConvoyTile { type = "Canyon", url = "/content/images/canyon/tile_leftedge.png", unit = null });
                    else if(x == 6)
                        board[x].Add(new ConvoyTile { type = "Canyon", url = "/content/images/canyon/tile_rightedge.png", unit = null });
                    else if(x == 5)
                        board[x].Add(new ConvoyTile { type = "Canyon", url = "/content/images/canyon/tile_canyon.png", unit = null });
                    else if (x == 0)
                        board[x].Add(new ConvoyTile { type = "Canyon", url = "/content/images/canyon/tile_leftcliff.png", unit = null });
                    else if (x == 10)
                        board[x].Add(new ConvoyTile { type = "Canyon", url = "/content/images/canyon/tile_rightcliff.png", unit = null });
                    else if (x < 4)
                    {
                        if(y<=1)
                        {
                            board[x].Add(new ConvoyTile { type = "Start-Left", url = "/content/images/canyon/tile_bluesrc.png", unit = null });
                        }
                        else if (y >= 10)
                        {
                            board[x].Add(new ConvoyTile { type = "End-Left", url = "/content/images/canyon/tile_bluedest.png", unit = null });
                        }
                        else
                        {
                            board[x].Add(new ConvoyTile { type = "Left", url = "/content/images/canyon/tile_path.png", unit = null });
                        }
                    }
                    else if (x > 6)
                    {
                        if (y <= 1)
                        {
                            board[x].Add(new ConvoyTile { type = "End-Right", url = "/content/images/canyon/tile_reddest.png", unit = null });
                        }
                        else if (y >= 10)
                        {
                            board[x].Add(new ConvoyTile { type = "Start-Right", url = "/content/images/canyon/tile_redsrc.png", unit = null });
                        }
                        else
                        {
                            board[x].Add(new ConvoyTile { type = "Right", url = "/content/images/canyon/tile_path.png", unit = null });
                        }                        
                    }
                }
            }
            board[1][3].unit = rock;
            board[2][7].unit = rock;
            board[3][5].unit = rock;
            board[9][8].unit = rock;
            board[8][4].unit = rock;
            board[7][6].unit = rock;
            #endregion

            #region initialize_deck
            int move_one_count = 8;
            int move_two_count = 10;
            int move_three_count = 8;
            int fire_one_count = 4;
            int fire_all_count = 4;
            int movefire_one_count = 6;
            int movefire_two_count = 4;
            int armoredconvoy_one_count = 3;
            int armoredconvoy_three_count = 2;
            int artillery_count = 2;
            int convoy_one_count = 3;
            int convoy_three_count = 2;
            int tank_count = 4;
            int convoy_five_count = 1;
            int gunship_count = 2;
            int heavytank_count = 0;
            int landmines_count = 2;
            int missileLauncher_count = 2;
            int railgun_count = 2;

            deck = new List<ConvoyPiece>();
            discard = new List<ConvoyPiece>();
            ConvoyPiece move_one = new ConvoyPiece { name = "Move 1", url = "/content/images/canyon/move_one.png", targetType = "Unit-Tile" };
            for (int i = 0; i < move_one_count; i++) deck.Add(move_one);
            ConvoyPiece move_two = new ConvoyPiece { name = "Move 2", url = "/content/images/canyon/move_two.png", targetType = "Unit-Tile" };
            for (int i = 0; i < move_two_count; i++) deck.Add(move_two);
            ConvoyPiece move_three = new ConvoyPiece { name = "Move 3", url = "/content/images/canyon/move_three.png", targetType = "Unit-Tile" };
            for (int i = 0; i < move_three_count; i++) deck.Add(move_three);
            ConvoyPiece fire_one = new ConvoyPiece { name = "Fire 1", url = "/content/images/canyon/fire_one.png", targetType = "Unit" };
            for (int i = 0; i < fire_one_count; i++) deck.Add(fire_one);
            ConvoyPiece fire_all = new ConvoyPiece { name = "Fire All", url = "/content/images/canyon/fire_all.png", targetType = "None" };
            for (int i = 0; i < fire_all_count; i++) deck.Add(fire_all);
            ConvoyPiece movefire_one = new ConvoyPiece { name = "Move Fire 1", url = "/content/images/canyon/movefire_one.png", targetType = "Unit-Tile" };
            for (int i = 0; i < movefire_one_count; i++) deck.Add(movefire_one);
            ConvoyPiece movefire_two = new ConvoyPiece { name = "Move Fire 2", url = "/content/images/canyon/movefire_two.png", targetType = "Unit-Tile" };
            for (int i = 0; i < movefire_two_count; i++) deck.Add(movefire_two);
            ConvoyPiece armoredconvoy_one = new ConvoyPiece { name = "Armored Convoy 1", url = "/content/images/canyon/unit_armoredconvoy1.png", targetType = "Tile" };
            for (int i = 0; i < armoredconvoy_one_count; i++) deck.Add(armoredconvoy_one);
            ConvoyPiece armoredconvoy_three = new ConvoyPiece { name = "Armored Convoy 3", url = "/content/images/canyon/unit_armoredconvoy3.png", targetType = "Tile" };
            for (int i = 0; i < armoredconvoy_three_count; i++) deck.Add(armoredconvoy_three);
            ConvoyPiece artillery = new ConvoyPiece { name = "Artillery", url = "/content/images/canyon/unit_artillery.png", targetType = "Tile" };
            for (int i = 0; i < artillery_count; i++) deck.Add(artillery);
            ConvoyPiece convoy_one = new ConvoyPiece { name = "Convoy 1", url = "/content/images/canyon/unit_convoy1.png", targetType = "Tile" };
            for (int i = 0; i < convoy_one_count; i++) deck.Add(convoy_one);
            ConvoyPiece convoy_three = new ConvoyPiece { name = "Convoy 3", url = "/content/images/canyon/unit_convoy3.png", targetType = "Tile" };
            for (int i = 0; i < convoy_three_count; i++) deck.Add(convoy_three);
            ConvoyPiece convoy_five = new ConvoyPiece { name = "Convoy 5", url = "/content/images/canyon/unit_convoy5.png", targetType = "Tile" };
            for (int i = 0; i < convoy_five_count; i++) deck.Add(convoy_five);
            ConvoyPiece gunship = new ConvoyPiece { name = "Gunship", url = "/content/images/canyon/unit_gunship.png", targetType = "Tile" };
            for (int i = 0; i < gunship_count; i++) deck.Add(gunship);
            ConvoyPiece heavytank = new ConvoyPiece { name = "Heavy Tank", url = "/content/images/canyon/unit_heavytank.png", targetType = "Tile" };
            for (int i = 0; i < heavytank_count; i++) deck.Add(heavytank);
            ConvoyPiece landmines = new ConvoyPiece { name = "Land Mines", url = "/content/images/canyon/unit_mines.png", targetType = "Tile" };
            for (int i = 0; i < landmines_count; i++) deck.Add(landmines);
            ConvoyPiece missileLauncher = new ConvoyPiece { name = "Missile Launcher", url = "/content/images/canyon/unit_missilelauncher.png", targetType = "Tile" };
            for (int i = 0; i < missileLauncher_count; i++) deck.Add(missileLauncher);
            ConvoyPiece railgun = new ConvoyPiece { name = "Rail Gun", url = "/content/images/canyon/unit_railgun.png", targetType = "Tile" };
            for (int i = 0; i < railgun_count; i++) deck.Add(railgun);
            ConvoyPiece tank = new ConvoyPiece { name = "Tank", url = "/content/images/canyon/unit_tank.png", targetType = "Tile" };
            for (int i = 0; i < tank_count; i++) deck.Add(tank);
            #endregion

            DraftDraw();
            state = "Draft";            
        }

        public void DraftDraw()
        {
            Random r = new Random();

            for (int i = 0; i < 3; i++)
            {
                foreach (ConvoyPlayerContext context in playerContexts)
                {
                    if (deck.Count == 0)
                    {
                        deck = discard;
                        discard = new List<ConvoyPiece>();
                    }

                    int randomIndex = r.Next(0, deck.Count);
                    ConvoyPiece piece = deck.ElementAt(randomIndex);
                    deck.RemoveAt(randomIndex);
                    context.draftHand.Add(piece);
                }
            }
        }

        public void PathfindingHelper(int x, int y, int range, bool flying, string side)
        {
            if (x < 0 || y < 0 || x > 10 || y > 11)
                return;
            if (board[x][y].unit == null || board[x][y].unit.name == "Land Mines")
            {
                board[x][y].selectable = true;
            }
            if (range > 0 && ((board[x][y].unit == null && board[x][y].type != "Canyon") || ((board[x][y].unit == null || board[x][y].unit.name != "Land Mines") && flying == true && (board[x][y].type.Contains("Side") || board[x][y].type=="Canyon") )))
            {
                PathfindingHelper(x, y - 1, range - 1, flying, side);
                PathfindingHelper(x, y + 1, range - 1, flying, side);
                PathfindingHelper(x - 1, y, range - 1, flying, side);
                PathfindingHelper(x + 1, y, range - 1, flying, side);
            }
        }

        public void MarkValidTargets(ConvoyPlayerContext context)
        {
            ClearSelections();
            int moveRange = 0;
            if (playedPiece.name.Contains("1"))
                moveRange = 1;
            if (playedPiece.name.Contains("2"))
                moveRange = 2;
            if (playedPiece.name.Contains("3"))
                moveRange = 3;

            board[selectX][selectY].selectable = true;

            PathfindingHelper(selectX, selectY - 1, moveRange-1, board[selectX][selectY].unit.name == "Gunship", context.side);
            PathfindingHelper(selectX, selectY + 1, moveRange - 1, board[selectX][selectY].unit.name == "Gunship", context.side);
            PathfindingHelper(selectX - 1, selectY, moveRange - 1, board[selectX][selectY].unit.name == "Gunship", context.side);
            PathfindingHelper(selectX + 1, selectY, moveRange - 1, board[selectX][selectY].unit.name == "Gunship", context.side);
        }

        public void ClearSelections()
        {
            for (int x = 0; x < 11; x++)
            {
                for (int y = 0; y < 12; y++)
                {
                    board[x][y].selectable = false;
                }
            }
        }

        public void MarkValidSelections(ConvoyPlayerContext context)
        {
            ClearSelections();
            if(playedPiece.name == "Land Mines")
            {
                // Empty spaces on opponent side
                for (int x = 1; x < 10; x++)
                {
                    for (int y = 1; y < 11; y++)
                    {
                        if (!board[x][y].type.Contains(context.side) && board[x][y].type != "Canyon")
                        {
                            if (board[x][y].unit == null)
                            {
                                board[x][y].selectable = true;
                            }
                        }
                    }
                }
            }
            if(playedPiece.targetType.Contains("Unit"))
            {
                // Any friendly piece except land mines
                for (int x = 1; x < 10; x++)
                {
                    for (int y = 0; y < 12; y++)
                    {
                        if (board[x][y].unit != null && board[x][y].unit.side == context.side && board[x][y].unit.name != "Land Mines")
                        {
                            if (playedPiece.name.Contains("Fire") && (board[x][y].unit.name.Contains("Convoy") || board[x][y].unit.name == "Heavy Tank"))
                                continue;
                            board[x][y].selectable = true;
                        }
                    }
                }
            }
            if (playedPiece.targetType == "Tile")
            {
                // Start tile on friendly side
                for (int x = 1; x < 10; x++)
                {
                    for (int y = 0; y < 12; y++)
                    {
                        if (board[x][y].type.Contains("Start") && board[x][y].type.Contains(context.side) && board[x][y].unit == null)
                        {
                            board[x][y].selectable = true;
                        }
                    }
                }
            }
        }

        public int ValidTargetCount()
        {
            int count =0;
            for (int x = 0; x < 11; x++)
            {
                for (int y = 0; y < 12; y++)
                {
                    if (board[x][y].selectable == true) count++;
                }
            }
            return count;
        }

        public bool IsUnitFunctioning(ConvoyPiece piece)
        {
            if (piece.name == "Heavy Tank" || piece.name.Contains("Armored"))
                return piece.hits < 2;
            else
                return piece.hits < 1;
        }

        public void UnitFire(int x, int y)
        {
            ConvoyPiece unit = board[x][y].unit;
            if (unit == null || unit.name == "Land Mines" || unit.name == "Heavy Tank" || unit.name.Contains("Convoy")) return;

            int fireDir = 0;
            if (unit.side == "Right")
                fireDir = -1;
            if (unit.side == "Left")
                fireDir = 1;

            if (unit.name == "Tank" || unit.name == "Rail Gun")
            {
                for (int i = x + fireDir; i < 9 && i >= 0; i += fireDir)
                {
                    if (board[i][y].unit != null)
                    {
                        if(board[i][y].unit.name != "Rock" && board[i][y].unit.name != "Gunship")
                            board[i][y].unit.hits++;

                        if (unit.name == "Tank" && board[i][y].unit.name != "Gunship")
                            return;
                    }
                }
            }
            if (unit.name == "Gunship")
            {
                for (int i = x + fireDir; i < 9 && i >= 0 && i < x + 3 && i > x - 3; i += fireDir)
                {
                    if (board[i][y].unit != null && board[i][y].unit.name != "Rock" && board[i][y].unit.name != "Gunship")
                    {
                        board[i][y].unit.hits++;
                    }
                }
            }
            if (unit.name == "Artillery")
            {
                int farX = 0;
                if (fireDir == 1)
                    farX = 8;
                if (fireDir == -1)
                    farX = 0;
                for (int i = farX; i < 9 && i >= 0 && i!=4; i -= fireDir)
                {
                    if (board[i][y].unit != null && board[i][y].unit.name != "Rock" && board[i][y].unit.name != "Gunship")
                    {
                        board[i][y].unit.hits++;                        
                        return;
                    }
                }
            }
            if (unit.name == "Missile Launcher")
            {
                for (int tx = 0; tx < 9; tx++)
                {
                    for (int ty = 0; ty < 12; ty++)
                    {
                        if (board[tx][ty].unit != null && board[tx][ty].unit.name == "Gunship" && board[tx][ty].unit.side != unit.side && Math.Abs(tx - x) + Math.Abs(ty - y) <= 4)
                        {
                            board[tx][ty].unit.hits++;
                        }
                    }
                }
            }
        }

        public void IncrementActivePlayer()
        {
            activePlayerIndex++;
            activePlayerIndex %= playerContexts.Count;
            activePlayerId = playerContexts[activePlayerIndex].playerId;
        }


        public void Update(ConvoyUpdate update)
        {
            if (state == "Draft")
            {
                // Set draft selection for player
                foreach (ConvoyPlayerContext context in playerContexts)
                {
                    if (update.playerId == context.playerId)
                    {
                        context.draftReady = true;
                        context.draftIndex = update.selectedCardIndex;
                    }
                }
                // If both players have selected card, move cards to hands
                bool draftRoundOver = true;                
                foreach (ConvoyPlayerContext context in playerContexts)
                {
                    if (context.draftReady == false)
                    {
                        logs.Add("Draft round complete. Swapping hands.");
                        draftRoundOver = false;
                    }
                }
                if (draftRoundOver == true)
                {
                    foreach (ConvoyPlayerContext context in playerContexts)
                    {
                        ConvoyPiece draftedPiece = context.draftHand[context.draftIndex];
                        context.draftHand.RemoveAt(context.draftIndex);
                        context.draftReady = false;
                        context.hand.Add(draftedPiece);
                    }
                    List<ConvoyPiece> tempPieces = playerContexts[0].draftHand;
                    playerContexts[0].draftHand = playerContexts[1].draftHand;
                    playerContexts[1].draftHand = tempPieces;
                }
                // If draft hands single pieces, put them in hand and advance to next phase
                if (playerContexts[0].draftHand.Count == 1)
                {
                    logs.Add("Only one remaining card in draft. Draft complete.");
                    foreach (ConvoyPlayerContext context in playerContexts)
                    {
                        context.hand.Add(context.draftHand[0]);
                        context.draftHand.RemoveAt(0);
                    }

                    activePlayerId = primaryPlayerId;
                    for(int i = 0; i < playerContexts.Count; i++)
                    {
                        if (playerContexts[i].playerId == activePlayerId)
                            activePlayerIndex = i;
                    }
                    playedPiece = new ConvoyPiece { name = "Bonus Move 2", url = "/content/images/canyon/move_two.png", targetType = "Unit-Tile" };
                    MarkValidSelections(playerContexts[activePlayerIndex]);
                    if (ValidTargetCount() == 0)
                    {
                        logs.Add("No valid move targets for " + playerContexts[activePlayerIndex].name + ". Turn forfeited.");
                        IncrementActivePlayer();
                        playedPiece = null;
                        state = "PlayCard";
                        
                    }
                    else
                    {
                        logs.Add(playerContexts[activePlayerIndex].name + " takes bonus Move-2 phase.");
                        state = "Select";
                    }
                }
                
            }
            else
            {
                // Must be active player
                if(update.playerId != activePlayerId)
                    return;

                foreach(ConvoyPlayerContext context in playerContexts)
                {
                    if(context.playerId == activePlayerId)
                    {
                        if (state == "PlayCard")
                        {
                            // Move chosen card to play area.
                            playedPiece = context.hand[update.selectedCardIndex];
                            context.hand.RemoveAt(update.selectedCardIndex);
                            // Advance to select if necessary, marking valid tiles 
                            MarkValidSelections(playerContexts[activePlayerIndex]);

                            logs.Add(playerContexts[activePlayerIndex].name + " played " + playedPiece + ".");

                            if (playedPiece.targetType == "None")
                            {
                                state = "Execute";
                            }
                            else if (ValidTargetCount() == 0)
                            {
                                dudPiece = true;
                                logs.Add("No valid targets.");
                                state = "Execute";
                            }
                            else
                            {                                
                                state = "Select";
                            }
                        }
                        else if (state == "Select")
                        {
                            // Update selectedX, selectY
                            selectX = update.selectedTileX;
                            selectY = update.selectedTileY;

                            logs.Add(playerContexts[activePlayerIndex].name + " selected tile " + selectX + "," + selectY + ".");
                            // Advance to target if necessary, marking valid tiles                            
                            if (playedPiece.targetType == "Tile" || playedPiece.targetType == "Unit")
                            {
                                state = "Execute";
                            }
                            else
                            {
                                MarkValidTargets(playerContexts[activePlayerIndex]);
                                if (ValidTargetCount() == 0)
                                {
                                    logs.Add("No valid targets.");
                                    dudPiece = true;
                                    state = "Execute";
                                }
                                else
                                {
                                    state = "Target";
                                }
                            }
                        }
                        else if (state == "Target")
                        {
                            // Update targetX, targetY
                            targetX = update.selectedTileX;
                            targetY = update.selectedTileY;
                            logs.Add(playerContexts[activePlayerIndex].name + " selected tile " + targetX + "," + targetY + ".");
                            // Advance to execute
                            state = "Execute";
                        }
                    }
                }
            }
            if (state == "Execute")
            {
                // Fall through - by now we have Piece, src, and target, or at least whatever is necessary
                // Perform action
                if (dudPiece == true)
                {
                    if (!playedPiece.name.Contains("Bonus"))
                    {
                        logs.Add(playerContexts[activePlayerIndex].name + " discards " + playedPiece.name);
                        discard.Add(playedPiece);
                    }
                }
                else if (playedPiece.name == "Fire All")
                {
                    // Fire all pieces
                    for (int x = 1; x < 10; x++)
                    {
                        for (int y = 1; y < 11; y++)
                        {
                            UnitFire(x, y);
                        }
                    }
                    logs.Add(playerContexts[activePlayerIndex].name + " discards " + playedPiece.name);
                    discard.Add(playedPiece);
                }
                else if (playedPiece.name.Contains("Move"))
                {
                    logs.Add(playerContexts[activePlayerIndex].name + " moved " + board[selectX][selectY].unit.name + " to " + targetX + "," + targetY);
                    // Move piece from selectXY to targetXY
                    if (board[targetX][targetY].unit!= null && board[targetX][targetY].unit.name == "Land Mines")
                    {
                        logs.Add("Land Mines detonate, damaging " + board[selectX][selectY].unit.name);
                        logs.Add(playerContexts[activePlayerIndex].name + " discards " + board[targetX][targetY].unit.name);
                        discard.Add(board[targetX][targetY].unit);
                        board[selectX][selectY].unit.hits++;
                    }                    
                    board[targetX][targetY].unit = board[selectX][selectY].unit;
                    if(selectX != targetX || selectY != targetY)
                        board[selectX][selectY].unit = null;

                    if (playedPiece.name.Contains("Fire"))
                    {
                        // Fire piece at targetXY
                        if (IsUnitFunctioning(board[targetX][targetY].unit))
                        {
                            UnitFire(targetX, targetY);
                        }
                    }
                    if (!playedPiece.name.Contains("Bonus"))
                    {
                        logs.Add(playerContexts[activePlayerIndex].name + " discards " + playedPiece.name);
                        discard.Add(playedPiece);
                    }
                }
                else if (playedPiece.name.Contains("Fire"))
                {
                    // Fire piece at selectXY
                    UnitFire(selectX, selectY);
                    logs.Add(playerContexts[activePlayerIndex].name + " discards " + playedPiece.name);
                    discard.Add(playedPiece);
                }
                else
                {
                    // Place piece at selectXY
                    board[selectX][selectY].unit = playedPiece;                    
                    board[selectX][selectY].unit.hits = 0;
                    board[selectX][selectY].unit.side = playerContexts[activePlayerIndex].side;
                    logs.Add(playerContexts[activePlayerIndex].name + " places " + playedPiece.name + " at " + selectX + "," + selectY);

                    board[selectX][selectY].unit.direction = 0;
                    if (board[selectX][selectY].unit.side == "Left")
                    {
                        board[selectX][selectY].unit.direction += 2;
                    }
                    if (board[selectX][selectY].unit.name == "Tank" || board[selectX][selectY].unit.name == "Rail Gun" || board[selectX][selectY].unit.name == "Gunship" || board[selectX][selectY].unit.name == "Artillery")
                    {
                        board[selectX][selectY].unit.direction +=3;                        
                    }
                    board[selectX][selectY].unit.direction %= 4;
                    
                }
                // Increment activeplayer
                IncrementActivePlayer();
                ClearSelections();
                if (playerContexts[activePlayerIndex].hand.Count == 0)
                {
                    // If hands empty, increment primary player and go to draft
                    primaryPlayerId = activePlayerId;
                    DraftDraw();
                    state = "Draft";
                    playedPiece = null;
                }
                else
                {
                    // else return to playcard phase
                    state = "PlayCard";
                    playedPiece = null;
                    ClearSelections();
                    dudPiece = false;
                }

            }

            // Destroy units, check endgame conditions
            for (int x = 0; x < 11; x++)
            {
                for (int y = 0; y < 12; y++)
                {
                    if (board[x][y].unit != null)
                    {
                        if (IsUnitFunctioning(board[x][y].unit) == false)
                        {
                            discard.Add(board[x][y].unit);
                            board[x][y].unit = null;
                        }
                        if (board[x][y].type == "Canyon" && board[x][y].unit != null && board[x][y].unit.name != "Gunship")
                        {
                            discard.Add(board[x][y].unit);
                            board[x][y].unit = null;
                        }
                        if (board[x][y].type.Contains("End") && board[x][y].unit.name.Contains("Convoy"))
                        {
                            foreach (ConvoyPlayerContext context in playerContexts)
                            {
                                if (context.side == board[x][y].unit.side)
                                {
                                    if (board[x][y].unit.name.Contains("1"))
                                    {
                                        context.score++;
                                    }
                                    if (board[x][y].unit.name.Contains("3"))
                                    {
                                        context.score+=3;
                                    }
                                    if (board[x][y].unit.name.Contains("5"))
                                    {
                                        context.score+=5;
                                    }
                                }
                            }
                            discard.Add(board[x][y].unit);
                            board[x][y].unit = null;
                        }
                    }
                }
            }
            foreach (ConvoyPlayerContext context in playerContexts)
            {
                if (context.score >= 10)
                {
                    gameOver = true;
                }
            }
        }

        public ConvoyState GetClientState(int playerId)
        {
            this.sourcePlayerId = playerId;
            ConvoyState clientState = new ConvoyState();
            clientState.board = this.board;
            clientState.activePlayerId = this.activePlayerId;
            clientState.activePlayerIndex = this.activePlayerIndex;
            clientState.deck = null;
            clientState.discard = null;
            clientState.dudPiece = false;
            clientState.gameOver = this.gameOver;
            clientState.playedPiece = this.playedPiece;
            clientState.primaryPlayerId = this.primaryPlayerId;
            clientState.selectX = this.selectX;
            clientState.selectY = this.selectY;
            clientState.state = this.state;
            clientState.tableId = this.tableId;
            clientState.sourcePlayerId = playerId;
            clientState.targetX = this.targetX;
            clientState.targetY = this.targetY;
            clientState.playerContexts = new List<ConvoyPlayerContext>();
            clientState.logs = this.logs;
            foreach (ConvoyPlayerContext context in playerContexts)
            {
                ConvoyPlayerContext clientContext = new ConvoyPlayerContext();
                clientContext.score = context.score;
                clientContext.playerId = context.playerId;
                clientContext.name = context.name;
                clientContext.side = context.side;
                if (clientContext.playerId == playerId)
                {
                    clientContext.hand = context.hand;
                    clientContext.draftIndex = context.draftIndex;
                    clientContext.draftHand = context.draftHand;
                    clientContext.draftReady = context.draftReady;
                }
                clientState.playerContexts.Add(clientContext);
            }
            return clientState;
        }
    }
}
