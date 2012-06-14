using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeckBuilder.Models;
using DeckBuilder.ViewModels;
using DeckBuilder.Stats;

namespace DeckBuilder.Controllers
{
    public class GameVersionController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        //
        // GET: /GameVersion/

        public ViewResult Index()
        {
            return View(db.Games.ToList());
        }

        //
        // GET: /GameVersion/Details/5

        public ViewResult Details(int id)
        {
            GameVersion version = db.Versions.Find(id);
            return View(version);
        }

        //
        // GET: /GameVersion/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // TEST: /GameVersion/Test/5

        [Authorize]
        public ActionResult Test(int id, int numPlayers=2)
        {
            GameVersion version = db.Versions.Find(id);
            Game game = version.ParentGame;
            

            PlayerIdentity playerIdentity = (PlayerIdentity)User.Identity;
            Player player = db.Players.Where(p => p.Name == playerIdentity.Name).Single();
            if (player.PlayerID != game.CreatorId)
                RedirectToAction("DeveloperProfile", "Home");

            

            Table newTable = new Table();
            newTable = db.Tables.Add(newTable);
            //newTable.LastUpdateTime = DateTime.Now;
            newTable.Game = db.Games.Single(g => g.Name == game.Name);
            newTable.Version = newTable.Game.Versions.First();            
            newTable.TableState = (int)TableState.Proposed;
            newTable.SoloPlayTest = true;
            db.SaveChanges();

            // Create Seats
            for (int i = 0; i < numPlayers; i++)
            {
                Seat s = new Seat
                {
                    PlayerId = player.PlayerID,
                    TableId = newTable.TableID,
                    Accepted = true,
                    Waiting = false
                };
                db.Seats.Add(s);    
            }            

            db.SaveChanges();

            newTable = db.Tables.Where(t => t.TableID == newTable.TableID).Include("Seats.Deck.CardSets.Card").Single();

            string error = newTable.GenerateInitialState();
            if (error == "")
            {
                db.SaveChanges();
                return RedirectToAction("Play", "Table", new { id = newTable.TableID, playerIndex = 0 });
            }
            else
            {
                db.Tables.Remove(newTable);
                error = error.Replace("<br/>", "\n");
                error = error.Replace("<string>", "Unknown");
                return RedirectToAction("Edit", "GameVersion", new { id = id, initError = error });
            }
        }


        public String ImageUrl(int id)
        {
            Game game = db.Versions.Find(id).ParentGame;
            return game.ImageUrl;
        }

                //
        // TEST: /GameVersion/Test/5

        [Authorize]
        public ActionResult Release(int id)
        {
            GameVersion version = db.Versions.Find(id);
            Game game = version.ParentGame;
            int parentId = game.GameID;
            

            PlayerIdentity playerIdentity = (PlayerIdentity)User.Identity;
            Player player = db.Players.Where(p => p.Name == playerIdentity.Name).Single();
            if (player.PlayerID != game.CreatorId)
                RedirectToAction("DeveloperProfile", "Home");

            GameVersion newVersion = new GameVersion
            {
                ModuleName = version.ModuleName,
                VersionString = version.VersionString,
                PythonScript = version.PythonScript,
                MaxPlayers = version.MaxPlayers,
                ParentGame = version.ParentGame,
                CreationDate = DateTime.Now,
                StatLog = "[]",
                DevStage = "Alpha"
            };
            game.Versions.Add(newVersion);
            version.DevStage = "Release";
            version.CreationDate = DateTime.Now;
            version.VersionString = "X";
            db.SaveChanges();

            return RedirectToAction("Edit", "Game", new { id = parentId });
        }

        public ActionResult Stats(int id)
        {
            /*GameVersion version = db.Versions.Find(id);

            Graph g = new Graph(new List<string> { "0", "1" });
            g.AddDataSet("WinRate");
            Condition winCondition = new Condition("result", "Win");
            Condition indexCondition = new Condition("index", "[x]");
            g.PercentOverItems("player", new List<Condition> { indexCondition }, new List<Condition> { winCondition });
            g.GenerateData(version.StatLog);

            return RedirectToAction("Edit", new { id = version.GameVersionID });*/
            ViewBag.VersionID = id;
            return View();
        }

        public JsonResult Graph(int id, Graph graphData)
        {
            GameVersion version = db.Versions.Find(id);

            List<List<float>> results = graphData.GenerateData(version.StatLog);

            /*Graph g = new Graph(new List<string> { "0", "1" });
            g.AddDataSet("WinRate");
            Condition winCondition = new Condition("result", "Win");
            Condition indexCondition = new Condition("index", "[x]");
            g.PercentOverItems("player", new List<Condition> { indexCondition }, new List<Condition> { winCondition });
            g.GenerateData(version.StatLog);*/

            return Json(results);          
        }

        //
        // POST: /GameVersion/Create

        [HttpPost]
        public ActionResult Create(GameVersion version)
        {
            if (ModelState.IsValid)
            {
                db.Versions.Add(version);
                db.SaveChanges();
                return RedirectToAction("Edit", "GameVersion", new { id = version.GameVersionID });
            }

            return View(version);
        }

        //
        // GET: /GameVersion/Edit/5

        public ActionResult Edit(int id, String initError)
        {
            if(initError != null)
            {
                String htmlError = initError.Replace("\n", "<br/>");
                ViewBag.Error = htmlError;
            }
            GameVersion version = db.Versions.Find(id);
            return View(version);
        }

        //
        // POST: /GameVersion/Edit/5

        [HttpPost]
        public ActionResult Edit(GameVersion version)
        {
            if (ModelState.IsValid)
            {
                db.Entry(version).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new { id=version.GameVersionID });
            }
            return View(version);
        }

        //
        // GET: /GameVersion/Delete/5

        public ActionResult Delete(int id)
        {
            GameVersion version = db.Versions.Find(id);
            return View(version);
        }

        //
        // POST: /Game/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            GameVersion version = db.Versions.Find(id);
            int parentId = version.ParentGame.GameID;
            foreach (Table t in db.Tables.Where(t => t.Version.GameVersionID == version.GameVersionID))
            {
                db.Tables.Remove(t);
            }
                
            db.Versions.Remove(version);
            db.SaveChanges();
            return RedirectToAction("Edit", "Game", new { id = parentId });
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}