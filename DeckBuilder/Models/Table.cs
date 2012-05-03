using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System.IO.Compression;

using DeckBuilder.Protoplasm;
using DeckBuilder.Helpers;
using DeckBuilder.Games;

using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Web.Hosting;
using DeckBuilder.Controllers;

namespace DeckBuilder.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public virtual ICollection<Seat> Seats { get; set; }

        public bool Finished { get; set; }

        // Serialized Game State
        public string GameState { get; set; }

        public int GameId { get; set; }
        public virtual Game Game { get; set; }

        public void GenerateInitialState()
        {
            if (Game.Name == "Geomancer")
            {
                GeomancerState gameState = new GeomancerState();  
                gameState.InitializeState(Seats.ToList());
                GameState = Compression.CompressGameState(gameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GeomancerState), new Type[] { typeof(GeomancerTile), typeof(GeomancerUnit), typeof(GeomancerCard), typeof(GeomancerCrystal), typeof(GeomancerSpell) }));
            }
            else if (Game.Name == "Onslaught")
            {
                OnslaughtState gameState = new OnslaughtState();
                gameState.InitializeState(Seats.ToList());
                GameState = Compression.CompressGameState(gameState, new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(OnslaughtState), new Type[] { typeof(OnslaughtPlayerContext), typeof(GalaxyCard), typeof(SupplyPile), typeof(InvasionCard), typeof(InvaderToken) }));
            }
            else if (Game.Name == "Mechtonic")
            {
                MechtonicState gameState = new MechtonicState();
                gameState.InitializeState(Seats.ToList());
                GameState = Compression.CompressGameState(gameState, MechtonicState.GetSerializer());
            }
            else
            {
                TableController.InitScriptEngine();
                TableController.LoadModules(Game.Name, Game.PythonScript);

                ScriptScope runScope = TableController.engine.CreateScope();
                runScope.ImportModule("cPickle");
                runScope.ImportModule("gamestate");
                runScope.ImportModule(Game.Name);

                // Input array of seats.
                Seat[] seatsArray = Seats.ToArray();
                runScope.SetVariable("seats", seatsArray);                

                ScriptSource runSource = TableController.engine.CreateScriptSourceFromString("pickledState = cPickle.dumps("+Game.Name+".Init(seats),0)", SourceCodeKind.Statements);                

                runSource.Execute(runScope);


                GameState = Compression.CompressStringState(runScope.GetVariable("pickledState"));
            }

        }
    }
}