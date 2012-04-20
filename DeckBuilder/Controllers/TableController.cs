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

            List<Seat> seats = db.Seats.ToList();
            List<string> playerNames = seats.Select(s => s.Player.Name).ToList();
            if (!playerNames.Contains(User.Identity.Name))
            {
                return RedirectToAction("Details", "Table", new { id = id });
            }

            Seat currentSeat = table.Seats.Where(s => s.Player.Name == User.Identity.Name).Single();

            if (table.Game == "Geomancer")
            {
                // DATABASE DECOMPRESS: decompress from database
                GeomancerState masterState = (GeomancerState)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) }));
                // WIRE COMPRESS : Send client specific game state                     
                GeomancerState clientState = masterState.GetClientState(currentSeat.PlayerId);
                ViewBag.state = new HtmlString(Compression.ConvertToJSON(clientState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) })));
            }
            else if (table.Game == "RPS")
            {
                // DATABASE DECOMPRESS: decompress from database
                RPSState masterState = (RPSState)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(RPSState), new Type[] { typeof(RPSPlayerContext) }));
                // WIRE COMPRESS : Send client specific game state                     
                RPSState clientState = masterState.GetClientState(currentSeat.PlayerId);
                ViewBag.state = new HtmlString(Compression.ConvertToJSON(clientState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(RPSState), new Type[] { typeof(RPSPlayerContext) })));
            }
            else if (table.Game == "Onslaught")
            {
                // DATABASE DECOMPRESS: decompress from database
                OnslaughtState masterState = (OnslaughtState)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(OnslaughtState), new Type[] { typeof(OnslaughtPlayerContext), typeof(GalaxyCard), typeof(SupplyPile), typeof(InvasionCard), typeof(InvaderToken) }));
                // WIRE COMPRESS : Send client specific game state                     
                OnslaughtState clientState = masterState.GetClientState(currentSeat.PlayerId);
                ViewBag.state = new HtmlString(Compression.ConvertToJSON(clientState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(OnslaughtState), new Type[] { typeof(OnslaughtPlayerContext), typeof(GalaxyCard), typeof(SupplyPile), typeof(InvasionCard), typeof(InvaderToken) })));
            }
            else if (table.Game == "Connect4")
            {
                // DATABASE DECOMPRESS: decompress from database
                Connect4State masterState = (Connect4State)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Connect4State), new Type[] { typeof(Connect4PlayerContext), typeof(Connect4Update) }));
                // WIRE COMPRESS : Send client specific game state                     
                Connect4State clientState = masterState.GetClientState(currentSeat.PlayerId);
                ViewBag.state = new HtmlString(Compression.ConvertToJSON(clientState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Connect4State), new Type[] { typeof(Connect4PlayerContext), typeof(Connect4Update)})));
            }
            else if (table.Game == "CanyonConvoy")
            {
                // DATABASE DECOMPRESS: decompress from database
                ConvoyState masterState = (ConvoyState)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ConvoyState), new Type[] { typeof(ConvoyPlayerContext), typeof(ConvoyPiece), typeof(ConvoyTile), typeof(ConvoyUpdate) }));
                // WIRE COMPRESS : Send client specific game state                     
                ConvoyState clientState = masterState.GetClientState(currentSeat.PlayerId);
                ViewBag.state = new HtmlString(Compression.ConvertToJSON(clientState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ConvoyState), new Type[] { typeof(ConvoyPlayerContext), typeof(ConvoyPiece), typeof(ConvoyTile), typeof(ConvoyUpdate) })));
            }
            else if (table.Game == "Mechtonic")
            {
                // DATABASE DECOMPRESS: decompress from database
                MechtonicState masterState = (MechtonicState)Compression.DecompressGameState(table.GameState, MechtonicState.GetSerializer());
                // WIRE COMPRESS : Send client specific game state                     
                MechtonicState clientState = masterState.GetClientState(currentSeat.PlayerId);
                ViewBag.state = new HtmlString(Compression.ConvertToJSON(clientState, MechtonicState.GetSerializer()));
            }
            else
            {
                throw new Exception("Unknown Game: " + table.Game);
            }
            ViewBag.game = table.Game;
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
        public ActionResult UpdateRPS(int id, RPSState inputState)
        {
            Table table = db.Tables.Find(id);

            // Get and decompress master state from database                        
            RPSState masterState = (RPSState)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(RPSState), new Type[] { typeof(RPSPlayerContext) }));
            // Merge state with master gamestate
            masterState.Update(inputState);
            // Compress state for database storage
            table.GameState = Compression.CompressGameState(masterState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(RPSState), new Type[] { typeof(RPSPlayerContext) }));
            db.SaveChanges();

            // Send updated states to clients
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<GameList>();
            foreach (Seat s in table.Seats)
            {
                clients[s.Player.Name+id].rps_updateGameState(masterState.GetClientState(s.PlayerId));
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

        // Update
        [HttpPost]
        public ActionResult UpdateConnect4(int id, Connect4Update inputState)
        {
            Table table = db.Tables.Find(id);

            // Get and decompress master state from database                        
            Connect4State masterState = (Connect4State)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Connect4State), new Type[] { typeof(Connect4PlayerContext), typeof(Connect4Update) }));
            // Merge state with master gamestate
            masterState.Update(inputState);
            // Compress state for database storage
            table.GameState = Compression.CompressGameState(masterState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Connect4State), new Type[] { typeof(Connect4PlayerContext), typeof(Connect4Update) }));
            db.SaveChanges();

            // Send updated states to clients
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<GameList>();
            foreach (Seat s in table.Seats)
            {
                clients[s.Player.Name + id].connect4_updateGameState(masterState.GetClientState(s.PlayerId));
            }

            return View();
        }

        // Update
        [HttpPost]
        public ActionResult UpdateConvoy(int id, ConvoyUpdate inputState)
        {
            Table table = db.Tables.Find(id);

            // Get and decompress master state from database                        
            ConvoyState masterState = (ConvoyState)Compression.DecompressGameState(table.GameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ConvoyState), new Type[] { typeof(ConvoyPlayerContext), typeof(ConvoyPiece), typeof(ConvoyTile), typeof(ConvoyUpdate) }));
            // Merge state with master gamestate
            masterState.Update(inputState);
            // Compress state for database storage
            table.GameState = Compression.CompressGameState(masterState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ConvoyState), new Type[] { typeof(ConvoyPlayerContext), typeof(ConvoyPiece), typeof(ConvoyTile), typeof(ConvoyUpdate) }));
            db.SaveChanges();

            // Send updated states to clients
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<GameList>();
            foreach (Seat s in table.Seats)
            {
                clients[s.Player.Name + id].convoy_updateGameState(masterState.GetClientState(s.PlayerId));
            }

            return View();
        }

        // Update
        [HttpPost]
        public ActionResult UpdateMechtonic(int id, MechtonicUpdate inputState)
        {
            Table table = db.Tables.Find(id);

            // Get and decompress master state from database                        
            MechtonicState masterState = (MechtonicState)Compression.DecompressGameState(table.GameState, MechtonicState.GetSerializer());
            // Merge state with master gamestate
            masterState.Update(inputState);
            // Compress state for database storage
            table.GameState = Compression.CompressGameState(masterState, MechtonicState.GetSerializer());
            db.SaveChanges();

            // Send updated states to clients
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<GameList>();
            foreach (Seat s in table.Seats)
            {
                clients[s.Player.Name + id].mechtonic_updateGameState(masterState.GetClientState(s.PlayerId));
            }

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