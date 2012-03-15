using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public virtual ICollection<Seat> Seats { get; set; }

        public bool Finished { get; set; }

        // Rock Paper Scissors Data
        public int TotalTurns { get; set; }
        public int Draws { get; set; }
        public string Results { get; set; }
        public string FinalResults { get; set; }
    }
}