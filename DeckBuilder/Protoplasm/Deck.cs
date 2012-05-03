using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Protoplasm
{
    public class Deck<T> where T : GamePiece
    {
        public List<T> cards { get; set; }

        public Deck()
        {
            cards = new List<T>();
        }

        public void Shuffle()
        {
            Random r = new Random();
            List<T> shuffledDeck = new List<T>();
            while (cards.Count > 0)
            {
                int index = r.Next(0, cards.Count);
                shuffledDeck.Add(cards.ElementAt(index));
                cards.RemoveAt(index);
            }
            cards = shuffledDeck;
        }

        public T Draw()
        {
            if (cards.Count == 0)
                return null;
            T drawCard = cards[0];
            cards.RemoveAt(0);
            return drawCard;
        }

        public T DrawWithDiscard(Pile<T> discard)
        {
            if (cards.Count == 0)
            {
                cards.AddRange(discard.cards);
                if (cards.Count == 0)
                    return null;
                discard.cards.Clear();
                Shuffle();
            }
            T drawCard = cards[0];
            cards.RemoveAt(0);
            return drawCard;
        }

        public void Add(T piece)
        {
            cards.Add(piece);
        }
    }

    public class Pile<T> where T : GamePiece
    {
        public List<T> cards { get; set; }

        public Pile()
        {
            cards = new List<T>();
        }

        public void Add(T piece)
        {
            cards.Add(piece);
        }
    }
}