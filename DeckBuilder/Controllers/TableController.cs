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
            ViewBag.state = new HtmlString(GetPythonView(table, pickledState, playerIndex));


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
            newTable.Version = db.Versions.Find(versionId);
            newTable.Game = newTable.Version.ParentGame;
            newTable.TableState = (int)TableState.Proposed;
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

        public string GetPythonView(Table table, string pickledState, int playerIndex)
        {
            PythonScriptEngine.InitScriptEngine();
            PythonScriptEngine.LoadModules(table.Version.ModuleName, table.Version.PythonScript);

            ScriptScope runScope = PythonScriptEngine.engine.CreateScope();
            runScope.ImportModule("cPickle");
            
            // Output state
            JSONContainer jsonContainer = new JSONContainer { json = "ERROR" };
            runScope.SetVariable("jsonContainer", jsonContainer);

            // Input state                                                                                                              
            runScope.SetVariable("inputState", pickledState);

            // Input playerId
            runScope.SetVariable("playerIndex", playerIndex);

            ScriptSource runSource = PythonScriptEngine.engine.CreateScriptSourceFromString("from encodings import hex_codec; import json; gameState = cPickle.loads(inputState);jsonString=json.dumps(gameState.view(playerIndex))", SourceCodeKind.Statements);

            for (int attempts = 0; attempts < 3; attempts++)
            {
                try
                {
                    runSource.Execute(runScope);
                    break;
                }
                catch (Exception e)
                {
                    PythonScriptEngine.errors.Add(e.ToString());
                    runSource.Execute(runScope);
                }
            }

            return runScope.GetVariable("jsonString");            
        }

        // Update
        [HttpPost]
        public ActionResult UpdateMain(int id, GameUpdate update)
        {
            DateTime start = DateTime.Now;
            Table table = db.Tables.Find(id);

            PythonScriptEngine.InitScriptEngine();
            PythonScriptEngine.LoadModules(table.Version.ModuleName, table.Version.PythonScript);

            ScriptScope runScope = PythonScriptEngine.engine.CreateScope();
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

            ScriptSource runSource = PythonScriptEngine.engine.CreateScriptSourceFromString("gameState = cPickle.loads(inputState);gameState.update(update);gameState.set_waiting_status(seats);game_over = gameState.game_over;finalState = cPickle.dumps(gameState)", SourceCodeKind.Statements);
            runSource.Execute(runScope);

            string final_pickledState = runScope.GetVariable("finalState");
            bool game_over = runScope.GetVariable("game_over");
            if (game_over == true)
            {
                if (table.TableState != (int)TableState.Complete && table.Version.DevStage == "Release")
                {
                    // Game just ended. Collect stats.
                    runSource = PythonScriptEngine.engine.CreateScriptSourceFromString("from encodings import hex_codec; import json; stats = json.dumps(gameState.stats())", SourceCodeKind.Statements);
                    runSource.Execute(runScope);
                    String latestStats = runScope.GetVariable("stats");
                    if (table.Version.StatLog == "" || table.Version.StatLog == null)
                        table.Version.StatLog = "[]";
                    table.Version.StatLog = table.Version.StatLog.Remove(table.Version.StatLog.Length - 1);
                    if(table.Version.StatLog.Length > 1)
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
                    string viewJson = GetPythonView(table, final_pickledState, i);
                    clients[s.Player.Name + id].main_updateGameState(viewJson);
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