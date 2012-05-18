﻿using System;
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
    public class GameController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        //
        // GET: /Game/

        public ViewResult Index()
        {
            return View(db.Games.ToList());
        }

        //
        // GET: /Game/Details/5

        public ViewResult Details(int id)
        {
            Game game = db.Games.Find(id);
            var viewModel = new GameDetailsViewModel
            {
                GameId = id,
                Creator = game.Creator,
                Name = game.Name,
                Versions = game.Versions.ToList()
            };
            return View(viewModel);
        }

        public ActionResult CreateVersion(int id)
        {
            Game game = db.Games.Find(id);
            GameVersion version = new GameVersion { VersionString = "1.0", MaxPlayers = 2, PythonScript = "", CreationDate = DateTime.Now, ParentGame = game };
            game.Versions.Add(version);
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = id }); 
        }


        //
        // GET: /Player/Admin

        public ViewResult Admin()
        {
            return View(db.Games.ToList());
        }

        //
        // GET: /Game/Create

        [Authorize]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Game/Create

        [HttpPost]
        [Authorize]
        public ActionResult Create(Game game)
        {
            PlayerIdentity playerIdentity = (PlayerIdentity)User.Identity;
            var player = db.Players.Where(p => p.Name == playerIdentity.Name).Single();
            game.Creator = player;
            game.CreatorId = player.PlayerID;            
            if (ModelState.IsValid)
            {
                db.Games.Add(game);
                db.SaveChanges();
                player.CreatedGames.Add(game);
                db.SaveChanges();
                return RedirectToAction("Edit", "Game", new { id = game.GameID });  
            }

            return View(game);
        }
        
        //
        // GET: /Game/Edit/5
 
        public ActionResult Edit(int id)
        {
            Game game = db.Games.Find(id);
            return View(game);
        }

        //
        // POST: /Game/Edit/5

        [HttpPost]
        public ActionResult Edit(Game game)
        {
            if (ModelState.IsValid)
            {
                db.Entry(game).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(game);
        }

        //
        // GET: /Game/Delete/5
 
        public ActionResult Delete(int id)
        {
            Game game = db.Games.Find(id);
            return View(game);
        }

        //
        // POST: /Game/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Game game = db.Games.Find(id);
            db.Games.Remove(game);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}