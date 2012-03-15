using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Models
{
    public class Seat
    {
        public int SeatID { get; set; }

        public int PlayerId { get; set; }
        public virtual Player Player { get; set; }

        public int DeckId { get; set; }
        public virtual Deck Deck { get; set; }

        public int TableId { get; set; }
        public virtual Table Tables { get; set; }

        public bool Active { get; set; }

        // Rock Paper Scissors Data
        public int Wins { get; set; }
        public string LastMove { get; set; }        
    }
}