using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DeckBuilder.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        public int SourceId { get; set; }
        public virtual Player Source { get; set; }

        public int designRating { get; set; }
        public string designComments { get; set; }

        public int clarityRating { get; set; }
        public string clarityComments { get; set; }

        public int aestheticsRating { get; set; }
        public string aestheticsComments { get; set; }

        public int stabilityRating { get; set; }
        public string stabilityComments { get; set; }

    }
}