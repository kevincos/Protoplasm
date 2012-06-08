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
                ViewBag.Notifications = db.Notifications.Where(n => n.PlayerID == player.PlayerID).OrderBy(n => n.DatePosted).Take(10);
                ViewBag.NotificationsCount = db.Notifications.Where(n => n.PlayerID == player.PlayerID).Count();
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
            SelectList deckDropdown = new SelectList(player.Decks, "DeckID", "Name");

            List<GameVersion> versionList = db.Games.ToList().Select(g => g.LatestRelease).ToList();
            versionList.AddRange(player.CreatedGames.ToList().Select(g => g.ActiveDev).ToList());
            versionList = versionList.Where(v => v != null).ToList();
            SelectList gameDropdown = new SelectList(versionList, "GameVersionID", "DisplayName");            

            SelectList roomDropdown = new SelectList(new List<string> { "General", "Classic", "Prototype" });
            
            
            ViewBag.SelectedDeck = deckDropdown;
            ViewBag.SelectedGame = gameDropdown;
            ViewBag.LobbyRoom = roomDropdown;
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
