using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeckBuilder.Models;

/*
namespace DeckBuilder.Controllers
{ 
    public class DeckController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        //
        // GET: /Deck/

        public ViewResult Index()
        {
            var decks = db.Decks.Include(d => d.Player);
            return View(decks.ToList());
        }

        //
        // GET: /Deck/Details/5

        public ViewResult Details(int id)
        {
            Deck deck = db.Decks.Find(id);
            return View(deck);
        }

        //
        // GET: /Deck/Create

        public ActionResult Create(int playerId)
        {
            ViewBag.PlayerId = playerId;
            Player player = db.Players.Find(playerId);
            PopulateAvailableCards(player, null);
            return View();
        } 

        //
        // POST: /Deck/Create

        [HttpPost]
        public ActionResult Create(Deck deck, FormCollection formCollection, int[] cardQuantities)
        {            
            if (ModelState.IsValid)
            {                
                db.Decks.Add(deck);
                db.SaveChanges();

                Deck deckToUpdate = db.Decks.Where(d => d.DeckID == deck.DeckID).Include(d => d.CardSets).Single();
                UpdateDeckCards(deckToUpdate, cardQuantities);

                db.SaveChanges();

                return RedirectToAction("Index");  
            }

            ViewBag.PlayerId = new SelectList(db.Players, "PlayerID", "Name", deck.PlayerId);
            return View(deck);
        }
        
        //
        // GET: /Deck/Edit/5
 
        public ActionResult Edit(int id)
        {
            Deck deck = db.Decks.Find(id);
            Player player = deck.Player;
            ViewBag.PlayerId = new SelectList(db.Players, "PlayerID", "Name", deck.PlayerId);
            PopulateAvailableCards(player, deck);
            return View(deck);
        }

        //
        // POST: /Deck/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection formCollection, int[] cardQuantities)
        {
            var deckToUpdate = db.Decks
                .Include(p => p.CardSets)
                .Where(p => p.DeckID == id)
                .Single();

            if (ModelState.IsValid)
            {
                UpdateDeckCards(deckToUpdate, cardQuantities);
                db.Entry(deckToUpdate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlayerId = new SelectList(db.Players, "PlayerID", "Name", deckToUpdate.PlayerId);
            return View(deckToUpdate);
        }


        private void PopulateAvailableCards(Player player, Deck deck)
        {
            var playerCardSets = player.CardSets.Select(c=>c.Card);
            var deckCardSets = new HashSet<int>();
            if(deck != null)
                deckCardSets = new HashSet<int>(deck.CardSets.Select(c=>c.CardID));
            
            var viewModel = new List<CardSet>();
            foreach (var card in playerCardSets)
            {
                if (deckCardSets.Contains(card.CardID))
                {
                    viewModel.Add(new CardSet
                    {
                        CardID = card.CardID,
                        Card = card,
                        Quantity = deck.CardSets.Single(c => c.CardID == card.CardID).Quantity
                    });
                }
                else
                {
                    viewModel.Add(new CardSet
                    {
                        CardID = card.CardID,
                        Card = card,
                        Quantity = 0
                    });
                }
            }
            ViewBag.CardSets = viewModel;
        }

        private void UpdateDeckCards(Deck deckToUpdate, int[] cardQuantities)
        {
            if (cardQuantities == null)
            {
                return;
            }

            var playerCardSets = db.Players.Find(deckToUpdate.PlayerId).CardSets.Select(c => c.Card).ToList();

            for (int i = 0; i < playerCardSets.Count(); i++)
            {
                if (cardQuantities[i] > 0)
                {
                    CardSet cardSetToUpdate = deckToUpdate.CardSets.SingleOrDefault(c => c.CardID == playerCardSets[i].CardID);
                    if (cardSetToUpdate == null)
                    {
                        var cardSet = new CardSet
                        {
                            DeckID = deckToUpdate.DeckID,
                            CardID = playerCardSets[i].CardID,
                            Card = playerCardSets[i],
                            Quantity = cardQuantities[i]
                        };
                        db.CardSets.Add(cardSet);
                        deckToUpdate.CardSets.Add(cardSet);
                    }
                    else
                    {
                        cardSetToUpdate.Quantity = cardQuantities[i];
                    }
                }
                else
                {
                    CardSet cardSetToRemove = deckToUpdate.CardSets.SingleOrDefault(c => c.CardID == playerCardSets[i].CardID);
                    if (cardSetToRemove != null)
                    {
                        db.CardSets.Remove(cardSetToRemove);
                        deckToUpdate.CardSets.Remove(cardSetToRemove);
                    }
                }
            }
        }

        //
        // GET: /Deck/Delete/5
 
        public ActionResult Delete(int id)
        {
            Deck deck = db.Decks.Find(id);
            return View(deck);
        }

        //
        // POST: /Deck/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Deck deck = db.Decks.Find(id);

            List<CardSet> oldCardSets = db.CardSets.Where(c => c.DeckID == deck.DeckID).ToList();
            foreach (CardSet cardSet in oldCardSets)
            {
                db.CardSets.Remove(cardSet);
            }
            db.Decks.Remove(deck);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}*/