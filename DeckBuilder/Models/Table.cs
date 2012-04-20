using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System.IO.Compression;

using DeckBuilder.Helpers;
using DeckBuilder.Games;

namespace DeckBuilder.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public virtual ICollection<Seat> Seats { get; set; }

        public bool Finished { get; set; }

        // Serialized Game State
        public string GameState { get; set; }

        public string Game { get; set; }

        public void GenerateInitialState()
        {
            if (Game == "Geomancer")
            {
                GeomancerState gameState = new GeomancerState();  
                gameState.InitializeState(Seats.ToList());
                GameState = Compression.CompressGameState(gameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) }));
            }
            else if (Game == "RPS")
            {
                RPSState gameState = new RPSState();
                gameState.InitializeState(Seats.ToList());
                GameState = Compression.CompressGameState(gameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(RPSState), new Type[] { typeof(RPSPlayerContext)}));                
            }
            else if (Game == "Onslaught")
            {
                OnslaughtState gameState = new OnslaughtState();
                gameState.InitializeState(Seats.ToList());
                GameState = Compression.CompressGameState(gameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(OnslaughtState), new Type[] { typeof(OnslaughtPlayerContext), typeof(GalaxyCard), typeof(SupplyPile), typeof(InvasionCard), typeof(InvaderToken) }));
            }
            else if (Game == "Connect4")
            {
                Connect4State gameState = new Connect4State();
                gameState.InitializeState(Seats.ToList());
                GameState = Compression.CompressGameState(gameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Connect4State), new Type[] { typeof(Connect4PlayerContext), typeof(Connect4Update)}));
            }
            else if (Game == "CanyonConvoy")
            {
                ConvoyState gameState = new ConvoyState();
                gameState.InitializeState(Seats.ToList());
                GameState = Compression.CompressGameState(gameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ConvoyState), new Type[] { typeof(ConvoyPlayerContext), typeof(ConvoyPiece), typeof(ConvoyTile), typeof(ConvoyUpdate) }));
            }
            else if (Game == "Mechtonic")
            {
                MechtonicState gameState = new MechtonicState();
                gameState.InitializeState(Seats.ToList());
                GameState = Compression.CompressGameState(gameState, MechtonicState.GetSerializer());
            }   
            else
            {
                throw new Exception("Invalid game: " + Game);
            }

        }
    }
}