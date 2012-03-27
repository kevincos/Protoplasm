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

        // Summon Unit Info

        // Summon Crystal Info
        [StringLength(50)]
        public string Crystal_Name { get; set; }
        [StringLength(1024)]
        public string Crystal_Url { get; set; }
        public int Crystal_Range { get; set; }
        public int Crystal_Mana { get; set; }

        // Summon Unit Info
        [StringLength(50)]
        public string Unit_Name { get; set; }
        [StringLength(1024)]
        public string Unit_Url { get; set; }
        public int Unit_MaxHP { get; set; }
        public int Unit_Attack { get; set; }
        public int Unit_Defense { get; set; }
        public int Unit_Speed { get; set; }
        public string Unit_Awareness { get; set; }

        //public int extra { get; set; }

        [DisplayName("Type")]
        public int? CardTypeId { get; set; }

        public virtual CardType CardType { get; set; }
        public virtual ICollection<CardSet> CardSets { get; set; }


    }
}