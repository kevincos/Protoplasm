using System;
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
                DeckId = db.Decks.First().DeckID,
                Accepted = true,
                Waiting = false
            };
            db.Seats.Add(s1);
            Seat s2 = new Seat
            {
                PlayerId = opponent.PlayerID,
                TableId = newTable.TableID,
                DeckId = db.Decks.First().DeckID,
                Accepted = false,
                Waiting = false
            };
            db.Seats.Add(s2);

            db.SaveChanges();

            newTable = db.Tables.Where(t => t.TableID == newTable.TableID).Include("Seats.Deck.CardSets.Card").Single();

            newTable.GenerateInitialState();
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
        [HttpPost]
        public ActionResult UpdateMain(int id, GameUpdate update)
        {
            try
            {
                DateTime start = DateTime.Now;
                Table table = db.Tables.Find(id);

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

                string errorString = PythonScriptEngine.RunCode(runScope, "gameState = cPickle.loads(inputState);gameState.update(update);gameState.set_waiting_status(seats);game_over = gameState.game_over;finalState = cPickle.dumps(gameState)", table.Alpha || table.SoloPlayTest);
                if (errorString != "")
                    return Content(errorString);

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

            if (destroyTable == true)
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