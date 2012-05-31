using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Debugging;
using System.Web.Hosting;

using IronPython;
using IronPython.Modules;
using IronPython.Hosting;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Types;
using IronPython.Runtime;
using DeckBuilder.Models;

namespace DeckBuilder.Protoplasm_Python
{
    public class StackContext
    {
        public string Exception { get; set; }
        public int Line { get; set; }
        public string FileName { get; set; }
        public string Code { get; set; }
        public string Function { get; set; }

        public override String ToString()
        {
            if ((Function == "" || Function == null || Function == "<module>" || Function == "<string>"))
                return Code + "   (from " + FileName + " line: " + Line + ")";
            else
                return Code + "   (from " + Function + " in " + FileName + " line: " + Line + ")";
        }
    }

    public class PythonScriptEngine
    {
        public static ScriptEngine engine = null;
        
        public static Dictionary<string,int> loadedModules = null;
        
        public static List<string> errors = null;
        
        public static Stack<StackContext> stackTrace = new Stack<StackContext>();
        public static TraceBackFrame lastFrame = null;

        public static DeckBuilderContext db = new DeckBuilderContext();

        public static TracebackDelegate OnTraceback(TraceBackFrame frame, string result, object payload)
        {
            FunctionCode code = (FunctionCode)frame.f_code;
            if (result == "exception")
            {
                if (frame != lastFrame)
                {
                    int line_no = (int)frame.f_lineno;
                    String module_name = code.co_filename.Replace(".py", "");
                    String line = "Unknown";
                    try
                    {
                        line = System.IO.File.ReadAllLines(code.co_filename)[line_no - 1].Replace("\t", "");
                    }
                    catch
                    {
                        // GET MODULE CODE
                        if (module_name != "<string>")
                        {
                            GameVersion version = db.Versions.Find(loadedModules[module_name]);
                            String moduleCode = version.PythonScript;
                            string[] lines = moduleCode.Split(new string[] { "\n" }, StringSplitOptions.None);
                            line = lines[line_no - 1].Replace("\t", "");
                        }
                    }
                    PythonTuple tuple = (PythonTuple)payload;
                    PythonType type = (PythonType)tuple[0];
                    stackTrace.Push(new StackContext { Line = line_no, Code = line, Exception = PythonType.Get__name__(type), Function = code.co_name, FileName = code.co_filename });
                }
                lastFrame = frame;
            }            
            if (result == "line")
            {
                stackTrace = new Stack<StackContext>();                
            }
            return PythonScriptEngine.OnTraceback;
        }


        public static void InitScriptEngine(bool debug)
        {
            if (engine == null || loadedModules == null || errors == null)
            {
                engine = Python.CreateEngine();
                engine.SetTrace(null);
                var paths = engine.GetSearchPaths();
                paths.Add(HostingEnvironment.MapPath(@"~/Python"));
                paths.Add(HostingEnvironment.MapPath(@"~/Protoplasm-Python"));
                engine.SetSearchPaths(paths);
                loadedModules = new Dictionary<string,int>();
                errors = new List<string>();
            }
        }
        public static void LoadModules(string moduleName, string code, bool debug, int versionId)
        {
            if (debug == true)
            {
                loadedModules.Remove(moduleName + versionId);
            }
            if (!loadedModules.ContainsKey(moduleName + versionId))
            {
                loadedModules.Add(moduleName + versionId, versionId);
                ScriptScope moduleScope = engine.CreateModule(moduleName + versionId);
                ScriptSource moduleSource = engine.CreateScriptSourceFromString(code, moduleName + versionId + ".py", SourceCodeKind.File);
                moduleSource.Execute(moduleScope);
            }            
        }

        public static ScriptScope GetScope(bool debug)
        {
            InitScriptEngine(debug);
            return engine.CreateScope();
        }

        public static string RunCode(ScriptScope runScope, string source, bool debug)
        {
            engine.SetTrace(null);

            ScriptSource runSource = engine.CreateScriptSourceFromString(source, SourceCodeKind.File);

            String result = "";
      
            try
            {
                runSource.Execute(runScope);
            }
            catch (Exception e)
            {                
                errors.Add(e.ToString());
                if (debug == true)
                {
                    engine.SetTrace(PythonScriptEngine.OnTraceback);
                    try
                    {
                        runSource.Execute(runScope);
                    }
                    catch (Exception e2)
                    {
                        errors.Add(e2.ToString());
                        result += stackTrace.Peek().Exception + " Exception: " + e2.Message + "<br/><br/>";
                        foreach(StackContext context in stackTrace.Reverse())
                        {
                            result += context.ToString() + "<br/>";
                        }
                    }
                }
            }

            engine.SetTrace(null);
            return result;
        }

        public static void ForceRunCode(ScriptScope runScope, string source, bool debug, int attempts)
        {
            engine.SetTrace(null);

            ScriptSource runSource = engine.CreateScriptSourceFromString(source, SourceCodeKind.File);

  
            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    runSource.Execute(runScope);
                    break;
                }
                catch (Exception e)
                {
                    errors.Add(e.ToString());
                    runSource.Execute(runScope);
                }
            }

        }

        public static void RunCode(ScriptScope runScope, string source)
        {
            RunCode(runScope, source, false);
        }
    }
}