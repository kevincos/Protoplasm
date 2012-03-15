﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeckBuilder.Models;

namespace DeckBuilder.Controllers
{ 
    public class PlayerController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        //
        // GET: /Player/

        public ViewResult Index()
        {
            return View(db.Players.ToList());
        }

        //
        // GET: /Player/Details/5

        public ViewResult Details(int id)
        {
            Player player = db.Players.Find(id);
            return View(player);
        }

        //
        // GET: /Player/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Player/Create

        [HttpPost]
        public ActionResult Create(Player player)
        {
            if (ModelState.IsValid)
            {
                db.Players.Add(player);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(player);
        }
        
        //
        // GET: /Player/Edit/5
 
        [Authorize]
        public ActionResult Edit(int id)
        {
            Player player = db.Players.Find(id);

            if (player.Name != User.Identity.Name)
                return RedirectToAction("Details", "Player", new { id = player.PlayerID });
            

            PopulateOwnedCards(player);
            return View(player);
        }

        //
        // POST: /Player/Edit/5

        [HttpPost, Authorize]
        public ActionResult Edit(int id, FormCollection formCollection, int[] cardQuantities)
        {
            
            var playerToUpdate = db.Players
            .Include(p => p.CardSets)           
            .Where(p => p.PlayerID == id)
            .Single();
            if (TryUpdateModel(playerToUpdate))
            {
                if (ModelState.IsValid)
                {
                    UpdateOwnedCards(playerToUpdate, cardQuantities);
                    db.Entry(playerToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(playerToUpdate);
            }
            PopulateOwnedCards(playerToUpdate);
            return View(playerToUpdate);
        }

        //
        // GET: /Player/Delete/5
 
        public ActionResult Delete(int id)
        {
            Player player = db.Players.Find(id);
            return View(player);
        }

        //
        // POST: /Player/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Player player = db.Players.Find(id);
            db.Players.Remove(player);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        private void PopulateOwnedCards(Player player)
        {
            var allCards = db.Cards;
            var playerCardSets = new HashSet<int>(player.CardSets.Select(c => c.CardID));
            var viewModel = new List<CardSet>();
            foreach (var card in allCards)
            {
                if (playerCardSets.Contains(card.CardID))
                {
                    viewModel.Add(new CardSet
                    {
                        CardID = card.CardID,
                        Card = card,
                        Quantity = player.CardSets.Single(c => c.CardID == card.CardID).Quantity
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

        private void UpdateOwnedCards(Player playerToUpdate, int[] cardQuantities)
        {
            if (cardQuantities == null)
            {                
                return;
            }

            var allCards = db.Cards.ToList();
            
            for(int i =0; i < allCards.Count(); i++)
            {
                if (cardQuantities[i] > 0)
                {
                    CardSet cardSetToUpdate = playerToUpdate.CardSets.SingleOrDefault(c => c.CardID == allCards[i].CardID);
                    if (cardSetToUpdate == null)
                    {
                        var cardSet = new CardSet
                        {
                            PlayerID = playerToUpdate.PlayerID,
                            CardID = allCards[i].CardID,
                            Card = allCards[i],
                            Quantity = cardQuantities[i]
                        };
                        db.CardSets.Add(cardSet);
                        playerToUpdate.CardSets.Add(cardSet);
                    }
                    else
                    {
                        cardSetToUpdate.Quantity = cardQuantities[i];
                    }
                }
                else
                {
                    CardSet cardSetToRemove = playerToUpdate.CardSets.SingleOrDefault(c => c.CardID == allCards[i].CardID);
                    if (cardSetToRemove != null)
                    {
                        db.CardSets.Remove(cardSetToRemove);
                        playerToUpdate.CardSets.Remove(cardSetToRemove);
                    }

                }
            }
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}