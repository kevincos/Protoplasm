using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Stats
{
    public enum ConditionType
    {
        MatchAll=0,
        KeyValue=1,
        IndexedValue=2,
        DictValue=3,
        KeyValueRange=4,
        IndexedValueRange=5,
        DictValueRange=6
    }

    public class Condition
    {
        public string key { get; set; }
        public string value { get; set; }
        public string index { get; set; }
        public string dictKey { get; set; }
        public string min { get; set; }
        public string max { get; set; }
        public int type { get; set; }

    }
}