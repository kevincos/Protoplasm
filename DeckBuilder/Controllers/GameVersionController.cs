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
    public class GameVersionController : Controller
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
            GameVersion version = db.Versions.Find(id);
            return View(version);
        }

        //
        // GET: /Game/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Game/Create

        [HttpPost]
        public ActionResult Create(GameVersion version)
        {
            if (ModelState.IsValid)
            {
                db.Versions.Add(version);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(version);
        }

        //
        // GET: /Game/Edit/5

        public ActionResult Edit(int id)
        {
            GameVersion version = db.Versions.Find(id);
            return View(version);
        }

        //
        // POST: /Game/Edit/5

        [HttpPost]
        public ActionResult Edit(GameVersion version)
        {
            if (ModelState.IsValid)
            {
                db.Entry(version).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id=version.GameVersionID });
            }
            return View(version);
        }

        //
        // GET: /Game/Delete/5

        public ActionResult Delete(int id)
        {
            GameVersion version = db.Versions.Find(id);
            return View(version);
        }

        //
        // POST: /Game/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            GameVersion version = db.Versions.Find(id);
            db.Versions.Remove(version);
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