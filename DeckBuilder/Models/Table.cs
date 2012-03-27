using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace DeckBuilder.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public virtual ICollection<Seat> Seats { get; set; }

        public bool Finished { get; set; }

        // Serialized Game State
        public string GameState { get; set; }

        // Rock Paper Scissors Data
        public int TotalTurns { get; set; }
        public int Draws { get; set; }
        public string Results { get; set; }
        public string FinalResults { get; set; }

        public void GenerateInitialState()
        {
            GeomancerState gameState = new GeomancerState();
            if (Seats.Count == 2)
            {
                gameState.InitializeState(Seats.Select(s => s.Deck).ToList());
                
            }
            else
            {
                gameState.InitializeState();
            }

            // DATABASE COMPRESS : Compress initial state to save to database
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) });
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, gameState);
            ms.Close();
            MemoryStream compressStream = new MemoryStream(ms.ToArray());
            string minijson = "";
            using (var cmpStream = new MemoryStream())
            {
                using (var hgs = new GZipStream(cmpStream, CompressionMode.Compress))
                {
                    compressStream.CopyTo(hgs);
                }
                minijson = Encoding.Default.GetString(cmpStream.ToArray());
            }
            string json = Encoding.Default.GetString(ms.ToArray());

            GameState = minijson;

        }
    }
}