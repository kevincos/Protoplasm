using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Protoplasm
{
    public class SquarePiece
    {
        public string url { get; set; }
    }

    public class SquareTile
    {
        public string url { get; set; }

        public List<SquarePiece> pieces { get; set; }

        public SquareTile()
        {
            pieces = new List<SquarePiece>();
        }
    }

    public class SquareBoard
    {
        public List<List<SquareTile>> board { get; set; }
        public int length { get; set; }
        public int width { get; set; }

        public SquareBoard(int length, int width)
        {
            this.length = length;
            this.width = width;
            board = new List<List<SquareTile>>();
            for (int x = 0; x < length; x++)
            {
                board.Add(new List<SquareTile>());
                for (int y = 0; y < width; y++)
                {
                    board[x].Add(new SquareTile());
                }
            }
        }
    }
}