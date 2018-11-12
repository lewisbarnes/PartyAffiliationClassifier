using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PartyAffiliationClassifier
{
    public class UserInteractionHandler
    {
        Network network;
        public UserInteractionHandler()
        {
            network = Network.GetNetwork();
        }
        
        public void Start()
        {
            bool validInput = false;

            do
            {
                Console.Clear();
                Console.WriteLine("Party Affiliation Classifier\n\nChoose an option from the menu below\n\n1. Train\n2. Classify\n3. Exit\n\nEnter number of option: ");
                Console.Write(">");
                var option = Console.ReadLine()[0];
                switch (option)
                {
                    case '1':
                        validInput = true;
                        Train();
                        break;
                    case '2':
                        validInput = true;
                        Classify();
                        break;
                    case '3':
                        validInput = true;
                        Console.Clear();
                        Console.Write("Exiting");
                        for (int i = 0; i < 3; i++)
                        {
                            System.Threading.Thread.Sleep(500);
                            Console.Write(".");
                        }
                        break;
                    default:

                        break;
                }
            } while (!validInput);

        }

        public void Train()
        {
            bool validInput = false;
            TrainingDoc t = new TrainingDoc();
            do
            {
                Console.Clear();
                Console.WriteLine("Choose a document to train the network with\n");
                
                var docs = Directory.GetFiles("TrainingDocs");

                var trainingDocPairs = new Dictionary<int, string>();
                var trainingDocPairDisplay = new Dictionary<int, string>();

                for (int i = 0; i < docs.Count(); i++)
                {
                    trainingDocPairs[i + 1] = docs[i];
                    trainingDocPairDisplay[i + 1] = docs[i].Replace("TrainingDocs\\", "");
                }

                foreach (var pair in trainingDocPairDisplay)
                {
                    Console.WriteLine($"{pair.Key}. {pair.Value}");
                }

                var choiceInput = Console.ReadLine();

                try
                {
                    var choice = Convert.ToInt32(choiceInput);

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
                    Console.WriteLine(e.Message + " Try a number instead!" );
                    Console.ReadLine();
                }
            } while (!validInput);

            network.AddTrainingDoc(t);

            Console.WriteLine("Trained Network with \"{0}\"", t.FileName);
            Console.ReadLine();

            Start();

        }

        public void Classify()
        {
            if (network.TrainingDocs.Count == 0)
            {
                Console.WriteLine("Cannot classify document without training data, must train network first");

                Start();
            }

            var docs = Directory.GetFiles("UnknownDocs");
            var unknownDocPairs = new Dictionary<int, string>();

            for (int i = 0; i < docs.Count(); i++)
            {
                unknownDocPairs[i + 1] = docs[i].Replace("UnknownDocs\\", "");
            }

            do
            {
                Console.Clear();
                foreach (var pair in unknownDocPairs)
                {
                    Console.WriteLine($"{pair.Key}. {pair.Value}");
                }

                Console.Write("\nChoose a document by entering its number: ");
                var choice = Console.ReadLine();

                int menuChoice;

                try
                {
                    menuChoice = Convert.ToInt32(choice);

                    if(unknownDocPairs.TryGetValue(menuChoice, out string value))
                    {
                        var result = network.ClassifyUnknownDocument(new Doc($"UnknownDocs\\{value}"));
                        Console.WriteLine($"Classified as {result.Item1} with {Math.Round(result.Item2, 2)}% probability");
                    }
                }
                catch(FormatException e)
                {
                    Console.WriteLine(e.Message + " Try a number instead!");
                    Console.ReadLine();
                }

                Console.ReadLine();

            } while (true);
        }

        public TrainingDoc AssignDocumentParty(string fileName)
        {
            Console.Clear();
            bool validInput = false;
            TrainingDoc t = new TrainingDoc();
            do
            {
                Console.WriteLine("Choose the party this document belongs to\n");
                var categoryPairs = new Dictionary<int, Category>() { { 1, Category.CONSERVATIVE }, { 2, Category.COALITION }, { 3, Category.LABOUR } };
                foreach (var pair in categoryPairs)
                {
                    Console.WriteLine($"{pair.Key}. {pair.Value}");
                }
                Console.Write("\n> ");
                var choice = Console.ReadLine();
                switch (choice[0])
                {
                    case '1':
                        t = new TrainingDoc(fileName, Category.CONSERVATIVE);
                        validInput = true;
                        break;
                    case '2':
                        t = new TrainingDoc(fileName, Category.COALITION);
                        validInput = true;

                        break;
                    case '3':
                        t = new TrainingDoc(fileName, Category.LABOUR);
                        validInput = true;
                        break;
                    default:
                        Console.WriteLine("Invalid Input!");
                        break;
                }
            } while (!validInput);
            return t;
        }


    }
}
