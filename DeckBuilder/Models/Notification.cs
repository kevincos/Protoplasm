using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }

        public int PlayerID { get; set; }
        public int TableID { get; set; }

        public String Message { get; set; }
        public String Url { get; set; }
        public bool Read { get; set; }
        public bool Suppressed { get; set; }
        public DateTime DatePosted { get; set; }
    }
}