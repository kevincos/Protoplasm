using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using DeckBuilder.Models;

using System.Data;
using System.Data.Entity;

using DeckBuilder.Protoplasm_Python;

namespace DeckBuilder.Controllers
{
    public class HomeController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        [Authorize]
        public ActionResult Custom()
        {
            PlayerIdentity playerIdentity = (PlayerIdentity)User.Identity;
            var player = db.Players.Where(p => p.Name == playerIdentity.Name).Single();
            if (player.Name != "KevinC")
            {
                return RedirectToAction("Index");
            }

            foreach (GameVersion v in db.Versions)
            {
                if (v.DevStage != "Alpha" && v.DevStage != "Release")
                    v.DevStage = "Alpha";
            }

           
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult ResetPython()
        {
            PlayerIdentity playerIdentity = (PlayerIdentity)User.Identity;
            var player = db.Players.Where(p => p.Name == playerIdentity.Name).Single();
            if (player.Name != "KevinC")
            {
                return RedirectToAction("Index");
            }
            PythonScriptEngine.Reset();
            return RedirectToAction("Index");
        }

        public ActionResult WipeTables()
        {
            foreach (Table t in db.Tables)
            {
                db.Tables.Remove(t);
            }
            foreach (Seat s in db.Seats)
            {
                db.Seats.Remove(s);
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {

            ViewBag.TopPosts = db.Posts.Include(p=>p.Player).OrderByDescending(p => p.Date).Take(2);
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Menu()
        {            
            var player = db.Players.Where(p => p.Name == User.Identity.Name).SingleOrDefault();
            if (player == null)
            {
                ViewBag.Name = "";
                ViewBag.Notifications = new List<Notification>();
                ViewBag.PlayerId = 0;
                ViewBag.NotificationsCount = 0;
            }
            else
            {
                ViewBag.Name = player.Name;
                ViewBag.PlayerId = player.PlayerID;
                ViewBag.Notifications = db.Notifications.Where(n => n.PlayerID == player.PlayerID && n.Suppressed == false).OrderByDescending(n => n.DatePosted).Take(10);
                ViewBag.NotificationsCount = db.Notifications.Where(n => n.PlayerID == player.PlayerID && n.Read==false && n.Suppressed == false).Count();
            }

            return PartialView();
        }
        
        [Authorize]
        public ActionResult Profile()
        {
            PlayerIdentity playerIdentity = (PlayerIdentity)User.Identity;
            var player = db.Players.Where(p => p.Name == playerIdentity.Name).Single();
                
            return RedirectToAction("Details", "Player", new { id = player.PlayerID });
        }

        [Authorize]
        public ActionResult DeveloperProfile()
        {
            PlayerIdentity playerIdentity = (PlayerIdentity)User.Identity;
            var player = db.Players.Where(p => p.Name == playerIdentity.Name).Single();

            return RedirectToAction("Developer", "Player", new { id = player.PlayerID });
        }

        [Authorize]
        public ActionResult Lobby()
        {
            PlayerIdentity playerIdentity = (PlayerIdentity)User.Identity;
            var player = db.Players.Where(p => p.Name == playerIdentity.Name).Single();
            ViewBag.PlayerName = player.Name;
            //SelectList deckDropdown = new SelectList(player.Decks, "DeckID", "Name");

            //List<GameVersion> releaseGames = db.Games.ToList().Select(g => g.LatestRelease).ToList();
            List<Game> releaseGames = db.Games.ToList();
            releaseGames = releaseGames.Where(g => g.LatestRelease != null).ToList();
            List<GameVersion> allGames = db.Games.ToList().Select(g => g.LatestRelease).ToList();
            allGames.AddRange(player.CreatedGames.ToList().Select(g => g.ActiveDev).ToList());
            allGames = allGames.Where(v => v != null).ToList();
            releaseGames = releaseGames.Where(v => v != null).ToList();
            SelectList gameDropdown = new SelectList(allGames, "GameVersionID", "DisplayName");
            SelectList rankedGameDropdown = new SelectList(releaseGames, "GameID", "Name");
            
            ViewBag.SelectedGame = gameDropdown;
            ViewBag.SelectedGameRanked = rankedGameDropdown;
            
            return View();
        }

        public ActionResult Docs()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
