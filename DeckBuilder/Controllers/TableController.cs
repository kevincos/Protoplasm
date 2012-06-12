﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

using DeckBuilder.Models;
using SignalR.Infrastructure;
using SignalR;
using SignalR.Hosting.AspNet;
using System.IO.Compression;

using DeckBuilder.Helpers;
using DeckBuilder.Games;
using DeckBuilder.Async;
using DeckBuilder.Protoplasm;
using DeckBuilder.ViewModels;

using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Web.Hosting;
using DeckBuilder.Protoplasm_Python;

namespace DeckBuilder.Controllers
{ 

    public class TableController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        
        //
        // GET: /Table/

        public ViewResult Index()
        {
            return View(db.Tables.ToList());
        }

        //
        // GET: /Table/Details/5

        public ViewResult Details(int id)
        {
            Table table = db.Tables.Where(t => t.TableID == id).Include("Seats").Single();
            return View(table);
        }

        //
        // GET: /Table/Wait?matchRequestId=2312
        [Authorize]
        public ActionResult Wait(int matchRequestId)
        {            
            Player player = db.Players.Where(p => p.Name == User.Identity.Name).Single();
            ViewBag.WaitId = matchRequestId;
            ViewBag.PlayerName = player.Name;
            return View();
        }

        //
        // GET: /Table/Play/5
        [Authorize]
        public ActionResult Play(int id, int playerIndex=0)
        {
            Table table = db.Tables.Where(t => t.TableID == id).Include("Seats").Single();

            List<string> playerNames = table.Seats.Select(s => s.Player.Name).ToList();
            if (!playerNames.Contains(User.Identity.Name))
            {
                return RedirectToAction("Details", "Table", new { id = id });
            }

            Seat currentSeat = null;
            if (table.SoloPlayTest == true)                
                currentSeat = table.Seats.ElementAt(playerIndex);
            else
                for (int i = 0; i < table.Seats.Count; i++)
                {
                    Seat s = table.Seats.ElementAt(i);
                    if (s.Player.Name == User.Identity.Name)
                    {
                        playerIndex = i;
                        currentSeat = s;
                    }
                }
                
            // Unmark notification
            Notification existingNotification = db.Notifications.SingleOrDefault(n => n.PlayerID == currentSeat.PlayerId && n.TableID == id);
            if (existingNotification != null)
            {
                existingNotification.Read = true;                
                db.SaveChanges();
            }

            if (table.TableState == (int)TableState.Proposed)
            {
                currentSeat.Accepted = true;
                bool allAccepted = true;
                foreach (Seat s in table.Seats)
                {
                    if (s.Accepted == false)
                        allAccepted = false;
                }
                if (allAccepted == true)
                    table.TableState = (int)(TableState.InPlay);
                db.SaveChanges();
            }

            string pickledState = Compression.DecompressStringState(table.GameState);                
            ViewBag.state = new HtmlString(GetPythonView(table, pickledState, playerIndex,true).Item1);


            ViewBag.InitialChatData = new HtmlString(table.ChatRecord);
            ViewBag.game = table.Game.Name;
            ViewBag.TableId = table.TableID;
            ViewBag.PlayerIndex = playerIndex;
            ViewBag.PlayerName = currentSeat.Player.Name;
            ViewBag.AvailablePlayers = 0;
            if (table.SoloPlayTest == true)
                ViewBag.AvailablePlayers = table.Seats.Count;

            return View(table);
        }

        [Authorize]
        public ActionResult Challenge(int opponentId, int versionId)
        {
            Player player = db.Players.Where(p => p.Name == User.Identity.Name).Single();
            Player opponent = db.Players.Find(opponentId);
            // Create Table
            Table newTable = new Table();
            newTable = db.Tables.Add(newTable);
            //newTable.LastUpdateTime = DateTime.Now;
            newTable.Version = db.Versions.Find(versionId);
            newTable.Game = newTable.Version.ParentGame;
            newTable.TableState = (int)TableState.Proposed;
            if (newTable.Version.DevStage == "Alpha")
                newTable.Alpha = true;
            db.SaveChanges();

            // Create Seats
            Seat s1 = new Seat
            {
                PlayerId = player.PlayerID,
                TableId = newTable.TableID,                
                Accepted = true,
                Waiting = false
            };
            db.Seats.Add(s1);
            Seat s2 = new Seat
            {
                PlayerId = opponent.PlayerID,
                TableId = newTable.TableID,                
                Accepted = false,
                Waiting = false
            };
            db.Seats.Add(s2);

            db.SaveChanges();
            
            newTable = db.Tables.Where(t => t.TableID == newTable.TableID).Include("Seats.Deck.CardSets.Card").Single();

            newTable.GenerateInitialState();
            db.SaveChanges();


            foreach (Seat s in newTable.Seats)
            {                
                SeatViewModel viewModel = new SeatViewModel(s);
                String message = "";
                if(s.Accepted == true)
                    message = "You started a game of " + newTable.Game.Name + " with " + viewModel.formattedOpponentNames + ". (TableID:" + newTable.TableID + ") " + DateTime.Now;
                else
                    message = "A new game of " + newTable.Game.Name + " has been proposed with " + viewModel.formattedOpponentNames + ". (TableID:" + newTable.TableID + ") " + DateTime.Now;

                Notification n = new Notification { PlayerID = s.PlayerId, Message = message, TableID = newTable.TableID, DatePosted = DateTime.Now, Read = false, Url = "/Table/Play/" + newTable.TableID };
                db.Notifications.Add(n);
                NotificationsHub.UpdateNotifications(s.Player.Name,s.PlayerId);
                
            }
            db.SaveChanges();


            return RedirectToAction("Play", new { id = newTable.TableID });
        }

