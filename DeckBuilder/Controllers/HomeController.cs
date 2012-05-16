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

        public ActionResult Index()
        {
            foreach (Table t in db.Tables)
            {
                db.Tables.Remove(t);
            }
            //foreach (Game g in db.Games)
            //{
                //db.Games.Remove(g);
            //}
            db.SaveChanges();

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

        public ActionResult About()
        {
            return View();
        }
    }
}
