using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Models
{
    public class CardType
    {
        public int CardTypeId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public List<Card> Cards { get; set; }

    }
}