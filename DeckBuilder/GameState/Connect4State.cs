using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;

namespace DeckBuilder.Games
{
    public class Connect4PlayerContext
    {
        public string color {get;set;}
        public int playerId { get; set; }
        public string name { get; set; }
    }

    public class Connect4Update
    {
        public int playerId { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class Connect4State
    {
        public List<Connect4PlayerContext> playerContexts { get; set; }
        public int sourcePlayerId { get; set; }
        public int activePlayerId { get; set; }
        public int activePlayerIndex { get; set; }
        public int tableId { get; set; }
        public bool gameOver { get; set; }

        public List<List<string>> grid { get; set; }

        public void InitializeState(List<Seat> seats)
        {
            tableId = seats[0].TableId;
            playerContexts = new List<Connect4PlayerContext>();
            foreach (Seat seat in seats)
            {
                Connect4PlayerContext playerContext = new Connect4PlayerContext();
                playerContext.playerId = seat.PlayerId;
                playerContext.name = seat.Player.Name;                
                playerContexts.Add(playerContext);
            }
            playerContexts[0].color = "Black";
            playerContexts[1].color = "Red";
            activePlayerIndex = 0;
            activePlayerId = playerContexts[activePlayerIndex].playerId;

            grid = new List<List<string>>();
            for (int x = 0; x < 7; x++)
            {
                grid.Add(new List<string>());
                for(int y =0; y < 7; y++)
                {
                    grid[x].Add("Empty");
                }
            }
        }

        public void Update(Connect4Update update)
        {
            if (activePlayerId != update.playerId)
                return;

            for (int playerIndex = 0; playerIndex < playerContexts.Count; playerIndex++)
            {
                Connect4PlayerContext context = playerContexts[playerIndex];
                if (context.playerId == update.playerId)
                {
                    int column = update.x;
                    int row = update.y;

                    while (row + 1 < 7 && grid[column][row + 1] == "Empty")
                        row++;
                    grid[column][row] = context.color;
                }
            }

            // Check for winner
            {
                bool redWins = false;
                bool blackWins = false;
                //Horiz
                for (int x = 0; x < 7; x++)
                {
                    int longestRedStreak = 0;
                    int longestBlackStreak = 0;
                    for (int y = 0; y < 7; y++)
                    {
                        if (grid[x][y] == "Red")
                        {
                            longestRedStreak++;
                            longestBlackStreak = 0;
                            if (longestRedStreak == 4)
                                redWins = true;
                        }
                        if (grid[x][y] == "Black")
                        {
                            longestRedStreak=0;
                            longestBlackStreak++;
                            if (longestBlackStreak == 4)
                                blackWins = true;
                        }
                    }
                }
                //Vert
                for (int y = 0; y < 7; y++)
                {
                    int longestRedStreak = 0;
                    int longestBlackStreak = 0;
                    for (int x = 0; x < 7; x++)
                    {
                        if (grid[x][y] == "Red")
                        {
                            longestRedStreak++;
                            longestBlackStreak = 0;
                            if (longestRedStreak == 4)
                                redWins = true;
                        }
                        if (grid[x][y] == "Black")
                        {
                            longestRedStreak = 0;
                            longestBlackStreak++;
                            if (longestBlackStreak == 4)
                                blackWins = true;
                        }
                    }
                }
                //DiagUpRight
                for (int x = 0; x < 7; x++)
                {
                    int longestRedStreak = 0;
                    int longestBlackStreak = 0;
                    for (int y = 0; y < 7; y++)
                    {
                        if (x - 4 + y < 0 || x - 4 + y >= 6)
                            continue;
                        if (grid[x-4+y][y] == "Red")
                        {
                            longestRedStreak++;
                            longestBlackStreak = 0;
                            if (longestRedStreak == 4)
                                redWins = true;
                        }
                        if (grid[x-4+y][y] == "Black")
                        {
                            longestRedStreak = 0;
                            longestBlackStreak++;
                            if (longestBlackStreak == 4)
                                blackWins = true;
                        }
                    }
                }
                //DiagDownRight
                for (int x = 0; x < 7; x++)
                {
                    int longestRedStreak = 0;
                    int longestBlackStreak = 0;
                    for (int y = 0; y < 7; y++)
                    {
                        if (x - 4 + y < 0 || x - 4 + y >= 7)
                            continue;
                        if (grid[x - 4 + y][6-y] == "Red")
                        {
                            longestRedStreak++;
                            longestBlackStreak = 0;
                            if (longestRedStreak == 4)
                                redWins = true;
                        }
                        if (grid[x - 4 + y][6-y] == "Black")
                        {
                            longestRedStreak = 0;
                            longestBlackStreak++;
                            if (longestBlackStreak == 4)
                                blackWins = true;
                        }
                    }
                }
                if (redWins || blackWins)
                    this.gameOver = true;
            }
            

            activePlayerIndex++;
            activePlayerIndex %= playerContexts.Count;
            activePlayerId = playerContexts[activePlayerIndex].playerId;
        }

        public Connect4State GetClientState(int playerId)
        {
            this.sourcePlayerId = playerId;
            return this;
        }
    }
}