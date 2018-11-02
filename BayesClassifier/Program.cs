using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PartyAffiliationClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Party Affiliation Classifier";
            // Instantiate new UserInteractionMenu object and Start
            new UserInteractionHandler().Start();
        }
    }
}
