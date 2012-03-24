using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SignalR.Hubs;
using DeckBuilder.Models;

namespace DeckBuilder.Async
{
    public class GameProposal
    {
        public String player1;
        public bool player1Confirmed = false;
        public int player1Deck;
        

        public String player2;
        public bool player2Confirmed = false;
        public int player2Deck;

        public bool InvolvesPlayer(string player)
        {
            if (player1 == player || player2 == player)
                return true;
            return false;
        }

        public bool InvolvesPlayers(string p1, string p2)
        {
            return InvolvesPlayer(p1) && InvolvesPlayer(p2);
        }

        public void ConfirmPlayer(string player, int deckId)
        {
            if (player1 == player)
            {
                player1Confirmed = true;
                player1Deck = deckId;
            }
            if (player2 == player) 
            {
                player2Confirmed = true;
                player2Deck = deckId;
            }
        }

        public int GetDeckId(string player)
        {
            if (player == player1)
                return player1Deck;
            if (player == player2)
                return player2Deck;
            return 0;
        }

        public bool IsConfirmed()
        {
            return (player1Confirmed && player2Confirmed);
        }
    }

    public class LobbyChat : Hub, IDisconnect
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        public static Dictionary<string, int> activePlayers = new Dictionary<string,int>();
        public static Dictionary<string, string> connectionIdToName = new Dictionary<string, string>();
        public static List<GameProposal> proposals = new List<GameProposal>();

        public void Reset()
        {
            activePlayers = new Dictionary<string,int>();
            proposals = new List<GameProposal>();
            connectionIdToName = new Dictionary<string, string>();
        }

        public void EnterLobby(string data)
        {
            //Reset();

            Caller.name = data;
            Caller.updatePlayerlist(activePlayers.Keys);   

            AddToGroup(data);
            if (activePlayers.ContainsKey(data) == false)
                activePlayers.Add(data, 0);
            activePlayers[data]++;
            connectionIdToName.Add(Context.ConnectionId, data);

            Clients.addPlayer(data);         
        }



        public void ProposeGame(string opponent)
        {
            if (activePlayers.ContainsKey(opponent))
            {
                GameProposal proposal = new GameProposal();
                proposal.player1 = Caller.name;
                proposal.player2 = opponent;
                bool proposalExists = false;
                foreach (GameProposal p in proposals)
                {
                    if (p.InvolvesPlayers(Caller.name, opponent))
                        proposalExists = true;
                }
                if(proposalExists == false)
                    proposals.Add(proposal);

                Clients[opponent].proposalNotification(Caller.name);
            }
        }

        public void Confirm(string opponent, int deckId)
        {
            foreach (GameProposal p in proposals)
            {
                if (p.InvolvesPlayers(opponent, Caller.name))
                {
                    p.ConfirmPlayer(Caller.name, deckId);
                    if (p.IsConfirmed())
                    {
                        // NOTE: GAME CREATION DOES NOT BELONG IN LOBBY CODE

                        // Create Table
                        Table newTable = new Table();                        
                        newTable = db.Tables.Add(newTable);
                        db.SaveChanges();

                        // Create Seats
                        String yourName = Caller.name;
                        Player p1 = db.Players.Where(pl => pl.Name == yourName).Single();
                        Player p2 = db.Players.Where(pl => pl.Name == opponent).Single();
                        
                        Seat s1 = new Seat
                        {
                            PlayerId = p1.PlayerID,
                            DeckId = p.GetDeckId(yourName),
                            TableId = newTable.TableID,
                            Active = true
                        };
                        Seat s2 = new Seat
                        {
                            PlayerId = p2.PlayerID,
                            DeckId = p.GetDeckId(opponent),
                            TableId = newTable.TableID,
                            Active = true
                        };
                        db.Seats.Add(s1);
                        db.Seats.Add(s2);
                        db.SaveChanges();

                        newTable.GenerateInitialState();
                        db.SaveChanges();

                        proposals.Remove(p);                                     

                        // Redirect plyaers to game
                        Clients[opponent].beginGame(newTable.TableID);
                        Clients[Caller.name].beginGame(newTable.TableID);
                        return;
                    }                    
                }
            }
        }

        public void Broadcast(string data)
        {
            
            // Invoke a method on the calling client
            Clients.addMessage(Caller.name + ": " + data);            
        }

        public void LeaveLobby()
        {
            try
            {
                RemoveFromGroup(Caller.name);
                activePlayers[Caller.name]--;
                if (activePlayers[Caller.name] == 0)
                {
                    activePlayers.Remove(Caller.name);
                    Broadcast(Caller.name + " has left.");
                    Clients.removePlayer(Caller.name);
                }
                connectionIdToName.Remove(Context.ConnectionId);
            }
            catch (Exception) { }
        }

        public Task Disconnect()
        {
            try
            {
                string name = connectionIdToName[Context.ConnectionId];
                RemoveFromGroup(name);
                activePlayers[name]--;
                if (activePlayers[name] == 0)
                {
                    activePlayers.Remove(name);
                    Broadcast(name + " has disconnected.");
                }
                connectionIdToName.Remove(Context.ConnectionId);
            }
            catch (Exception) { }
            return null;
        }
        
    }
}