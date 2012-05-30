using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DeckBuilder.Models;

namespace DeckBuilder.ViewModels
{
    public class RankViewModel
    {
        public String Name { get; set; }
        public int GamesPlayed { get; set; }        
        public int WinRate { get; set; }
    }

    public class GameDetailsViewModel
    {
        public int GameId { get; set; }

        public String Name { get; set; }
        public Player Creator { get; set; }

        public List<DeckBuilder.Models.GameVersion> Versions { get; set; }

        public List<RankViewModel> Rankings { get; set; }

        public void SetRankings(ICollection<Record> records)
        {
            Rankings = records.OrderByDescending(r => r.RankedWins * 100 / r.RankedGamesPlayed).Select(r => new RankViewModel { Name = r.Player.Name, GamesPlayed = r.RankedGamesPlayed, WinRate = r.RankedWins * 100 / r.RankedGamesPlayed }).ToList();
        }
    }
}