using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DeckBuilder.Models
{
    public class CardSet
    {
        public int CardSetID { get; set; }

        public int CardID { get; set; }
        public virtual Card Card { get; set; }
        
        public int? PlayerID { get; set; }
        public virtual Player Player { get; set; }

        public int? DeckID { get; set; }
        public virtual Deck Deck { get; set; }
        
        public int Quantity { get; set; }
    }
}