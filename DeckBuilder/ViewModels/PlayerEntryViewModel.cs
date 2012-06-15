using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;

namespace DeckBuilder.ViewModels
{
    public class PlayerListViewModel
    {
        public List<PlayerEntryViewModel> players { get; set; }
        public int count { get; set; }

    }

    public class PlayerEntryViewModel
    {
        public int PlayerId { get; set; }

        public String Name { get; set; }
        public String ImageUrl { get; set; }
    }
}