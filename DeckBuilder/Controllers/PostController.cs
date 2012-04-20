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
    public class PostController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        //
        // GET: /Post/

        public ViewResult Index()
        {
            var posts = db.Posts.Include(p => p.Player);
            return View(posts.ToList());
        }

        //
        // GET: /Post/Details/5

        public ViewResult Details(int id)
        {
            Post post = db.Posts.Find(id);
            return View(post);
        }

        //
        // GET: /Post/Create

        public ActionResult Create()
        {
            ViewBag.PlayerId = new SelectList(db.Players, "PlayerID", "Name");
            return View();
        } 

        //
        // POST: /Post/Create

        [HttpPost]
        public ActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.PlayerId = new SelectList(db.Players, "PlayerID", "Name", post.PlayerId);
            return View(post);
        }
        
        //
        // GET: /Post/Edit/5
 
        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
            ViewBag.PlayerId = new SelectList(db.Players, "PlayerID", "Name", post.PlayerId);
            return View(post);
        }

        //
        // POST: /Post/Edit/5

        [HttpPost]
        public ActionResult Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlayerId = new SelectList(db.Players, "PlayerID", "Name", post.PlayerId);
            return View(post);
        }

        //
        // GET: /Post/Delete/5
 
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            return View(post);
        }

        //
        // POST: /Post/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
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