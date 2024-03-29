﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity;
using SignalR.Hubs;
using DeckBuilder.Models;

namespace DeckBuilder.Async
{    
    public class ProposalPlayerStatus
    {
        public string name { get; set; }
        public bool ready { get; set; }        
    }

    public class Proposal
    {
        public static int latestID = 0;

        public int ProposalID { get; set; }
        public string gameName { get; set; }
        public int versionId {get; set;}
        public int hostIndex { get; set; }
        public List<ProposalPlayerStatus> players { get; set; }
        public List<string> withdrawnPlayers { get; set; }

        public Proposal(string hostPlayer, List<string> opponents, string gameName, int versionId)
        {
            this.ProposalID = latestID;
            latestID++;
            this.gameName = gameName.Replace("\n","");
            this.versionId = versionId;
            this.hostIndex = 0;
            players = new List<ProposalPlayerStatus>();
            players.Add(new ProposalPlayerStatus { name = hostPlayer, ready = true });
            foreach (string opponent in opponents)
            {
                players.Add(new ProposalPlayerStatus { name = opponent, ready = false });
            }
        }

        public bool Involves(string playerName)
        {
            foreach (ProposalPlayerStatus p in players)
            {
                if (p.name == playerName)
                    return true;
            }
            return false;
        }

        public List<String> PlayerNames
        {
            get
            {
                return players.Select(p => p.name).ToList();
            }
        }

        public void Cancel(string name)
        {
            players.RemoveAll(p => p.name == name);
            foreach (ProposalPlayerStatus player in players)
            {
                player.ready = false;
            }            
        }

        public void Confirm(string name)
        {
            foreach (ProposalPlayerStatus player in players)
            {
                if (player.name == name)
                {
                    player.ready = true;
                }
            }
        }

        public bool Final
        {
            get
            {
                foreach (ProposalPlayerStatus player in players)
                {
                    if (player.ready == false)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }

    public class LobbyChat : Hub, IDisconnect
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        public static Dictionary<string, int> activePlayers = new Dictionary<string,int>();
        public static Dictionary<string, string> connectionIdToName = new Dictionary<string, string>();        
        public static List<Proposal> activeProposals = new List<Proposal>();

        public void Reset()
        {
            activePlayers = new Dictionary<string,int>();            
            connectionIdToName = new Dictionary<string, string>();
        }

        public void EnterLobby(string data)
        {
            //Reset();

            Caller.name = data;
            Caller.updatePlayerlist(activePlayers.Keys);   
            
            AddToGroup("LOBBY_"+data);
            if (activePlayers.ContainsKey(data) == false)
                activePlayers.Add(data, 0);
            activePlayers[data]++;
            if(!connectionIdToName.Keys.Contains(Context.ConnectionId))
                connectionIdToName.Add(Context.ConnectionId, data);

            Clients.addPlayer(data);         
        }

        public List<Proposal> GetProposalsForPlayer(string player)
        {
            return activeProposals.Where(p => p.Involves(player)==true).ToList();
        }

        public void UpdateClients(Proposal p, string cancelledPlayer)
        {
            foreach (string name in p.PlayerNames)
            {
                List<Proposal> proposals = GetProposalsForPlayer(name);
                Clients["LOBBY_"+name].updateProposals(proposals);
            }
            if (cancelledPlayer != null)
            {
                List<Proposal> proposals = GetProposalsForPlayer(cancelledPlayer);
                Clients["LOBBY_"+cancelledPlayer].updateProposals(proposals);
            }
        }

        public void NewProposal(string hostPlayer, List<string> opponents, int versionId, string gameName)
        {

            Proposal p = new Proposal(hostPlayer, opponents, gameName, versionId);
            activeProposals.Add(p);
            UpdateClients(p,null);
            return;
        }


        public void CancelProposal(int id, string name)
        {
            Proposal proposal = activeProposals.Find(p => p.ProposalID == id);
            if (proposal != null)
            {
                proposal.Cancel(name);
                if (proposal.PlayerNames.Count < 2)
                {
                    activeProposals.Remove(proposal);
                }
                UpdateClients(proposal, name);
            }
            return;
        }

        public void ConfirmProposal(int id, string name)
        {
            Proposal proposal = activeProposals.Find(p => p.ProposalID == id);
            if (proposal != null)
            {
                proposal.Confirm(name);
                if (proposal.Final == true)
                {
                    // Create game
                    // NOTE: GAME CREATION DOES NOT BELONG IN LOBBY CODE

                    // Create Table
                    Table newTable = new Table();
                    newTable = db.Tables.Add(newTable);
                    //newTable.LastUpdateTime = DateTime.Now;
                    string gameName = proposal.gameName.Replace("*","");
                    newTable.Game = db.Games.Single(g => g.Name == gameName);
                    newTable.Version = newTable.Game.Versions.Single(v => v.GameVersionID == proposal.versionId);
                    newTable.TableState = (int)TableState.Proposed;
                    if (newTable.Version.DevStage == "Alpha")
                        newTable.Alpha = true;
                    db.SaveChanges();

                    // Create Seats
                    foreach (string playerName in proposal.PlayerNames)
                    {
                        Player p = db.Players.Where(pl => pl.Name == playerName).Single();
                        Seat s = new Seat
                        {
                            PlayerId = p.PlayerID,
                            TableId = newTable.TableID,
                            Accepted = true,
                            Waiting = false                            
                        };                        
                        db.Seats.Add(s);                        
                    }

                    db.SaveChanges();

                    newTable = db.Tables.Where(t => t.TableID == newTable.TableID).Single();
                    
                    newTable.GenerateInitialState();
                    db.SaveChanges();
                    

                    activeProposals.Remove(proposal);

                    // Redirect plyaers to game
                    foreach (string playerName in proposal.PlayerNames)
                    {
                        Clients["LOBBY_"+playerName].beginGame(newTable.TableID);
                    }                                                       
                }
                else
                {
                    UpdateClients(proposal,null);
                }
            }
            return;
        }

        public void Broadcast(string chatText)
        {
            
            // Invoke a method on the calling client
            //Clients.addMessage(Caller.name + ": " + data);

            foreach (String playerName in activePlayers.Keys)
            {
                Clients["LOBBY_"+playerName].addMessage(DateTime.Now.Hour + ":" + DateTime.Now.Minute + "  " + Context.User.Identity.Name + ": " + chatText);
            }
            //Clients.addMessage(DateTime.Now.Hour + ":" + DateTime.Now.Minute + "  " + Context.User.Identity.Name + ": " + chatText);
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