        public Tuple<string,string> GetPythonView(Table table, string pickledState, int playerIndex, bool force)
        {
            
            PythonScriptEngine.InitScriptEngine(table.Alpha || table.SoloPlayTest);
            PythonScriptEngine.LoadModules(table.Version.ModuleName, table.Version.PythonScript, table.Alpha || table.SoloPlayTest, table.Version.GameVersionID);

            ScriptScope runScope = PythonScriptEngine.GetScope(table.Alpha || table.SoloPlayTest);
            runScope.ImportModule("cPickle");
            
            // Output state
            JSONContainer jsonContainer = new JSONContainer { json = "ERROR" };
            runScope.SetVariable("jsonContainer", jsonContainer);

            // Input state                                                                                                              
            runScope.SetVariable("inputState", pickledState);

            // Input playerId
            runScope.SetVariable("playerIndex", playerIndex);

            if (force)
                PythonScriptEngine.ForceRunCode(runScope, "from encodings import hex_codec; import json; gameState = cPickle.loads(inputState);jsonString=json.dumps(gameState.view(playerIndex))", table.Alpha || table.SoloPlayTest, 3);
            else
            {
                string error = PythonScriptEngine.RunCode(runScope, "from encodings import hex_codec; import json; gameState = cPickle.loads(inputState);jsonString=json.dumps(gameState.view(playerIndex))", table.Alpha || table.SoloPlayTest);
                if(error != "")
                    return Tuple.Create<string, string>("", error);            

            }
            
            return Tuple.Create<string,string>(runScope.GetVariable("jsonString"),"");            
        }

