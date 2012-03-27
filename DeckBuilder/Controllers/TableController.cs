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

            // Case 1 - Player is active
            Seat currentSeat = table.Seats.Where(s => s.Player.Name == User.Identity.Name).Single();

            ViewBag.YourTurn = currentSeat.Active;
            ViewBag.PlayerName = User.Identity.Name;
            ViewBag.PlayerId = currentSeat.PlayerId;
            ViewBag.TableId = id;
            if (table.TotalTurns > 0)
            {
                ViewBag.Results = table.Results;
            }
            if (table.Finished)
            {
                ViewBag.FinalResults = table.FinalResults;
            }
            ViewBag.Finished = table.Finished;
            ViewBag.PlayerScore = currentSeat.Wins;

            // ASSUMES ONLY 2 PLAYERS
            Seat opponentSeat = table.Seats.Where(s => s.Player.Name != User.Identity.Name).Single();
            ViewBag.OpponentName = opponentSeat.Player.Name;
            ViewBag.OpponentScore = opponentSeat.Wins;

            if (table.Finished == false)
            {
                if (currentSeat.Active == true)
                {
                    ViewBag.Message = "YOUR TURN!";
                    ViewBag.MoveList = new SelectList(new List<string> { "Rock", "Paper", "Scissors" });
                }
                else
                {
                    ViewBag.Message = "You've played " + currentSeat.LastMove + "! Waiting for opponent...";
                }
            }

            // DATABASE DECOMPRESS: decompress from database
            string minijson = table.GameState;
            string json = "";            
            using (var decomStream = new MemoryStream(Encoding.Default.GetBytes(minijson)))
            {
                using (var hgs = new GZipStream(decomStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(hgs))
                    {
                        json = reader.ReadToEnd();
                    }
                }
            }

            System.Runtime.Serialization.Json.DataContractJsonSerializer deserializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) });
            MemoryStream masterStream = new MemoryStream(Encoding.Default.GetBytes(json));
            GeomancerState masterState = (GeomancerState)deserializer.ReadObject(masterStream);
            foreach (GeomancerPlayerContext playerContext in masterState.playerContexts)
            {
                playerContext.deck = null;
                if (playerContext.playerId != currentSeat.PlayerId)
                {
                    playerContext.hand = null;
                }
            }
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) });
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, masterState);
            string clientJson = Encoding.Default.GetString(ms.ToArray());

            // WIRE COMPRESS : Send compressed game state to clinet
            ViewBag.state = new HtmlString(clientJson);  

            return View(table);
        }

        // Update
        [HttpPost]
        public ActionResult Update(int id, GeomancerState inputState)
        {
            Table table = db.Tables.Find(id);
            
            // DATABASE DECOMPRESS: Get and decompress master state from database
            string minijson = table.GameState;
            string json = "";
            using (var decomStream = new MemoryStream(Encoding.Default.GetBytes(minijson)))
            {
                using (var hgs = new GZipStream(decomStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(hgs))
                    {
                        json = reader.ReadToEnd();
                    }
                }
            }
            System.Runtime.Serialization.Json.DataContractJsonSerializer deserializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) });
            MemoryStream masterStream = new MemoryStream(Encoding.Default.GetBytes(json));
            GeomancerState masterState = (GeomancerState)deserializer.ReadObject(masterStream);

            // WIRE DECOMPRESS: Merge state with master gamestate



            for (int a = 0; a < inputState.tileList.Count(); a++)
            {
                for (int b = 0; b < inputState.tileList[a].Count(); b++)
                {
                    GeomancerTile inputTile = inputState.tileList[a][b];
                    GeomancerTile masterTile = masterState.tileList[a][b];
                    if (inputTile != null)
                    {
                        if (inputTile.moveUnit != null)
                        {
                            masterTile.unit = inputTile.moveUnit;
                            if (inputState.tileList[inputTile.moveUnit.moveA][inputTile.moveUnit.moveB].unit.used == true)
                                masterState.tileList[inputTile.moveUnit.moveA][inputTile.moveUnit.moveB].unit = null;
                            masterTile.moveUnit = null;
                        }

                    }
                }
            }
            for (int a = 0; a < inputState.tileList.Count(); a++)
            {
                for (int b = 0; b < inputState.tileList[a].Count(); b++)
                {
                    GeomancerTile inputTile = inputState.tileList[a][b];
                    GeomancerTile masterTile = masterState.tileList[a][b];
                    if (inputTile != null)
                    {
                        if (inputTile.spell != null)
                        {
                            GeomancerCard sourceCard = inputState.playerContexts[inputState.activePlayerIndex].hand[inputTile.spell.sourceCardIndex];
                            if (sourceCard.type == "Summon")
                            {
                                masterTile.unit = inputState.playerContexts[inputState.activePlayerIndex].hand[inputTile.spell.sourceCardIndex].castUnit;
                                masterTile.unit.playerId = inputTile.spell.playerId;
                            }
                            if (sourceCard.type == "Crystal")
                            {
                                masterTile.crystal = inputState.playerContexts[inputState.activePlayerIndex].hand[inputTile.spell.sourceCardIndex].castCrystal;
                                masterTile.crystal.playerId = inputTile.spell.playerId;
                            }
                            masterTile.spell = null;
                        }
                    }
                }
            }

            masterState.playerContexts[inputState.activePlayerIndex].hand = inputState.playerContexts[inputState.activePlayerIndex].hand.Where(c => c.used == false).ToList();
            Random r = new Random();
            masterState.DrawCard(r, masterState.playerContexts[masterState.activePlayerIndex]);

            masterState.activePlayerIndex++;
            masterState.activePlayerIndex %= masterState.playerContexts.Count;

            // DATABASE COMPRESS: Compress state for database storage
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) });
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, masterState);
            ms.Close();
            MemoryStream compressStream = new MemoryStream(ms.ToArray());
            minijson = "";
            using (var cmpStream = new MemoryStream())
            {
                using (var hgs = new GZipStream(cmpStream, CompressionMode.Compress))
                {
                    compressStream.CopyTo(hgs);
                }
                minijson = Encoding.Default.GetString(cmpStream.ToArray());
            }                                               
            json = Encoding.Default.GetString(ms.ToArray());
            
            //table.GameState = json;
            table.GameState = minijson;
            db.SaveChanges();

            
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<GameList>();
            foreach (GeomancerPlayerContext playerContext in masterState.playerContexts)
            {
                playerContext.deck = null;
            }
            foreach (Seat s in table.Seats)
            {
                List<List<GeomancerCard>> savedHands = new List<List<GeomancerCard>>();
                foreach (GeomancerPlayerContext playerContext in masterState.playerContexts)
                {
                    savedHands.Add(playerContext.hand);
                    if (s.PlayerId != playerContext.playerId)
                    {
                        playerContext.hand = null;
                    }
                    
                }
                // WIRE COMPRESS: Send compact version to client                   
                clients[s.Player.Name].updateGameState(masterState);
                for(int i = 0;i < masterState.playerContexts.Count; i++)
                {
                    GeomancerPlayerContext playerContext = masterState.playerContexts[i];
                    playerContext.hand = savedHands[i];
                }
            }

            // WIRE COMPRESS: Return compact version to client
            //return Json(masterState);
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