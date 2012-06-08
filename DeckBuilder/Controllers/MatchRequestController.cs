using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeckBuilder.Models;
using DeckBuilder.ViewModels;
using DeckBuilder.Async;

using SignalR.Infrastructure;
using SignalR;
using SignalR.Hosting.AspNet;

namespace DeckBuilder.Controllers
{
    public class MatchRequestController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        public ActionResult Add(int gameId, int numPlayers)
        {                                    
            Player player = db.Players.Where(p => p.Name == User.Identity.Name).Single();

            
            // Search database for earliest request with same gameId and numberOfPlayers, but different  playerId
            List<MatchRequest> successfulMatches = db.MatchRequests.OrderBy(matchRequest => matchRequest.RequestTime).Where(matchRequest => matchRequest.GameId == gameId && matchRequest.NumberOfPlayers == numPlayers && matchRequest.PlayerId != player.PlayerID).ToList();
            if (successfulMatches.Count < numPlayers-1)
            {
                MatchRequest m = new MatchRequest { GameId = gameId, NumberOfPlayers = numPlayers, PlayerId = player.PlayerID, RequestTime = DateTime.Now };
                db.MatchRequests.Add(m);
                db.SaveChanges();
                return RedirectToAction("Wait", "Table", new { MatchRequestId = m.MatchRequestID });
            }
            else
            {
                successfulMatches = successfulMatches.GetRange(0, numPlayers-1);
                
                Game game = db.Games.Find(gameId);

                Table newTable = new Table();
                newTable = db.Tables.Add(newTable);
                //newTable.LastUpdateTime = DateTime.Now;
                newTable.Version = db.Versions.Find(game.LatestRelease.GameVersionID);
                if (newTable.Version.DevStage == "Alpha")
                    newTable.Alpha = true;
                newTable.Game = newTable.Version.ParentGame;
                newTable.TableState = (int)TableState.Proposed;
                db.SaveChanges();

                // Create Seats
                
                Seat s = new Seat
                {
                    PlayerId = player.PlayerID,
                    TableId = newTable.TableID,
                    DeckId = db.Decks.First().DeckID,
                    Accepted = true,
                    Waiting = false
                };
                db.Seats.Add(s);
                foreach (MatchRequest match in successfulMatches)
                {
                    Seat s2 = new Seat
                    {
                        PlayerId = match.PlayerId,
                        TableId = newTable.TableID,
                        DeckId = db.Decks.First().DeckID,
                        Accepted = true,
                        Waiting = false
                    };
                    db.Seats.Add(s2);
                }

                db.SaveChanges();

                newTable = db.Tables.Where(t => t.TableID == newTable.TableID).Include("Seats.Deck.CardSets.Card").Include("Seats.Player").Single();
                newTable.Ranked = true;
                newTable.TableState = (int)TableState.InPlay;

                newTable.GenerateInitialState();

                foreach (Seat seat in newTable.Seats)
                {
                    SeatViewModel viewModel = new SeatViewModel(seat);
                    String message = "";
                    if (seat.Waiting == true)
                        message = "Your turn in " + newTable.Game.Name + " with " + viewModel.formattedOpponentNames + ". (Ranked) " + DateTime.Now;
                    else
                        message = "A new game of " + newTable.Game.Name + " has been started with " + viewModel.formattedOpponentNames + ". (Ranked) " + DateTime.Now;

                    Notification n = new Notification { PlayerID = seat.PlayerId, Message = message, TableID = newTable.TableID, DatePosted = DateTime.Now, Read = false, Url = "/Table/Play/" + newTable.TableID };
                    db.Notifications.Add(n);
                    NotificationsHub.UpdateNotifications(seat.Player.Name, seat.PlayerId);

                }
                db.SaveChanges();

                IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
                dynamic clients = connectionManager.GetClients<WaitingArea>();
                
                foreach (MatchRequest match in successfulMatches)
                {
                    Player opponent = db.Players.Find(match.PlayerId);
                    clients[opponent.Name + match.MatchRequestID].goToTable(newTable.TableID);
                    db.MatchRequests.Remove(match);
                }
                
                db.SaveChanges();
                return RedirectToAction("Play", "Table", new { id = newTable.TableID });
            }
            
        }

        public ViewResult Index()
        {
            return View(db.MatchRequests.ToList());
        }        
    }
}
