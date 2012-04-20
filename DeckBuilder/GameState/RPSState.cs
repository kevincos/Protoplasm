using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;

namespace DeckBuilder.Games
{
    public class RPSPlayerContext
    {
        public List<string> moveHistory { get; set; }
        public string currentMove { get; set; }
        public int wins { get; set; }

        public int playerId { get; set; }
        public string name { get; set; }

        public RPSPlayerContext()
        {
            moveHistory = new List<string>();
            currentMove = "None";
        }
    }

    public class RPSState
    {
        public List<RPSPlayerContext> playerContexts { get; set; }
        public int currentRound { get; set; }
        public int draws { get; set; }
        public int sourcePlayerId { get; set; }
        public int tableId { get; set; }

        public RPSState()
        {
            playerContexts = new List<RPSPlayerContext>();
            currentRound = 0;
            draws = 0;
        }

        public void InitializeState(List<Seat> seats)
        {
            tableId = seats[0].TableId;
            foreach (Seat seat in seats)
            {
                RPSPlayerContext playerContext = new RPSPlayerContext();
                playerContext.playerId = seat.PlayerId;
                playerContext.name = seat.Player.Name;
                playerContext.wins = 0;
                playerContext.currentMove = "None";
                playerContext.moveHistory = new List<string>();
                playerContexts.Add(playerContext);
            }
        }

        public void Update(RPSState inputState)
        {
            bool finished = true;
            for (int i = 0; i < this.playerContexts.Count; i++)
            {
                if (this.playerContexts[i].playerId == inputState.sourcePlayerId)
                {
                    this.playerContexts[i].currentMove = inputState.playerContexts[i].currentMove;
                }
                if (this.playerContexts[i].currentMove == "None") finished = false;
            }

            if (finished)
            {
                string p1move = this.playerContexts[0].currentMove;
                string p2move = this.playerContexts[1].currentMove;
                this.playerContexts[0].moveHistory.Add(p1move);
                this.playerContexts[1].moveHistory.Add(p2move);
                this.currentRound++;
                if (p1move == p2move)
                {
                    // Draw
                    this.draws++;                                        
                }
                else if (p1move == "Rock" && p2move == "Scissors" || p1move == "Scissors" && p2move == "Paper" || p1move == "Paper" && p2move == "Rock")
                {
                    // P1 wins
                    this.playerContexts[0].wins++;
                }
                else
                {
                    // P2 wins
                    this.playerContexts[1].wins++;
                }
                this.playerContexts[0].currentMove = "None";
                this.playerContexts[1].currentMove = "None";
            }
            //if(this.playerContexts[0].wins == 3 || this.playerContexts[1].wins == 3) GAME END
        }

        public RPSState GetClientState(int playerId)
        {
            RPSState clientState = new RPSState();
            clientState.sourcePlayerId = playerId;
            clientState.currentRound = this.currentRound;
            clientState.tableId = this.tableId;
            clientState.draws = this.draws;
            foreach (RPSPlayerContext playerContext in this.playerContexts)
            {
                RPSPlayerContext clientContext = new RPSPlayerContext();
                clientContext.playerId = playerContext.playerId;
                clientContext.moveHistory = playerContext.moveHistory;
                clientContext.wins = playerContext.wins;
                clientContext.name = playerContext.name;
                if (playerId == playerContext.playerId)
                {
                    clientContext.currentMove = playerContext.currentMove;
                }
                else if (playerContext.currentMove != "None")
                {
                    clientContext.currentMove = "Unknown";
                }
                clientState.playerContexts.Add(clientContext);
            }
            return clientState;
        }
    }
}