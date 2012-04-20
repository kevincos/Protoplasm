using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Models
{
    public class Post
    {
        public int PostID { get; set; }

        public string Title { get; set; }

        public int PlayerId { get; set; }
        public virtual Player Player { get; set; }

        public DateTime Date { get; set; }

        public string Content { get; set; }
    }
}