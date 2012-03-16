using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace DeckBuilder.Models
{
    public class Card
    {
        [ScaffoldColumn(false)]
        public int CardID { get; set; }

        [Required(ErrorMessage = "Card name is required.")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mana cost required.")]
        [Range(0, 10)]
        public int ManaCost { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        [StringLength(1024)]
        public string CardArtUrl { get; set; }

        [DisplayName("Type")]
        public int? CardTypeId { get; set; }

        public int Extra { get; set; }

        public virtual CardType CardType { get; set; }
        public virtual ICollection<CardSet> CardSets { get; set; }


    }
}