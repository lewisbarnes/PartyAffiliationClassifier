using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;

namespace PartyAffiliationClassifier
{
    public class UserInteractionController
    {
        Network network = new Network();
        public UserInteractionController()
        {
            network = network.GetNetwork();
        }

        /// <summary>
        /// Main loop for classifier application
        /// </summary>
        public void Start()
        {
            Start:
            Console.Clear();
            Console.Write("Party Affiliation Classifier\n\nChoose an option from the menu below\n\n1. Train\n2. Classify\n\n" +
                "You can hit the escape key to exit\n\nHit the desired number key");
            switch (Console.ReadKey(false).Key)
            {
                case ConsoleKey.D1:
                    Train();
                    break;
                case ConsoleKey.D2:
                    Classify();
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
                default:
                    goto Start;
            }
            goto Start;
        }

        /// <summary>
        /// Train the network with new data
        /// </summary>
        public void Train()
        {
            bool validInput = false;
            TrainingDoc t = new TrainingDoc();
            do
            {
                Console.Clear();
                Console.WriteLine("Choose a document to train the network with\n");

                string[] docs = Directory.GetFiles("TrainingDocs");

                Dictionary<int, string> trainingDocPairs = new Dictionary<int, string>();
                Dictionary<int, string> trainingDocPairDisplay = new Dictionary<int, string>();

                for (int i = 0; i < docs.Count(); i++)
                {
                    trainingDocPairs[i + 1] = docs[i];
                    trainingDocPairDisplay[i + 1] = docs[i].Replace("TrainingDocs\\", "");
                }

                foreach (KeyValuePair<int, string> pair in trainingDocPairDisplay)
                {
                    Console.WriteLine($"{pair.Key}. {pair.Value}");
                }

                string choiceInput = Console.ReadLine();

                try
                {
                    int choice = Convert.ToInt32(choiceInput);

                    if (trainingDocPairs.TryGetValue(choice, out string value))
                    {
                        validInput = true;
                        t = AssignDocumentParty(value);
                    }
                    else
                    {
                        Console.WriteLine("Invalid option chosen!");
                        Console.ReadLine();
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message + " Try a number instead!");
                    Console.ReadLine();
                }
            } while (!validInput);

            network.AddTrainingDoc(t);

            Console.WriteLine("Trained Network with \"{0}\"", t.FileName);
            Console.ReadLine();

            return;

        }

        /// <summary>
        /// Classify an unknown document
        /// </summary>
        public void Classify()
        {
            string[] docs = Directory.GetFiles("UnknownDocs");
            Dictionary<int, string> unknownDocPairs = new Dictionary<int, string>();
            TextInfo ti = new CultureInfo("en-UK", false).TextInfo;
            string choice;
            int menuChoice;

            if (network.TotalDocs == 0)
            {
                Console.WriteLine("Cannot classify document without training data, must train network first");
                Start();
            }

            for (int i = 0; i < docs.Count(); i++)
            {
                unknownDocPairs[i + 1] = docs[i].Replace("UnknownDocs\\", "");
            }

            do
            {
                Console.Clear();
                foreach (KeyValuePair<int, string> pair in unknownDocPairs)
                {
                    Console.WriteLine($"{pair.Key}. {pair.Value}");
                }

                Console.Write("\nChoose a document by entering its number: ");
                choice = Console.ReadLine();



                try
                {
                    menuChoice = Convert.ToInt32(choice);

                    if (unknownDocPairs.TryGetValue(menuChoice, out string value))
                    {
                        Doc doc = new Doc($"UnknownDocs\\{value}");
                        ClassificationResults result = network.ClassifyUnknownDocument(doc);
                        double[] percentages = new double[] { result.GetPartyPercentage(Category.CONSERVATIVE), result.GetPartyPercentage(Category.COALITION), result.GetPartyPercentage(Category.LABOUR) };
                        percentages = percentages.OrderByDescending(x => x).ToArray();
                        Console.WriteLine($"\nClassified as {ti.ToTitleCase(result.MostLikely.ToString().ToLower())} Party - {result.GetPartyPercentage(result.MostLikely):f2}%");
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message + " Try a number instead!");
                    Console.ReadLine();
                }

                Console.ReadLine();

            } while (true);
        }
        /// <summary>
        /// Assign the party to the document
        /// </summary>
        /// <param name="fileName">The name of the file to assign the party to</param>
        /// <returns></returns>
        public TrainingDoc AssignDocumentParty(string fileName)
        {
            Console.Clear();
            TrainingDoc t = new TrainingDoc();
            Console.WriteLine("Choose the party this document belongs to\n\nYou can hit the escape key to go back");
            Dictionary<int, Category> categoryPairs = new Dictionary<int, Category>() { { 1, Category.CONSERVATIVE }, { 2, Category.COALITION }, { 3, Category.LABOUR } };
            foreach (KeyValuePair<int, Category> pair in categoryPairs)
            {
                Console.WriteLine($"{pair.Key}. {pair.Value}");
            }
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.D1:
                    t = new TrainingDoc(fileName, Category.CONSERVATIVE);
                    break;
                case ConsoleKey.D2:
                    t = new TrainingDoc(fileName, Category.COALITION);
                    break;
                case ConsoleKey.D3:
                    t = new TrainingDoc(fileName, Category.LABOUR);
                    break;
                case ConsoleKey.Escape:
                    Train();
                    break;
                default:
                    Console.WriteLine("Invalid Input!");
                    break;
            }
            return t;
        }


    }
}
