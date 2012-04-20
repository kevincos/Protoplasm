using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;

namespace DeckBuilder.Games
{
    public class TemplatePlayerContext
    {
        public string playerInfo { get; set; }
        public int playerId { get; set; }
        public string name { get; set; }
    }

    public class TemplateUpdate
    {
        public int playerId { get; set; }
        public string updateData { get; set; }

    }

    public class TemplateState
    {
        public List<TemplatePlayerContext> playerContexts { get; set; }
        public int sourcePlayerId { get; set; }
        public int activePlayerId { get; set; }
        public int activePlayerIndex { get; set; }
        public int tableId { get; set; }
        public bool gameOver { get; set; }


        public void InitializeState(List<Seat> seats)
        {
            tableId = seats[0].TableId;
            playerContexts = new List<TemplatePlayerContext>();
            foreach (Seat seat in seats)
            {
                TemplatePlayerContext playerContext = new TemplatePlayerContext();
                playerContext.playerId = seat.PlayerId;
                playerContext.name = seat.Player.Name;
                playerContexts.Add(playerContext);
            }
            playerContexts[0].playerInfo = "Black";
            playerContexts[1].playerInfo = "Red";
            activePlayerIndex = 0;
            activePlayerId = playerContexts[activePlayerIndex].playerId;            
        }

        public void Update(TemplateUpdate update)
        {
        }

        public TemplateState GetClientState(int playerId)
        {
            this.sourcePlayerId = playerId;
            return this;
        }
    }


}