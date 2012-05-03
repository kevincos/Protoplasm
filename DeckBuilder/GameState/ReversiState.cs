using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Protoplasm;
using DeckBuilder.Models;

namespace DeckBuilder.Games
{
    public class ReversiPlayerContext : PlayerContext
    {
        public string color { get; set; }
    }


    public class ReversiState : GameState<ReversiPlayerContext>
    {
        public override System.Runtime.Serialization.Json.DataContractJsonSerializer GetSerializer()
        {
            return new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ReversiState), new Type[] {typeof(ReversiPlayerContext), typeof(PlayerContext), typeof(GameObject), typeof(GameUpdate), typeof(GameView), typeof(SquareBoard<GamePiece>), typeof(GamePiece), typeof(SquareTile<GamePiece>)});
        }

        public SquareBoard<GamePiece> board { get; set; }

        public override void InitializeState(List<Seat> seats)
        {
            base.InitializeState(seats);
            playerContexts[0].color = "white";
            playerContexts[1].color = "black";

            board = new SquareBoard<GamePiece>("MainBoard", 8, 8);
            for (int x = 0; x < board.width; x++)
            {
                for (int y = 0; y < board.length; y++)
                {
                    board.grid[x][y].url = "/content/images/classic/reversitile.png";
                }
            }
            board.grid[3][3].pieces.Add(GetPiece("black"));
            board.grid[4][4].pieces.Add(GetPiece("black"));
            board.grid[3][4].pieces.Add(GetPiece("white"));
            board.grid[4][3].pieces.Add(GetPiece("white"));

            MarkValidSpaces(playerContexts.Find(pc => pc.playerId == activePlayerId).color);
        }

        public GamePiece GetPiece(string color)
        {
            GamePiece piece = new GamePiece();
            piece.name = color;            
            piece.url = "/content/images/classic/"+color+"chip.png";            
            return piece;
        }

        public void FlipPiece(GamePiece piece)
        {
            if (piece.name == "black")
            {
                piece.name = "white";
                piece.url = "/content/images/classic/whitechip.png";
            }
            else
            {
                piece.name = "black";
                piece.url = "/content/images/classic/blackchip.png";
            }
        }

        public int MarkValidSpaces(string color)
        {
            board.ClearSelection();
            int validSpaces = 0;
            for (int x = 0; x < board.length; x++)
            {
                for (int y = 0; y < board.width; y++)
                {
                    if (CountFlips(x, y, color) > 0)
                    {
                        validSpaces++;
                        board.grid[x][y].selectable = true;
                    }
                }
            }
            return validSpaces;
        }

        public int CountFlips(int x, int y, string color)
        {
            int totalFlips = 0;
            if (board.grid[x][y].pieces.Count != 0)
                return 0;
            // Flip pieces horiz
            for (int i = x + 1; i < board.length; i++)
            {
                GamePiece piece = board.grid[i][y].pieces.FirstOrDefault();
                if (piece != null && piece.name == color)
                {
                    // Fill in
                    totalFlips += i - x - 1;
                    break;
                }
                else if (piece == null)
                    break;
            }
            for (int i = x - 1; i >= 0; i--)
            {
                GamePiece piece = board.grid[i][y].pieces.FirstOrDefault();
                if (piece != null && piece.name == color)
                {
                    // Fill in
                    totalFlips += x- i-1;
                    break;
                }
                else if (piece == null)
                    break;
            }
            // Flip vertical
            for (int i = y + 1; i < board.width; i++)
            {
                GamePiece piece = board.grid[x][i].pieces.FirstOrDefault();
                if (piece != null && piece.name == color)
                {
                    // Fill in
                    totalFlips += i - y - 1;
                    break;
                }
                else if (piece == null)
                    break;
            }
            for (int i = y - 1; i >= 0; i--)
            {
                GamePiece piece = board.grid[x][i].pieces.FirstOrDefault();
                if (piece != null && piece.name == color)
                {
                    // Fill in
                    totalFlips += y - i - 1;
                    break;
                }
                else if (piece == null)
                    break;
            }
            //Diagonals
            for (int i = 1; ; i++)
            {
                if (x + i >= board.length || y + i >= board.width)
                    break;
                GamePiece piece = board.grid[x + i][y + i].pieces.FirstOrDefault();
                if (piece == null)
                    break;
                if (piece.name == color) {
                    totalFlips += i - 1;
                    break;
                }
            }
            for (int i = 1; ; i++)
            {
                if (x - i < 0 || y - i < 0)
                    break;
                GamePiece piece = board.grid[x - i][y - i].pieces.FirstOrDefault();
                if (piece == null)
                    break;
                if (piece.name == color)
                {
                    totalFlips += i - 1;
                    break;
                }
            }
            for (int i = 1; ; i++)
            {
                if (x + i >= board.length || y - i < 0)
                    break;
                GamePiece piece = board.grid[x + i][y - i].pieces.FirstOrDefault();
                if (piece == null)
                    break;
                if (piece.name == color)
                {
                    totalFlips += i - 1;
                    break;
                }
            }
            for (int i = 1; ; i++)
            {
                if (x - i < 0|| y + i >= board.width)
                    break;
                GamePiece piece = board.grid[x - i][y + i].pieces.FirstOrDefault();
                if (piece == null)
                    break;
                if (piece.name == color)
                {
                    totalFlips += i - 1;
                    break;
                }
            }



            return totalFlips;
        }

        public void DoFlips(int x, int y, string color)
        {
            // Flip pieces horiz
            for (int i = x + 1; i < board.length; i++)
            {
                GamePiece piece = board.grid[i][y].pieces.FirstOrDefault();
                if (piece != null && piece.name == color)
                {
                    // Fill in
                    for (int j = x + 1; j < i; j++)
                        FlipPiece(board.grid[j][y].pieces.FirstOrDefault());
                    break;
                }
                else if (piece == null)
                    break;
            }
            for (int i = x - 1; i >= 0; i--)
            {
                GamePiece piece = board.grid[i][y].pieces.FirstOrDefault();
                if (piece != null && piece.name == color)
                {
                    // Fill in
                    for (int j = x - 1; j > i; j--)
                        FlipPiece(board.grid[j][y].pieces.FirstOrDefault());
                    break;
                }
                else if (piece == null)
                    break;
            }

            // Flip Verticals
            for (int i = 1; ; i++)
            {
                if (y + i >= board.width)
                    break;
                GamePiece piece = board.grid[x][y + i].pieces.FirstOrDefault();
                if (piece == null)
                    break;
                if (piece.name == color)
                {
                    for (int j = 1; j < i; j++)
                    {
                        FlipPiece(board.grid[x][y + j].pieces.FirstOrDefault());
                    }
                    break;
                }
            }
            for (int i = 1; ; i++)
            {
                if (y - i < 0)
                    break;
                GamePiece piece = board.grid[x][y - i].pieces.FirstOrDefault();
                if (piece == null)
                    break;
                if (piece.name == color)
                {
                    for (int j = 1; j < i; j++)
                    {
                        FlipPiece(board.grid[x][y - j].pieces.FirstOrDefault());
                    }
                    break;
                }
            }

            // Flip Diagonals
            for (int i = 1; ; i++)
            {
                if (x + i >= board.length || y + i >= board.width)
                    break;
                GamePiece piece = board.grid[x + i][y + i].pieces.FirstOrDefault();
                if (piece == null)
                    break;
                if (piece.name == color)
                {
                    for (int j = 1; j < i; j++)
                    {
                        FlipPiece(board.grid[x + j][y + j].pieces.FirstOrDefault());                        
                    }
                    break;
                }
            }
            for (int i = 1; ; i++)
            {
                if (x - i < 0 || y - i < 0)
                    break;
                GamePiece piece = board.grid[x - i][y - i].pieces.FirstOrDefault();
                if (piece == null)
                    break;
                if (piece.name == color)
                {
                    for (int j = 1; j < i; j++)
                    {
                        FlipPiece(board.grid[x - j][y - j].pieces.FirstOrDefault());
                    }
                    break;
                }
            }
            for (int i = 1; ; i++)
            {
                if (x + i >= board.length || y - i < 0)
                    break;
                GamePiece piece = board.grid[x + i][y - i].pieces.FirstOrDefault();
                if (piece == null)
                    break;
                if (piece.name == color)
                {
                    for (int j = 1; j < i; j++)
                    {
                        FlipPiece(board.grid[x + j][y - j].pieces.FirstOrDefault());
                    }
                    break;
                }
            }
            for (int i = 1; ; i++)
            {
                if (x - i < 0 || y + i >= board.width)
                    break;
                GamePiece piece = board.grid[x - i][y + i].pieces.FirstOrDefault();
                if (piece == null)
                    break;
                if (piece.name == color)
                {
                    for (int j = 1; j < i; j++)
                    {
                        FlipPiece(board.grid[x - j][y + j].pieces.FirstOrDefault());
                    }
                    break;
                }
            }
        }

        public override void Update(GameUpdate update)
        {
            if (update.playerId == activePlayerId)
            {
                ReversiPlayerContext updateContext = playerContexts.Find(pc => pc.playerId == update.playerId);
                int flips = CountFlips(update.selectX, update.selectY, updateContext.color);

                board.grid[update.selectX][update.selectY].pieces.Add(GetPiece(updateContext.color));
                
                DoFlips(update.selectX, update.selectY, updateContext.color);
                logs.Add(updateContext.color + " plays a piece at " + update.selectX + "," + update.selectY + " flipping " + flips + " pieces.");
 

                AdvanceActivePlayer();

                // Mark Empty Spaces
                int validSpaces = MarkValidSpaces(playerContexts.Find(pc => pc.playerId == activePlayerId).color);

                if (validSpaces == 0)
                {
                    // LOG LOSE A TURN
                    logs.Add(playerContexts.Find(pc => pc.playerId == activePlayerId).color + " has no valid moves and must forfeit a turn.");
                    AdvanceActivePlayer();
                    validSpaces = MarkValidSpaces(playerContexts.Find(pc => pc.playerId == activePlayerId).color);
                    if (validSpaces == 0)
                    {
                        logs.Add("No valid moves remaining. Game Over!");
                        int whiteCount = 0;
                        int blackCount = 0;
                        for (int x = 0; x < board.length; x++)
                        {
                            for (int y = 0; y < board.length; y++)
                            {
                                if (board.grid[x][y].pieces.Count > 0)
                                {
                                    if (board.grid[x][y].pieces.FirstOrDefault().name == "black")
                                        blackCount++;
                                    if (board.grid[x][y].pieces.FirstOrDefault().name == "white")
                                        whiteCount++;
                                }
                            }
                        }
                        logs.Add("Final Score - Black: " + blackCount + " White: " + whiteCount);
                        string winner = "";
                        if (blackCount > whiteCount)
                            winner = "Black";
                        if (whiteCount > blackCount)
                            winner = "White";
                        if (winner == "")
                            logs.Add("Draw!");
                        else
                            logs.Add(winner + " wins!");

                        // LOG GAME OVER!
                        gameOver = true;
                    }
                    
                }
            }
        }

        public override GameView GetClientView(int playerId)
        {
            GameView view = new GameView();
            view.activePlayerId = playerId;
            view.tableId = tableId;
            SquareBoard<GamePiece> board = this.board.View(400, 300, 300, 300);
            if (playerId != activePlayerId)
                board.ClearSelection();
            view.drawList.Add(board.View(400,300, 300, 300));
            view.logs = logs;
            return view;
        }
    }
}