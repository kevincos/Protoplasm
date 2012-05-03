using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Models
{
    public class Game
    {
        public int GameID { get; set; }

        public string Name { get; set; }

        public string PythonScript { get; set; }

        public int MaxPlayers { get; set; }
    }
}