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
    public enum TableState
    {
        Proposed=0,
        Cancelled=1,
        InPlay=2,
        Complete=3
    }

    public class Table
    {
        public int TableID { get; set; }
        public virtual ICollection<Seat> Seats { get; set; }

        public bool Finished { get; set; }

        // Serialized Game State
        public string GameState { get; set; }

        public int GameId { get; set; }
        public virtual Game Game { get; set; }

        public virtual GameVersion Version { get; set; }

        public int TableState { get; set; }

        public void GenerateInitialState()
        {
            TableController.InitScriptEngine();
            TableController.LoadModules(Game.Name, Version.PythonScript);

            ScriptScope runScope = TableController.engine.CreateScope();
            runScope.ImportModule("cPickle");
            runScope.ImportModule("protoplasm");
            runScope.ImportModule(Game.Name);

            // Input array of seats.
            Seat[] seatsArray = Seats.ToArray();
            runScope.SetVariable("seats", seatsArray);

            ScriptSource runSource = TableController.engine.CreateScriptSourceFromString("initialState = " + Game.Name + ".Init(seats);pickledState = cPickle.dumps(initialState,0)", SourceCodeKind.Statements);                

            runSource.Execute(runScope);


            GameState = Compression.CompressStringState(runScope.GetVariable("pickledState"));
        }
    }
}