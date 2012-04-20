using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;

namespace DeckBuilder.Protoplasm
{
    public class GameState
    {
        public List<PlayerContext> playerContexts { get; set; }
        
        public string state { get; set; }


        public void InitializeState(List<Seat> seats)
        {
        }
    }
}