        // Update
        [HttpPost, Authorize]
        public ActionResult UpdateMain(int id, GameUpdate update)
        {
            try
            {
                DateTime start = DateTime.Now;
                Table table = db.Tables.Find(id);
                Seat currentSeat = table.Seats.ElementAt(update.playerIndex);
                if (currentSeat.Player.Name != User.Identity.Name)
                    return RedirectToAction("Index","Home");

                PythonScriptEngine.InitScriptEngine(table.Alpha || table.SoloPlayTest);
                PythonScriptEngine.LoadModules(table.Version.ModuleName, table.Version.PythonScript, table.Alpha || table.SoloPlayTest,table.Version.GameVersionID);

                ScriptScope runScope = PythonScriptEngine.GetScope(table.Alpha || table.SoloPlayTest);
                runScope.ImportModule("cPickle");

                string init_pickledState = Compression.DecompressStringState(table.GameState);
                DateTime modulesLoaded = DateTime.Now;

                // PYTHON UPDATE

                // Input state                                                                                                              
                runScope.SetVariable("inputState", init_pickledState);
                runScope.SetVariable("update", update);
                // Input array of seats.
                Seat[] seatsArray = table.Seats.ToArray();
                runScope.SetVariable("seats", seatsArray);

                DateTime variablesSet = DateTime.Now;
                List<bool> previousWaitingStates = seatsArray.Select(s => s.Waiting).ToList();
                string errorString = PythonScriptEngine.RunCode(runScope, "gameState = cPickle.loads(inputState);gameState.update(update);gameState.set_waiting_status(seats);game_over = gameState.game_over;finalState = cPickle.dumps(gameState)", table.Alpha || table.SoloPlayTest);
                if (errorString != "")
                    return Content(errorString);

                for(int i =0; i < seatsArray.Length; i++)
                {
                    int targetPlayerId = seatsArray[i].PlayerId;
                    if (seatsArray[i].Waiting != previousWaitingStates[i] && seatsArray[i].Waiting == true)
                    {
                        // ADD NOTIFICATION
                        
                        Notification existingNotification = db.Notifications.FirstOrDefault(n => n.PlayerID == targetPlayerId && n.TableID == table.TableID);
                        SeatViewModel viewModel = new SeatViewModel(seatsArray[i]);
                        String newMessage = "Your turn in " + table.Game.Name + " with " + viewModel.formattedOpponentNames + ". (TableID:"+table.TableID+") " + DateTime.Now;
                        
                        if (existingNotification == null)
                        {
                            Notification n = new Notification { PlayerID = seatsArray[i].PlayerId, Message = newMessage, TableID = table.TableID, DatePosted = DateTime.Now, Read = false, Url = "/Table/Play/" + table.TableID };
                            db.Notifications.Add(n);
                        }
                        else
                        {
                            existingNotification.DatePosted = DateTime.Now;
                            existingNotification.Read = false;
                            existingNotification.Suppressed = false;
                            existingNotification.Url = "/Table/Play/" + table.TableID;
                            existingNotification.Message = newMessage;
                        }
                        NotificationsHub.UpdateNotifications(seatsArray[i].Player.Name, seatsArray[i].PlayerId);                            
                    }
                    else if (seatsArray[i].Waiting != previousWaitingStates[i] && seatsArray[i].Waiting == false)
                    {
                        Notification existingNotification = db.Notifications.SingleOrDefault(n => n.PlayerID == targetPlayerId && n.TableID == id);
                        if (existingNotification != null)
                        {
                            existingNotification.Read = true;
                            existingNotification.Suppressed = true;
                            NotificationsHub.UpdateNotifications(seatsArray[i].Player.Name, seatsArray[i].PlayerId);                            
                            db.SaveChanges();
                        }
                    }
                    else if (seatsArray[i].Waiting == true)
                    {
                        Notification existingNotification = db.Notifications.SingleOrDefault(n => n.PlayerID == targetPlayerId && n.TableID == id);
                        if (existingNotification != null)
                        {
                            existingNotification.Read = true;
                            NotificationsHub.UpdateNotifications(seatsArray[i].Player.Name, seatsArray[i].PlayerId);                            
                            db.SaveChanges();
                        }
                    }
                }

                string final_pickledState = runScope.GetVariable("finalState");
                bool game_over = runScope.GetVariable("game_over");
                if (game_over == true)
                {
                    if (table.TableState != (int)TableState.Complete && table.Version.DevStage == "Release")
                    {
                        // Log player wins/losses
                        foreach (Seat s in table.Seats)
                        {
                            Player p = s.Player;
                            if (p.Records == null) p.Records = new List<Record>();
                            // Find record if it exists. Otherwise create one.
                            Record r = p.Records.FirstOrDefault(record => record.GameId == table.GameId);
                            if (r == null)
                            {
                                r = new Record();
                                r.GameId = table.GameId;
                                r.PlayerId = p.PlayerID;
                                p.Records.Add(r);
                            }
                            if (table.Ranked == true)
                            {
                                r.RankedGamesPlayed++;
                                if (s.Result == "Win")
                                    r.RankedWins++;
                                if (s.Result == "Loss")
                                    r.RankedLosses++;
                                if (s.Result == "Draw")
                                    r.RankedDraws++;
                            }
                            else
                            {
                                r.GamesPlayed++;
                                if (s.Result == "Win")
                                    r.Wins++;
                                if (s.Result == "Loss")
                                    r.Losses++;
                                if (s.Result == "Draw")
                                    r.Draws++;
                            }

                        }
                        // Game just ended. Collect stats.
                        errorString = PythonScriptEngine.RunCode(runScope, "from encodings import hex_codec; import json; stats = json.dumps(gameState.stats())", table.Alpha || table.SoloPlayTest);
                        if (errorString != "")
                            return Content(errorString);
                        String latestStats = runScope.GetVariable("stats");
                        if (table.Version.StatLog == "" || table.Version.StatLog == null)
                            table.Version.StatLog = "[]";
                        table.Version.StatLog = table.Version.StatLog.Remove(table.Version.StatLog.Length - 1);
                        if (table.Version.StatLog.Length > 1)
                            table.Version.StatLog += ",";
                        table.Version.StatLog += latestStats + "]";

                        // Notify players of game end.
                        foreach (Seat s in seatsArray)
                        {
                            Notification existingNotification = db.Notifications.SingleOrDefault(n => n.PlayerID == s.PlayerId && n.TableID == id);
                            if (existingNotification != null)
                            {
                                existingNotification.Suppressed = false;
                                if(s.PlayerId==currentSeat.PlayerId)
                                    existingNotification.Read = true;
                                else
                                    existingNotification.Read = false;
                                SeatViewModel viewModel = new SeatViewModel(s);
                                existingNotification.Message = "Game Over! ";
                                if (s.Result == "Win")
                                    existingNotification.Message = "Victory! ";                                
                                if(s.Result == "Loss")
                                    existingNotification.Message = "Defeat! ";
                                if (s.Result == "Draw")
                                    existingNotification.Message = "Draw! ";
                                existingNotification.Message += table.Game.Name + " game with " + viewModel.formattedOpponentNames + " has ended. (TableID:" + table.TableID + ") " + DateTime.Now;
                                db.SaveChanges();
                            }
                        }

                    }
                    table.TableState = (int)TableState.Complete;
                }
                table.GameState = Compression.CompressStringState(final_pickledState);

                DateTime scriptsExecuted = DateTime.Now;

                db.SaveChanges();

                DateTime databaseSaved = DateTime.Now;

                // Send updated states to clients
                IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
                dynamic clients = connectionManager.GetClients<GameList>();
                for (int i = 0; i < table.Seats.Count; i++)
                {
                    Seat s = table.Seats.ElementAt(i);
                    if (table.SoloPlayTest == false || s.Waiting == true)
                    {
                        Tuple<string,string> viewInfo = GetPythonView(table, final_pickledState, i,false);
                        if(viewInfo.Item2 != "")
                            return Content(viewInfo.Item2);
                        clients[s.Player.Name + id].main_updateGameState(viewInfo.Item1);
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan totalTime = end.Subtract(start);
                TimeSpan moduleLoadTime = modulesLoaded.Subtract(start);
                TimeSpan variablesSetTime = variablesSet.Subtract(modulesLoaded);
                TimeSpan scriptsExecutedTime = scriptsExecuted.Subtract(variablesSet);
                TimeSpan databaseSavedTime = databaseSaved.Subtract(scriptsExecuted);
                TimeSpan updateTime = end.Subtract(databaseSaved);
                return View();
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }
        }

        //
        // GET: /Table/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Table/Create

        [HttpPost]
        public ActionResult Create(Table table)
        {
            if (ModelState.IsValid)
            {
                db.Tables.Add(table);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(table);
        }
        
        //
        // GET: /Table/Edit/5
 
        public ActionResult Edit(int id)
        {
            Table table = db.Tables.Find(id);
            return View(table);
        }

        //
        // POST: /Table/Edit/5

        [HttpPost]
        public ActionResult Edit(Table table)
        {
            if (ModelState.IsValid)
            {
                db.Entry(table).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(table);
        }

        public ActionResult Remove(int id)
        {            
            Player player = db.Players.Where(p => p.Name == User.Identity.Name).Single();
            Table table = db.Tables.Find(id);
            table.Seats.First(s => s.PlayerId == player.PlayerID).Removed = true;

            bool destroyTable = true;
            foreach (Seat s in table.Seats)
            {
                if (s.Removed == false)
                    destroyTable = false;
            }            

            if (destroyTable == true || table.SoloPlayTest == true)
                db.Tables.Remove(table);
            db.SaveChanges();

            // Sets seat to Removed. If both seats are Removed, delete table.
            return RedirectToAction("Profile", "Home");
        }

        public ActionResult Cancel(int id)
        {
            // Sets table to Cancelled and Removes it from current player's list.
            Player player = db.Players.Where(p => p.Name == User.Identity.Name).Single();
            Table table = db.Tables.Find(id);
            Seat seat = table.Seats.First(s => s.PlayerId == player.PlayerID);
            if (seat.Accepted == false)
            {
                table.TableState = (int)TableState.Cancelled;
                seat.Removed = true;
            }
            db.SaveChanges();
            return RedirectToAction("Profile", "Home");
        }

        public ActionResult Forfeit(int id)
        {
            // Sets table to completed.
            Player player = db.Players.Where(p => p.Name == User.Identity.Name).Single();
            Table table = db.Tables.Find(id);
            Seat seat = table.Seats.First(s => s.PlayerId == player.PlayerID);
            if (table.Seats.Count == 2)
            {
                table.TableState = (int)TableState.Complete;
                seat.Removed = true;
            }
            db.SaveChanges();

            return RedirectToAction("Profile", "Home");
        }

        //
        // GET: /Table/Delete/5
 
        public ActionResult Delete(int id)
        {
            Table table = db.Tables.Find(id);
            return View(table);
        }

        //
        // POST: /Table/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Table table = db.Tables.Find(id);
            db.Tables.Remove(table);


            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}