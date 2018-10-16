using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyAffiliationClassifier
{
    public static class DictionaryHelper 
    {
        public static Dictionary<string,int> MergeDictionaries(params Dictionary<string,int>[] dictionaries)
        {
            return dictionaries
              .SelectMany(d => d)
              .GroupBy(
                pair => pair.Key,
                (key, pairs) => new { Key = key, Value = pairs.Sum(pair => pair.Value) })
              .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
