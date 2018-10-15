using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayesClassifier
{
    public static class DictionaryHelper 
    {
        public static Dictionary<string,int> MergeDictionaries(params Dictionary<string,int>[] dicts)
        {
            return dicts
              .SelectMany(d => d)
              .GroupBy(
                kvp => kvp.Key,
                (key, kvps) => new { Key = key, Value = kvps.Sum(kvp => kvp.Value) })
              .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
