using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Protoplasm_Python;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Web.Hosting;

namespace DeckBuilder.Stats
{
    public enum GraphType
    {                
        PercentOverItems=0,
        PercentOverGames=1,
        CountOverItems=2,
        CountOverGames = 3,                
        AverageOverItems=4,
        AverageOverGames = 5,
        SumOverItems = 6,
        SumOverGames = 7,                
    }

    public enum TargetType
    {
        KeyValue = 0,
        IndexedKey = 1,
        DictKey= 2
    }

    public class DataSet
    {
        public string Name;
        public int[] Data;

        public DataSet(String name, int length)
        {
            this.Name = name;
            this.Data = new int[length];
        }
    }

    public class Target
    {
        public int type { get; set; }
        public string key { get; set; }
        public string index { get; set; }
        public string dictKey { get; set; }
    }



    public class Graph
    {
        public List<String> dataSets { get; set; }          
        public List<String> independentAxis { get; set; }

        public string itemName { get; set; }

        public Target target { get; set; }

        public List<Condition> primaryConditionList { get; set; }
        public List<Condition> secondaryConditionList { get; set; }
        public int type { get; set; }

        public Graph(List<String> axis)
        {
            this.dataSets = new List<string>();
            this.independentAxis = axis;
        }

        public Graph()
        {
        }
        
        public void PercentOverItems(String itemName, List<Condition> primaryConditionList, List<Condition> secondaryConditionList)
        {
            this.itemName = itemName;
            this.primaryConditionList = primaryConditionList;
            this.secondaryConditionList = secondaryConditionList;
            this.type = (int)GraphType.PercentOverItems;
        }

        public void AddDataSet(String name)
        {           
            dataSets.Add(name);
        }

        public List<List<float>> GenerateData(String sourceData)
        {
            if (primaryConditionList == null)
                primaryConditionList = new List<Condition>();
            if (secondaryConditionList == null)
                secondaryConditionList = new List<Condition>();
            PythonScriptEngine.InitScriptEngine(false);
            ScriptScope runScope = PythonScriptEngine.engine.CreateScope();
            runScope.ImportModule("protoplasm_stats");
            runScope.ImportModule("json");
            ScriptSource runSource = null;
            runScope.SetVariable("source_json", sourceData);

            runScope.SetVariable("primary_condition", primaryConditionList);
            runScope.SetVariable("secondary_condition", secondaryConditionList);
            runScope.SetVariable("item_name", itemName);
            runScope.SetVariable("target", target);
            List<List<float>> results = new List<List<float>>();
            for(int j = 0; j < dataSets.Count; j++)
            {
                runScope.SetVariable("data_set_name", dataSets[j]);
                results.Add(new List<float>());     
                for (int i = 0; i < independentAxis.Count; i++)
                {
                    runScope.SetVariable("independent_variable", independentAxis[i]);
                    if (i - 1 >= 0)
                        runScope.SetVariable("prev_independent_variable", independentAxis[i - 1]);
                    else
                        runScope.SetVariable("prev_independent_variable", "-inf");
                        
                    // Access source Data                        
                    if (type == (int)GraphType.PercentOverItems)
                    {
                        runSource = PythonScriptEngine.engine.CreateScriptSourceFromString("result = protoplasm_stats.percent_over_items(source_json, item_name,secondary_condition,primary_condition,independent_variable,prev_independent_variable,data_set_name)", SourceCodeKind.Statements);
                    }
                    else if (type == (int)GraphType.PercentOverGames)
                    {
                        runSource = PythonScriptEngine.engine.CreateScriptSourceFromString("result = protoplasm_stats.percent_over_games(source_json, secondary_condition,primary_condition,independent_variable,prev_independent_variable,data_set_name)", SourceCodeKind.Statements);
                    }
                    else if (type == (int)GraphType.AverageOverItems)
                    {
                        runSource = PythonScriptEngine.engine.CreateScriptSourceFromString("result = protoplasm_stats.average_over_items(source_json, item_name,target, primary_condition,independent_variable,prev_independent_variable,data_set_name)", SourceCodeKind.Statements);
                    }
                    else if (type == (int)GraphType.AverageOverGames)
                    {
                        runSource = PythonScriptEngine.engine.CreateScriptSourceFromString("result = protoplasm_stats.average_over_games(source_json, target, primary_condition,independent_variable,prev_independent_variable,data_set_name)", SourceCodeKind.Statements);
                    }
                    else if (type == (int)GraphType.SumOverItems)
                    {
                        runSource = PythonScriptEngine.engine.CreateScriptSourceFromString("result = protoplasm_stats.sum_over_items(source_json, item_name,target, primary_condition,independent_variable,prev_independent_variable,data_set_name)", SourceCodeKind.Statements);                        
                    }
                    else if (type == (int)GraphType.SumOverGames)
                    {
                        runSource = PythonScriptEngine.engine.CreateScriptSourceFromString("result = protoplasm_stats.sum_over_games(source_json, target, primary_condition,independent_variable,prev_independent_variable,data_set_name)", SourceCodeKind.Statements);
                    }
                    else if (type == (int)GraphType.CountOverItems)
                    {
                        runSource = PythonScriptEngine.engine.CreateScriptSourceFromString("result = protoplasm_stats.count_over_items(source_json, item_name, primary_condition,independent_variable,prev_independent_variable,data_set_name)", SourceCodeKind.Statements);
                    }
                    else if (type == (int)GraphType.CountOverGames)
                    {
                        runSource = PythonScriptEngine.engine.CreateScriptSourceFromString("result = protoplasm_stats.count_over_games(source_json, primary_condition,independent_variable,prev_independent_variable,data_set_name)", SourceCodeKind.Statements);
                    }
                    runSource.Execute(runScope);
                    var r = runScope.GetVariable("result");
                    results[j].Add((float)r);                
                }

            }
            return results;
        }
    }
}