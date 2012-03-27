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

namespace DeckBuilder.Controllers
{
    public class Test
    {
        public int x { get; set; }
        public int y { get; set; }
    }
    public class Product
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Test test { get; set; }
    }

    public class HomeController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        public ActionResult Index()
        {
            /*GeomancerState gameState = new GeomancerState();
            gameState.InitializeState();
            ViewBag.Message = "Welcome to the Geomancer Deck Builder";
            
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell), typeof(GeomancerPlayerContext)});
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, gameState);
            string json = Encoding.Default.GetString(ms.ToArray());

            ViewBag.state = new HtmlString(json);            */
            

            return View();
        }

        [HttpPost]
        public ActionResult Submit(GeomancerState state)
        {
            foreach (GeomancerCard card in state.playerContexts[state.activePlayerIndex].hand)
            {
                card.used = false;                
            }
            for (int a = 0; a < state.tileList.Count(); a++)
            {
                for (int b = 0; b < state.tileList[a].Count(); b++)
                {                    
                    GeomancerTile tile = state.tileList[a][b];
                    if (tile != null)
                    {
                        if (tile.moveUnit != null)
                        {
                            tile.unit = tile.moveUnit;
                            if(state.tileList[tile.moveUnit.moveA][tile.moveUnit.moveB].unit.used == true)
                                state.tileList[tile.moveUnit.moveA][tile.moveUnit.moveB].unit = null;
                            tile.moveUnit = null;
                        }

                    }                    
                }
            }
            for (int a = 0; a < state.tileList.Count(); a++)
            {
                for (int b = 0; b < state.tileList[a].Count(); b++)
                {
                    GeomancerTile tile = state.tileList[a][b];
                    if (tile != null)
                    {
                        if (tile.spell != null)
                        {
                            GeomancerCard sourceCard = state.playerContexts[state.activePlayerIndex].hand[tile.spell.sourceCardIndex];
                            if (sourceCard.type == "Summon")
                            {
                                tile.unit = state.playerContexts[state.activePlayerIndex].hand[tile.spell.sourceCardIndex].castUnit;
                                tile.unit.playerId = tile.spell.playerId;
                            }
                            if (sourceCard.type == "Crystal")
                            {
                                tile.crystal = state.playerContexts[state.activePlayerIndex].hand[tile.spell.sourceCardIndex].castCrystal;
                                tile.crystal.playerId = tile.spell.playerId;
                            }
                            tile.spell = null;
                        }
                    }
                }
            }
            state.activePlayerIndex++;
            state.activePlayerIndex %= state.playerContexts.Count;
            return Json(state);
        }

        [HttpPost]
        public ActionResult Card(GeomancerCard card)
        {
            return null;
        }

        [HttpPost]
        public ActionResult Save(Product product)
        {
            //string message = string.Format("Created user '{0}' in the system.", inputModel.Name);
            return null;
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
