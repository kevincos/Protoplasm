using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Models
{
    public class Game
    {
        public int GameID { get; set; }
        
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }

        public int CreatorId { get; set; }
        public virtual Player Creator { get; set; }
        public virtual ICollection<Player> Testers { get; set; }

        public virtual ICollection<GameVersion> Versions { get; set; }

        public GameVersion LatestRelease
        {
            get
            {
                try
                {
                    return Versions.Where(v => v.DevStage == "Release").OrderBy(v => v.CreationDate).Last();
                }
                catch
                {
                    return null;
                }

            }
        }

        public GameVersion ActiveDev
        {
            get
            {
                try
                {
                    return Versions.Where(v => v.DevStage == "Alpha").OrderBy(v => v.CreationDate).First();
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}