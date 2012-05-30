using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeckBuilder.Models;
using DeckBuilder.ViewModels;

namespace DeckBuilder.Controllers
{
    public class MatchRequestController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        public ActionResult Add(int gameId, int numPlayers)
        {            
            return RedirectToAction("Index", "Game", null);
        }
    }
}
