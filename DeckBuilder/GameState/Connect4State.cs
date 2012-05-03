using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;
using DeckBuilder.Protoplasm;

namespace DeckBuilder.Games
{
    public class Connect4PlayerContext:PlayerContext
    {
        public string color {get;set;}        
    }

    public class Connect4State : GameState<Connect4PlayerContext>
    {
        public override System.Runtime.Serialization.Json.DataContractJsonSerializer GetSerializer()
        {
            return new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Connect4State), new Type[] { typeof(Connect4PlayerContext), typeof(PlayerContext), typeof(GameObject), typeof(GameUpdate), typeof(GameView), typeof(SquareBoard<GamePiece>), typeof(GamePiece), typeof(SquareTile<GamePiece>) });
        }

        public SquareBoard<GamePiece> board { get; set; }

        public override void InitializeState(List<Seat> seats)
        {
            base.InitializeState(seats);
            playerContexts[0].color = "Black";
            playerContexts[1].color = "Red";

            board = new SquareBoard<GamePiece>("MainBoard", 7, 7);
            for (int x = 0; x < board.width; x++)
            {
                for (int y = 0; y < board.length; y++)
                {
                    board.grid[x][y].url = "/content/images/classic/connect4grid.png";
                }
            }

            MarkValidSpaces();
        }

        public int MarkValidSpaces()
        {
            board.ClearSelection();
            int validSpaces = 0;
            for (int x = 0; x < board.length; x++)
            {
                for (int y = 0; y < board.width; y++)
                {
                    if (board.grid[x][y].pieces.Count ==  0)
                    {
                        validSpaces++;
                        board.grid[x][y].selectable = true;
                    }
                }
            }
            return validSpaces;
        }

        public override void Update(GameUpdate update)
        {
            if (activePlayerId != update.playerId || gameOver == true)
                return;

            for (int playerIndex = 0; playerIndex < playerContexts.Count; playerIndex++)
            {
                Connect4PlayerContext context = playerContexts[playerIndex];
                if (context.playerId == update.playerId)
                {
                    int column = update.selectX;
                    int row = update.selectY;

                    while (row + 1 < 7 && board.grid[column][row + 1].pieces.Count == 0)
                        row++;
                    board.grid[column][row].pieces.Add(new GamePiece { name = context.color, url = "/content/images/classic/" + context.color + "chip.png" });
                    logs.Add(context.color + " drops a chip in column " + (column+1) + ".");
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
                        GamePiece piece = board.grid[x][y].pieces.FirstOrDefault();
                        if (piece != null && piece.name == "Red")
                        {
                            longestRedStreak++;
                            longestBlackStreak = 0;
                            if (longestRedStreak == 4)
                                redWins = true;
                        }
                        if (piece != null && piece.name == "Black")
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
                        GamePiece piece = board.grid[x][y].pieces.FirstOrDefault();
                        if (piece != null && piece.name == "Red")
                        {
                            longestRedStreak++;
                            longestBlackStreak = 0;
                            if (longestRedStreak == 4)
                                redWins = true;
                        }
                        if (piece != null && piece.name == "Black")
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
                        GamePiece piece = board.grid[x - 4 + y][y].pieces.FirstOrDefault();
                        if (piece != null && piece.name == "Red")
                        {
                            longestRedStreak++;
                            longestBlackStreak = 0;
                            if (longestRedStreak == 4)
                                redWins = true;
                        }
                        if (piece != null && piece.name == "Black")
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
                        GamePiece piece = board.grid[x - 4 + y][6 - y].pieces.FirstOrDefault();
                        if (piece != null && piece.name == "Red")
                        {
                            longestRedStreak++;
                            longestBlackStreak = 0;
                            if (longestRedStreak == 4)
                                redWins = true;
                        }
                        if (piece != null && piece.name == "Black")
                        {
                            longestRedStreak = 0;
                            longestBlackStreak++;
                            if (longestBlackStreak == 4)
                                blackWins = true;
                        }
                    }
                }
                if (redWins || blackWins)
                {
                    if (redWins == true)
                        logs.Add("Red has 4 in a row! Red Wins!");
                    if (blackWins == true)
                        logs.Add("Black has 4 in a row! Black Wins!");
                    this.gameOver = true;
                }
            }


            AdvanceActivePlayer();
            int remainingSpaces = MarkValidSpaces();
            if (gameOver == false && remainingSpaces == 0)
            {
                gameOver = true;
                logs.Add("No spaces remaining! Game is a Draw!");
            }
        }

        public override GameView GetClientView(int playerId)
        {
            GameView view = new GameView();
            view.activePlayerId = playerId;
            view.tableId = tableId;
            SquareBoard<GamePiece> board = this.board.View(400, 300, 400, 400);
            if (playerId != activePlayerId || gameOver == true)
                board.ClearSelection();
            view.drawList.Add(board.View(400, 300, 400, 400));
            view.logs = logs;
            return view;
        }
    }
}