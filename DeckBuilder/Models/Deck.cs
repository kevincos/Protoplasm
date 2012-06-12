using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DeckBuilder.Models
{
    
    public class Deck
    {
        public int DeckID { get; set; }

        [Required(ErrorMessage = "A Deck Name is required")]
        [StringLength(160)]
        public string Name { get; set; }

        public virtual ICollection<CardSet> CardSets { get; set; }

        public int PlayerId { get; set; }
        public virtual Player Player { get; set; }
    }
}