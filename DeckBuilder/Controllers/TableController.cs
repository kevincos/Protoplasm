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
        public ActionResult Play(int id)
        {
            Table table = db.Tables.Where(t => t.TableID == id).Include("Seats").Single();

            List<string> playerNames = table.Seats.Select(s => s.Player.Name).ToList();
            if (!playerNames.Contains(User.Identity.Name))
            {
                return RedirectToAction("Details", "Table", new { id = id });
            }

            Seat currentSeat = table.Seats.Where(s => s.Player.Name == User.Identity.Name).Single();            

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

            if (table.Game.Name == "Geomancer")
            {
                // DATABASE DECOMPRESS: decompress from database
                GeomancerState masterState = (GeomancerState)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) }));
                // WIRE COMPRESS : Send client specific game state                     
                GeomancerState clientState = masterState.GetClientState(currentSeat.PlayerId);
                ViewBag.state = new HtmlString(Compression.ConvertToJSON(clientState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) })));
            }
            else if (table.Game.Name == "Onslaught")
            {
                // DATABASE DECOMPRESS: decompress from database
                OnslaughtState masterState = (OnslaughtState)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(OnslaughtState), new Type[] { typeof(OnslaughtPlayerContext), typeof(GalaxyCard), typeof(SupplyPile), typeof(InvasionCard), typeof(InvaderToken) }));
                // WIRE COMPRESS : Send client specific game state                     
                OnslaughtState clientState = masterState.GetClientState(currentSeat.PlayerId);
                ViewBag.state = new HtmlString(Compression.ConvertToJSON(clientState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(OnslaughtState), new Type[] { typeof(OnslaughtPlayerContext), typeof(GalaxyCard), typeof(SupplyPile), typeof(InvasionCard), typeof(InvaderToken) })));
            }
            else
            {
                string pickledState = Compression.DecompressStringState(table.GameState);                
                ViewBag.state = new HtmlString(GetPythonView(table, pickledState, currentSeat.PlayerId));
            }
            /*else
            {
                // DATABASE DECOMPRESS: decompress from database
                BaseGameState masterState = (BaseGameState)Compression.DecompressGameState(table.GameState, GameState<PlayerContext>.GetSerializer(table.Game.Name));
                // WIRE COMPRESS : Send client specific game state                     
                GameView clientState = masterState.GetClientView(currentSeat.PlayerId);
                ViewBag.state = new HtmlString(Compression.ConvertToJSON(clientState, masterState.GetSerializer()));
            }*/

            ViewBag.game = table.Game.Name;
            ViewBag.TableId = table.TableID;
            ViewBag.PlayerId = currentSeat.PlayerId;
            ViewBag.PlayerName = currentSeat.Player.Name;

            return View(table);
        }

        // Update
        [HttpPost]
        public ActionResult Update(int id, GeomancerState inputState)
        {
            Table table = db.Tables.Find(id);
            
            // Get and decompress master state from database                        
            GeomancerState masterState = (GeomancerState)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) }));
            // Merge state with master gamestate
            masterState.Update(inputState);          
            // Compress state for database storage
            table.GameState = Compression.CompressGameState(masterState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) }));
            db.SaveChanges();

            // Send updated states to clients
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<GameList>();
            foreach (Seat s in table.Seats)
            {
                clients[s.Player.Name + id].updateGameState(masterState.GetClientState(s.PlayerId));
            }

            return View();
        }

        // Update
        [HttpPost]
        public ActionResult UpdateOnslaught(int id, OnslaughtUpdate inputState)
        {
            Table table = db.Tables.Find(id);

            // Get and decompress master state from database                        
            OnslaughtState masterState = (OnslaughtState)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(OnslaughtState), new Type[] { typeof(OnslaughtPlayerContext), typeof(GalaxyCard), typeof(SupplyPile), typeof(InvasionCard), typeof(InvaderToken) }));
            // Merge state with master gamestate
            masterState.Update(inputState);
            // Compress state for database storage
            table.GameState = Compression.CompressGameState(masterState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(OnslaughtState), new Type[] { typeof(OnslaughtPlayerContext), typeof(GalaxyCard), typeof(SupplyPile), typeof(InvasionCard), typeof(InvaderToken) }));
            db.SaveChanges();

            // Send updated states to clients
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<GameList>();
            foreach (Seat s in table.Seats)
            {
                clients[s.Player.Name + id].onslaught_updateGameState(masterState.GetClientState(s.PlayerId));
            }

            return View();
        }

        [Authorize]
        public ActionResult Challenge(int opponentId, string gameName)
        {
            Player player = db.Players.Where(p => p.Name == User.Identity.Name).Single();
            Player opponent = db.Players.Find(opponentId);
            // Create Table
            Table newTable = new Table();
            newTable = db.Tables.Add(newTable);
            newTable.Game = db.Games.Single(g => g.Name == gameName);
            newTable.Version = newTable.Game.Versions.First();
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

        public static ScriptEngine engine = null;
        public static List<string> loadedModules = null;
        public static List<string> errors = null;

        public static void InitScriptEngine()
        {
            if (engine == null || loadedModules == null || errors == null)
            {
                engine = Python.CreateEngine();
                var paths = engine.GetSearchPaths();
                paths.Add(HostingEnvironment.MapPath(@"~/Python"));
                paths.Add(HostingEnvironment.MapPath(@"~/Protoplasm-Python"));
                engine.SetSearchPaths(paths);
                loadedModules = new List<string>();
                errors = new List<string>();
            }            
        }
        public static void LoadModules(string moduleName, string code)
        {
            if (!loadedModules.Contains(moduleName))
            {
                ScriptScope moduleScope = engine.CreateModule(moduleName);
                ScriptSource moduleSource = engine.CreateScriptSourceFromString(code, SourceCodeKind.File);
                moduleSource.Execute(moduleScope);
            }
        }

        public string GetPythonView(Table table, string pickledState, int playerId)
        {
            InitScriptEngine();
            LoadModules(table.Game.Name, table.Version.PythonScript);
            
            ScriptScope runScope = engine.CreateScope();
            runScope.ImportModule("cPickle");
            
            // Output state
            JSONContainer jsonContainer = new JSONContainer { json = "ERROR" };
            runScope.SetVariable("jsonContainer", jsonContainer);

            // Input state                                                                                                              
            runScope.SetVariable("inputState", pickledState);

            // Input playerId
            runScope.SetVariable("playerId", playerId);

            ScriptSource runSource = engine.CreateScriptSourceFromString("from encodings import hex_codec; import json; gameState = cPickle.loads(inputState);jsonString=json.dumps(gameState.view(playerId))", SourceCodeKind.Statements);

            for (int attempts = 0; attempts < 3; attempts++)
            {
                try
                {
                    runSource.Execute(runScope);
                    break;
                }
                catch (Exception e)
                {
                    errors.Add(e.ToString());
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

            InitScriptEngine();
            LoadModules(table.Game.Name, table.Version.PythonScript);

            ScriptScope runScope = engine.CreateScope();
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

            ScriptSource runSource = engine.CreateScriptSourceFromString("gameState = cPickle.loads(inputState);gameState.update(update);gameState.set_waiting_status(seats);game_over = gameState.game_over;finalState = cPickle.dumps(gameState)", SourceCodeKind.Statements);
            runSource.Execute(runScope);

            string final_pickledState = runScope.GetVariable("finalState");
            bool game_over = runScope.GetVariable("game_over");
            if (game_over == true)
                table.TableState = (int)TableState.Complete;
            table.GameState = Compression.CompressStringState(final_pickledState);

            DateTime scriptsExecuted = DateTime.Now;

            db.SaveChanges();

            DateTime databaseSaved = DateTime.Now;

            // Send updated states to clients
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<GameList>();
            foreach (Seat s in table.Seats)
            {
                string viewJson = GetPythonView(table, final_pickledState, s.PlayerId);
                clients[s.Player.Name + id].main_updateGameState(viewJson);
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

        /*
        // Update
        [HttpPost]
        public ActionResult UpdateMain(int id, GameUpdate inputState)
        {
            Table table = db.Tables.Find(id);

            // Get and decompress master state from database                        
            BaseGameState masterState = (BaseGameState)Compression.DecompressGameState(table.GameState, BaseGameState.GetSerializer(table.Game.Name));
            // Merge state with master gamestate
            masterState.Update(inputState);
            // Compress state for database storage
            table.GameState = Compression.CompressGameState(masterState, masterState.GetSerializer());
            db.SaveChanges();

            // Send updated states to clients
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<GameList>();
            foreach (Seat s in table.Seats)
            {
                clients[s.Player.Name + id].main_updateGameState(masterState.GetClientView(s.PlayerId));
            }

            return View();
        }*/

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