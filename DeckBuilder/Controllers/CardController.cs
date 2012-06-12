using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeckBuilder.Models;

namespace DeckBuilder.Controllers
{ 
    public class CardController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        //
        // GET: /Card/

        public ViewResult Index()
        {
            return View(db.Cards.ToList());
        }

        //
        // GET: /Card/Details/5

        public ViewResult Details(int id)
        {
            Card card = db.Cards.Find(id);
            return View(card);
        }

        //
        // GET: /Card/Create

        public ActionResult Create()
        {            
            return View();
        } 

        //
        // POST: /Card/Create

        [HttpPost]
        public ActionResult Create(Card card)
        {
            if (ModelState.IsValid)
            {
                db.Cards.Add(card);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(card);
        }
        
        //
        // GET: /Card/Edit/5
 
        public ActionResult Edit(int id)
        {
            Card card = db.Cards.Find(id);
            
            return View(card);
        }

        //
        // POST: /Card/Edit/5

        [HttpPost]
        public ActionResult Edit(Card card)
        {
            if (ModelState.IsValid)
            {
                db.Entry(card).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(card);
        }

        //
        // GET: /Card/Delete/5
 
        public ActionResult Delete(int id)
        {
            Card card = db.Cards.Find(id);
            return View(card);
        }

        //
        // POST: /Card/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Card card = db.Cards.Find(id);
            db.Cards.Remove(card);
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