using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Models
{
    public class MatchRequest
    {
        public int MatchRequestID { get; set; }
        public int PlayerId { get; set; }
        public int GameId { get; set; }
        public int NumberOfPlayers { get; set; }
        public DateTime RequestTime { get; set; }
    }
}