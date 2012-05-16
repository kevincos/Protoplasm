using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DeckBuilder.Models
{
    public class Player
    {        
        [Key]
        public int PlayerID { get; set; }

        [Required(ErrorMessage = "Player Name is required")]
        [Display(Name = "Name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(1024)]
        public string ProfileImageUrl { get; set; }

        public virtual ICollection<CardSet> CardSets { get; set; }
        public virtual ICollection<Deck> Decks { get; set; }
        public virtual ICollection<Table> Tables { get; set; }

        public virtual ICollection<Player> Friends { get; set; }
        public virtual ICollection<Table> ProposedGames { get; set; }
        public virtual ICollection<Table> ActiveGames { get; set; }
        public virtual ICollection<Table> CompletedGames { get; set; }

        public virtual ICollection<Game> CreatedGames { get; set; }

        
    }
}