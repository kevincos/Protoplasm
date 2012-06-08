using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeckBuilder.Models;
using DeckBuilder.ViewModels;
using DeckBuilder.Async;

using SignalR.Infrastructure;
using SignalR;
using SignalR.Hosting.AspNet;

namespace DeckBuilder.Controllers
{
    public class NotificationController : Controller
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        public ActionResult Mark(int id)
        {
            Notification n = db.Notifications.Find(id);
            n.Read = true;
            db.SaveChanges();
            return RedirectToAction("Notifications", "Player", new { id = n.PlayerID });
        }

        public ActionResult Remove(int id)
        {
            Notification n = db.Notifications.Find(id);
            db.Notifications.Remove(n);
            db.SaveChanges();
            return RedirectToAction("Notifications", "Player", new { id = n.PlayerID });            
        }

        public JsonResult ActiveRemove(int id)
        {
            Notification n = db.Notifications.Find(id);
            db.Notifications.Remove(n);
            db.SaveChanges();
            return Json(n.PlayerID);
        }

        public ViewResult Index()
        {
            return View(db.Notifications.ToList());
        }        
    }
}
