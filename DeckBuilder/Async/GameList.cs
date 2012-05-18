using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SignalR.Hubs;
using System.Web.Mvc;
using System.Security.Principal;
using DeckBuilder.Models;

namespace DeckBuilder.Async
{
    public class GameList : Hub
    {
        private DeckBuilderContext db = new DeckBuilderContext();

        public void EnterGame(string name)
        {
            Caller.name = name;
            AddToGroup(name);
        }

        public void Chat(int tableId, string chatText)
        {
            
            Table table = db.Tables.Find(tableId);
            String fullMessage = DateTime.Now.Hour + ":" + DateTime.Now.Minute + "  " + Context.User.Identity.Name + ": " + chatText;
            table.ChatRecord+=fullMessage+"<br/>";
            if (table.ChatRecord.Length > 1024) table.ChatRecord = table.ChatRecord.Substring(table.ChatRecord.Length - 1024);
            db.SaveChanges();
            foreach (Seat s in table.Seats)
            {
                Clients[s.Player.Name + tableId].update_chat(fullMessage);
            }
        }
    }
}