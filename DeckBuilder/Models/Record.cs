using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Models
{
    public class Record
    {
        public int RecordID { get; set; }

        public int PlayerId { get; set; }
        public virtual Player Player { get; set; }

        public int GameId { get; set; }
        public virtual Game Game { get; set; }

        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }

        public int RankedGamesPlayed { get; set; }
        public int RankedWins { get; set; }
        public int RankedDraws { get; set; }
        public int RankedLosses { get; set; }
    }
}