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

            foreach (Player p in db.Players.ToList())
            {
                p.ActiveSeats = new List<Seat>();
            }

            foreach (Game g in db.Games.Include("Creator").ToList())
            {
                g.CreatorId = 1;
                g.Creator = player;
            }
            db.SaveChanges();
            foreach (Game g in db.Games.Include("Creator").ToList())
            {
                if(g.Creator.CreatedGames == null)
                    g.Creator.CreatedGames = new List<Game>();
                if (!g.Creator.CreatedGames.Contains(g))
                {
                    g.Creator.CreatedGames.Add(g);
                }
            }
            db.SaveChanges();
            /*foreach (Table t in db.Tables)
            {
                db.Tables.Remove(t);
            }
            foreach (Game g in db.Games)
            {
                g.CreatorId = 1;
                g.Creator = player;
            }
            db.SaveChanges();*/            

            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            ViewBag.TopPosts = db.Posts.Include(p=>p.Player).OrderByDescending(p => p.Date).Take(2);
            return View();
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

            SelectList gameDropdown = new SelectList(db.Games.Select(g => g.Name).ToList());

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
