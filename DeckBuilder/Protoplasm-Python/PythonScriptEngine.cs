using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Web.Hosting;

namespace DeckBuilder.Protoplasm_Python
{
    public class PythonScriptEngine
    {
        public static ScriptEngine engine = null;
        public static List<string> loadedModules = null;
        public static List<string> errors = null;

        public static void InitScriptEngine()
        {
            if (engine == null || loadedModules == null || errors == null)
            {
                engine = Python.CreateEngine();
                var paths = engine.GetSearchPaths();
                paths.Add(HostingEnvironment.MapPath(@"~/Python"));
                paths.Add(HostingEnvironment.MapPath(@"~/Protoplasm-Python"));
                engine.SetSearchPaths(paths);
                loadedModules = new List<string>();
                errors = new List<string>();
            }
        }
        public static void LoadModules(string moduleName, string code)
        {
            if (!loadedModules.Contains(moduleName))
            {
                ScriptScope moduleScope = engine.CreateModule(moduleName);
                ScriptSource moduleSource = engine.CreateScriptSourceFromString(code, SourceCodeKind.File);
                moduleSource.Execute(moduleScope);
            }
        }
    }
}