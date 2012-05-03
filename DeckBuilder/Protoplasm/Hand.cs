using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Protoplasm
{
    public class GamePiece
    {
        public string url { get; set; }
        public string highlightUrl { get; set; }
        public string name { get; set; }        
        public bool selectable { get; set; }

        public GamePiece()
        {
        }

        public GamePiece View()
        {
            return new GamePiece {name = name, url = url, selectable = selectable, highlightUrl = highlightUrl };
        }
    }

    public class Hand<T> : GameObject where T:GamePiece, new()
    {
       public string name { get; set; }

       public List<T> cards { get; set; }

        public int x { get; set; }
        public int y { get; set; }
        public int cardX { get; set; }
        public int cardY { get; set; }

        public int selectedIndex { get; set; }

        public T SelectedCard
        {
            get
            {
                if (selectedIndex == -1)
                    return null;
                return cards[selectedIndex];
            }
        }

        public Hand()
        {
        }

        public T this[int i]
        {
            get
            {
                return cards[i];
            }
        }

        public Hand(string name)
        {
            this.type = "Hand";
            cards = new List<T>();
            this.name = name;
            selectedIndex = -1;
        }

        public void Add(T card)
        {
            cards.Add(card);
        }

        public void Add(string name, string url)
        {
            cards.Add(new T { name = name, url = url });
        }

        public void Deselect()
        {
            selectedIndex = -1;
        }

        public void ClearSelection()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].selectable = false;
                cards[i].highlightUrl = null;
            }
        }

        public void Select(int selectIndex)
        {
            selectedIndex = selectIndex;
        }

        public Hand<T> View()
        {
            Hand<T> viewHand = new Hand<T>(name);
            for (int i = 0; i < cards.Count; i++)
            {
                viewHand.cards.Add((T)cards[i].View());
            }
            viewHand.selectedIndex = selectedIndex;
            return viewHand;

        }


        public Hand<T> View(int x, int y, int cardX, int cardY)
        {
            Hand<T> viewHand = View();
            viewHand.x = x;
            viewHand.y = y;
            viewHand.cardX = cardX;
            viewHand.cardY = cardY;
            return viewHand;
        }
    }
}