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
    public class TableController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        //
        // GET: /Table/

        public ViewResult Index()
        {
            return View(db.Tables.ToList());
        }

        //
        // GET: /Table/Details/5

        public ViewResult Details(int id)
        {
            Table table = db.Tables.Where(t => t.TableID == id).Include("Seats").Single();
            return View(table);
        }

        //
        // GET: /Table/Play/5
        [Authorize]
        public ActionResult Play(int id)
        {
            Table table = db.Tables.Where(t => t.TableID == id).Include("Seats").Single();

            List<Seat> seats = db.Seats.ToList();
            List<string> playerNames = seats.Select(s => s.Player.Name).ToList();
            if (!playerNames.Contains(User.Identity.Name))
            {
                return RedirectToAction("Details", "Table", new { id = id });
            }

            // Case 1 - Player is active
            Seat currentSeat = table.Seats.Where(s => s.Player.Name == User.Identity.Name).Single();

            ViewBag.YourTurn = currentSeat.Active;
            ViewBag.PlayerName = User.Identity.Name;
            ViewBag.TableId = id;
            if (table.TotalTurns > 0)
            {
                ViewBag.Results = table.Results;
            }
            if (table.Finished)
            {
                ViewBag.FinalResults = table.FinalResults;
            }
            ViewBag.Finished = table.Finished;
            ViewBag.PlayerScore = currentSeat.Wins;

            // ASSUMES ONLY 2 PLAYERS
            Seat opponentSeat = table.Seats.Where(s => s.Player.Name != User.Identity.Name).Single();
            ViewBag.OpponentName = opponentSeat.Player.Name;
            ViewBag.OpponentScore = opponentSeat.Wins;

            if (table.Finished == false)
            {
                if (currentSeat.Active == true)
                {
                    ViewBag.Message = "YOUR TURN!";
                    ViewBag.MoveList = new SelectList(new List<string> { "Rock", "Paper", "Scissors" });
                }
                else
                {
                    ViewBag.Message = "You've played " + currentSeat.LastMove + "! Waiting for opponent...";
                }
            }
            return View(table);
        }

        //
        // GET: /Table/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Table/Create

        [HttpPost]
        public ActionResult Create(Table table)
        {
            if (ModelState.IsValid)
            {
                db.Tables.Add(table);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(table);
        }
        
        //
        // GET: /Table/Edit/5
 
        public ActionResult Edit(int id)
        {
            Table table = db.Tables.Find(id);
            return View(table);
        }

        //
        // POST: /Table/Edit/5

        [HttpPost]
        public ActionResult Edit(Table table)
        {
            if (ModelState.IsValid)
            {
                db.Entry(table).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(table);
        }

        //
        // GET: /Table/Delete/5
 
        public ActionResult Delete(int id)
        {
            Table table = db.Tables.Find(id);
            return View(table);
        }

        //
        // POST: /Table/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Table table = db.Tables.Find(id);
            db.Tables.Remove(table);
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