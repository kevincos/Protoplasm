using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeckBuilder.Models;

namespace DeckBuilder.Controllers
{
    public class HomeController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to the Geomancer Deck Builder";

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
            SelectList dropdown = new SelectList(player.Decks, "DeckID", "Name");
            
            
            ViewBag.SelectedDeck = dropdown;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
