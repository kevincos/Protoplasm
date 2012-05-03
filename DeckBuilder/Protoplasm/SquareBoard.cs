using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Protoplasm
{
    public class SquareTile<T> where T : GamePiece
    {
        public string type { get; set; }
        public string url { get; set; }

        public List<T> pieces { get; set; }

        public bool selectable { get; set; }

        public SquareTile()
        {
            pieces = new List<T>();
        }

        public SquareTile<T> View()
        {
            SquareTile<T> viewTile = new SquareTile<T> { url = url, selectable = selectable };
            viewTile.pieces = new List<T>();
            foreach (T piece in pieces)
                viewTile.pieces.Add((T)(piece.View()));
            return viewTile;
        }

        public bool IsEmpty
        {
            get
            {
                return pieces.Count == 0;
            }
        }

        public T TopPiece
        {
            get
            {
                return pieces.FirstOrDefault();
            }
        }
    }


    public class SquareBoard<T> : GameObject where T:GamePiece
    {
        public string name { get; set; }

        public List<List<SquareTile<T>>> grid { get; set; }
        public int length { get; set; }
        public int width { get; set; }

        public int x { get; set; }
        public int y { get; set; }
        public int xSize { get; set; }
        public int ySize { get; set; }

        public SquareBoard()
        {
        }

        public SquareBoard(string name, int length, int width)
        {
            this.name = name;
            this.type = "SquareBoard";
            this.length = length;
            this.width = width;
            grid = new List<List<SquareTile<T>>>();
            for (int x = 0; x < length; x++)
            {
                grid.Add(new List<SquareTile<T>>());
                for (int y = 0; y < width; y++)
                {
                    grid[x].Add(new SquareTile<T>());
                }
            }
        }

        public SquareTile<T> this[int x, int y]
        {
            get
            {
                return grid[x][y];
            }
            set
            {
                grid[x][y] = value;
            }
        }

        public SquareBoard<T> View()
        {
            SquareBoard<T> viewBoard = new SquareBoard<T>(name, length, width);
            for (int x = 0; x < length; x++)
            {                
                for (int y = 0; y < width; y++)
                {
                    viewBoard.grid[x][y] = grid[x][y].View();
                }
            }
            return viewBoard;

        }

        public SquareBoard<T> View(int x, int y, int xSize, int ySize)
        {
            SquareBoard<T> viewBoard = View();
            viewBoard.x = x;
            viewBoard.y = y;
            viewBoard.xSize = xSize;
            viewBoard.ySize = ySize;
            return viewBoard;
        }

        public void ClearSelection()
        {
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    grid[x][y].selectable = false;
                }
            }
        }
    }
}