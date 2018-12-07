using System;
using System.Collections.Generic;

namespace PartyAffiliationClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Party Affiliation Classifier";
            //Instantiate new UserInteractionMenu object and Start
            new UserInteractionController().Start();
        }
    }
}
