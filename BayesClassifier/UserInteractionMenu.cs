using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PartyAffiliationClassifier
{
    public class UserInteractionMenu
    {
        public Dictionary<int, string> UnknownDocPairs;
        public UserInteractionMenu()
        {
            var docs = Directory.GetFiles("UnknownDocs");
            UnknownDocPairs = new Dictionary<int, string>();
            for (int i = 0; i < docs.Count(); i++)
            {
                UnknownDocPairs[i + 1] = docs[i].Replace("UnknownDocs\\", "");
            }
        }
        
        public void Go()
        {
            foreach(var pair in UnknownDocPairs)
            {
                Console.WriteLine($"{pair.Key}. {pair.Value}");
            }

            Console.Write("Choose a document by entering its number: ");
            var choice = Console.ReadLine();
            if(UnknownDocPairs.TryGetValue(Convert.ToInt32(choice), out string value))
            {
                Console.WriteLine($"TrainingDocs\\{value}");
            }
        }
    }
}
