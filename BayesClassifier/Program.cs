using System;

namespace PartyAffiliationClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Party Affiliation Classifier";
            Console.CursorVisible = false;
            //Instantiate new UserInteractionMenu object and Start
            new UserInteractionHandler().Start();
        }
    }
}
