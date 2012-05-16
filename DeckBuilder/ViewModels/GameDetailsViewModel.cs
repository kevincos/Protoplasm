using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DeckBuilder.Models;

namespace DeckBuilder.ViewModels
{
    public class GameDetailsViewModel
    {
        public int GameId { get; set; }

        public String Name { get; set; }
        public Player Creator { get; set; }

        public List<DeckBuilder.Models.GameVersion> Versions { get; set; }
    }
}