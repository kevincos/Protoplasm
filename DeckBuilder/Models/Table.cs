﻿using System;
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
using DeckBuilder.Protoplasm_Python;

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

        public String ChatRecord { get; set; }

        public bool SoloPlayTest { get; set; }
        public bool Ranked { get; set; }
        public bool Alpha { get; set; }

        //public DateTime LastUpdateTime { get; set; }


        public void GenerateInitialState()
        {
            PythonScriptEngine.InitScriptEngine(SoloPlayTest == true || Alpha == true);
            PythonScriptEngine.LoadModules(Version.ModuleName, Version.PythonScript, SoloPlayTest == true || Alpha == true, Version.GameVersionID);


            ScriptScope runScope = PythonScriptEngine.GetScope(SoloPlayTest == true || Alpha == true);
            runScope.ImportModule("cPickle");
            runScope.ImportModule("protoplasm");
            runScope.ImportModule(Version.ModuleName + Version.GameVersionID);

            // Input array of seats.
            Seat[] seatsArray = Seats.ToArray();
            runScope.SetVariable("seats", seatsArray);

            string error = PythonScriptEngine.RunCode(runScope, "initialState = " + Version.ModuleName + Version.GameVersionID + ".Init(seats);pickledState = cPickle.dumps(initialState,0)", SoloPlayTest || Alpha);
            
            ChatRecord = "";

            GameState = Compression.CompressStringState(runScope.GetVariable("pickledState"));
        }
    }
}