using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Models
{
    public enum DevStage
    {
        Alpha,
        ClosedBeta,
        OpenBeta,
        Release        
    }

    public class GameVersion
    {
        public int GameVersionID { get; set; }

        public string VersionString { get; set; }

        public string ModuleName { get; set; }        

        public string PythonScript { get; set; }

        public int MaxPlayers { get; set; }

        public DevStage DevStage { get; set; }

        public DateTime CreationDate { get; set; }

        public virtual Game ParentGame { get; set; }

        public virtual ICollection<Table> CompletedGames { get; set; }
        public virtual ICollection<Table> ActiveGames { get; set; }
        public virtual ICollection<Table> ProposedGames { get; set; }

        public virtual ICollection<Feedback> Feedback { get; set; }
    }
}