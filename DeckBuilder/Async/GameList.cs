using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SignalR.Hubs;
using DeckBuilder.Models;

namespace DeckBuilder.Async
{
    public class GameList : Hub
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        public void EnterGame(string name)
        {
            Caller.name = name;
            AddToGroup(name);
        }

        // Logs move into database, notifies opponent it is their turn
        public void SubmitMove(int tableId, string playerName, string move)
        {
            // Find table
            Table currentTable = db.Tables.Find(tableId);

            // Check if it's the table is waiting on this player's input
            Seat mySeat = currentTable.Seats.Where(s => s.Player.Name == playerName).Single();
            if (mySeat.Active == false)
            {
                Caller.refreshGame();
                return;
            }

            // Update table model
            mySeat.Active = false;
            mySeat.LastMove = move;
            db.SaveChanges();
            
            // Notify players to refresh if all players are done
            if (currentTable.Seats.Where(s => s.Active == true).Count() == 0)
            {
                // NOTE: SCORING LOGIC RELIES ON EXACTLY 2 PLAYERS
                Seat player1 = currentTable.Seats.ElementAt(0);
                Seat player2 = currentTable.Seats.ElementAt(1);
                
                String resultMessage = "Results from Round " + currentTable.TotalTurns + ":";

                
                if (player1.LastMove == "Rock" && player2.LastMove == "Scissors")
                {
                    resultMessage = player1.Player.Name + "'s Rock smashes " + player2.Player.Name + "'s Scissors!";
                    player1.Wins++;
                }
                else if (player1.LastMove == "Paper" && player2.LastMove == "Rock")
                {
                    resultMessage = player1.Player.Name + "'s Paper smothers " + player2.Player.Name + "'s Rock!";
                    player1.Wins++;
                }
                else if (player1.LastMove == "Scissors" && player2.LastMove == "Paper")
                {
                    resultMessage = player1.Player.Name + "'s Scissors cut " + player2.Player.Name + "'s Paper!";
                    player1.Wins++;
                }
                else if (player2.LastMove == "Rock" && player1.LastMove == "Scissors")
                {
                    resultMessage = player2.Player.Name + "'s Rock smashes " + player1.Player.Name + "'s Scissors!";
                    player2.Wins++;
                }
                else if (player2.LastMove == "Paper" && player1.LastMove == "Rock")
                {
                    resultMessage = player2.Player.Name + "'s Paper smothers " + player1.Player.Name + "'s Rock!";
                    player2.Wins++;
                }
                else if (player2.LastMove == "Scissors" && player1.LastMove == "Paper")
                {
                    resultMessage = player2.Player.Name + "'s Scissors cut " + player1.Player.Name + "'s Paper!";
                    player2.Wins++;
                }
                else
                {
                    resultMessage = "Draw! Both players played " + player1.LastMove;
                    currentTable.Draws++;
                }

                if (player1.Wins == 3 || player2.Wins == 3)
                {
                    currentTable.Finished = true;
                    player1.Player.gamesPlayed++;
                    player2.Player.gamesPlayed++;
                    if (player1.Wins > player2.Wins)
                    {
                        player1.Player.gamesWon++;
                        currentTable.FinalResults = player1.Player.Name + " won with a score of " + player1.Wins + " to " + player2.Wins + ".";

                    }
                    else
                    {
                        player2.Player.gamesWon++;                        
                        currentTable.FinalResults = player2.Player.Name + " won with a score of " + player2.Wins + " to " + player1.Wins + ".";
                    }
                }

                currentTable.TotalTurns++;
                currentTable.Results = resultMessage;

                List<Seat> seats = currentTable.Seats.ToList();
                foreach (Seat seatToUpdate in seats)
                {
                    seatToUpdate.Active = true;
                    mySeat.LastMove = "";
                }
                db.SaveChanges();
                foreach (Seat seatToUpdate in seats)
                {
                    Clients[seatToUpdate.Player.Name].refreshGame();
                }
            }
            else
                Caller.refreshGame();
        }
    }
}