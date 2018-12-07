using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;

namespace PartyAffiliationClassifier
{
    public class UserInteractionController
    {
        private Network _network = new Network();
        public UserInteractionController()
        {
            _network = _network.GetNetwork();
        }

        /// <summary>
        /// Main loop for classifier application
        /// </summary>
        public void Start()
        {
            ConsoleKey choiceInput;
            Start:
            Console.Clear();
            Console.WriteLine("Party Affiliation Classifier\n\nChoose an option from the menu below\n\n1. Train\n2. Classify\n\n" +
                "You can hit the escape key to exit");
            Console.Write("Enter your choice: ");
            choiceInput = Console.ReadKey().Key;
            switch (choiceInput)
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
        private void Train()
        {
            StartTraining: 
            string docChoice;
            ConsoleKey choiceInput;
            int choice;
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
                Console.Write("Choose an option (Type 00 to go back): ");
                docChoice = Console.ReadLine();
                if (docChoice == "00") return;
                try
                {
                    choice = Convert.ToInt32(docChoice);

                    if (trainingDocPairs.TryGetValue(choice, out string value))
                    {
                        validInput = true;
                        t = AssignDocumentParty(value);
                        if (t == null) goto StartTraining;
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

            _network.AddTrainingDoc(t);

            Console.WriteLine("Trained Network with \"{0}\"", t.FileName);
            Console.ReadLine();
            Console.Clear();
        SavePrompt:
            Console.WriteLine("Save the network to file?");
            Console.WriteLine("1. Yes\n2. No\nHit enter after you have made your choice");
            choiceInput = Console.ReadKey().Key;
            switch (choiceInput)
            {
                case ConsoleKey.D1:
                    _network.Save();
                    Classify();
                    break;
                case ConsoleKey.D2:
                    goto StartTraining;
                default:
                    goto SavePrompt;
            }

            return;

        }

        /// <summary>
        /// Classify an unknown document
        /// </summary>
        private void Classify()
        {
            string[] docs = Directory.GetFiles("UnknownDocs");
            Dictionary<int, string> unknownDocPairs = new Dictionary<int, string>();
            TextInfo ti = new CultureInfo("en-UK", false).TextInfo;
            string choice;
            int menuChoice;

            if (_network.TotalDocs == 0)
            {
                Console.Clear();
                Console.WriteLine("Cannot classify documents without training data, please train network first");
                Console.ReadLine();
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

                Console.Write("\nChoose a document by entering its number (Type 00 to go back): ");
                choice = Console.ReadLine();
                if (choice == "00") return;
                try
                {
                    menuChoice = Convert.ToInt32(choice);

                    if (unknownDocPairs.TryGetValue(menuChoice, out string value))
                    {
                        Console.Clear();
                        Console.WriteLine($"Classifying {value}\n");
                        Doc doc = new Doc($"UnknownDocs\\{value}");
                        Dictionary<string,ClassificationResults> results = _network.ClassifyUnknownDocument(doc);
                        Console.WriteLine($"\nClassified as {ti.ToTitleCase(results["normal"].MostLikely.ToString().ToLower())} Party - {results["normal"].GetPartyPercentage(results["normal"].MostLikely):f2}% certainty using word stemming and removing stopwords");
                        foreach(KeyValuePair<Category,double> r in results["normal"].GetOthers)
                        {
                            Console.WriteLine($"{ti.ToTitleCase(r.Key.ToString().ToLower())} - {r.Value:f2}% certainty");
                        }
                        Console.WriteLine($"\nClassified as {ti.ToTitleCase(results["ngram"].MostLikely.ToString().ToLower())} Party - {results["ngram"].GetPartyPercentage(results["ngram"].MostLikely):f2}% certainty using word-level trigrams");
                        foreach (KeyValuePair<Category, double> r in results["ngram"].GetOthers)
                        {
                            Console.WriteLine($"{ti.ToTitleCase(r.Key.ToString().ToLower())} - {r.Value:f2}% certainty");
                        }
                        Console.WriteLine($"\nClassified as {ti.ToTitleCase(results["tfidf"].MostLikely.ToString().ToLower())} Party - {results["tfidf"].GetPartyPercentage(results["tfidf"].MostLikely):f2}% certainty using TF-IDF");
                        foreach (KeyValuePair<Category, double> r in results["tfidf"].GetOthers)
                        {
                            Console.WriteLine($"{ti.ToTitleCase(r.Key.ToString().ToLower())} - {r.Value:f2}% certainty");
                        }
                        doc.Dispose();
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
        private TrainingDoc AssignDocumentParty(string fileName)
        {
            ConsoleKey choiceInput;
            int choice;
            Console.Clear();
            TrainingDoc td = new TrainingDoc();
            Console.WriteLine("Choose the party this document belongs to:\n");
            Dictionary<int, Category> categoryPairs = new Dictionary<int, Category>() { { 1, Category.CONSERVATIVE }, { 2, Category.COALITION }, { 3, Category.LABOUR } };
            foreach (KeyValuePair<int, Category> pair in categoryPairs)
            {
                Console.WriteLine($"{pair.Key}. {pair.Value}");
            }
            Console.Write("Enter an option: ");
            choiceInput = Console.ReadKey().Key;
            switch (choiceInput)
            {
                case ConsoleKey.D1:
                    td = new TrainingDoc(fileName, Category.CONSERVATIVE);
                    break;
                case ConsoleKey.D2:
                    td = new TrainingDoc(fileName, Category.COALITION);
                    break;
                case ConsoleKey.D3:
                    td = new TrainingDoc(fileName, Category.LABOUR);
                    break;
                case ConsoleKey.Escape:
                    return null;
                default:
                    Console.WriteLine("Invalid Input!");
                    break;
            }
            return td;
        }


    }
}
