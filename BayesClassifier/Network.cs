using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PartyAffiliationClassifier
{
    [Serializable]
    public class Network
    {
        public Dictionary<Category, Dictionary<string, int>> WordFrequencies;
        public Dictionary<Category, List<Word>> Words;
        public Dictionary<Category, double> PriorProbs;
        public List<TrainingDoc> TrainingDocs;

        public Network()
        {
            PriorProbs = new Dictionary<Category, double>();
            Words = new Dictionary<Category, List<Word>>();
            WordFrequencies = new Dictionary<Category, Dictionary<string, int>>();
            TrainingDocs = new List<TrainingDoc>();
        }

        /// <summary>
        /// Get the training network saved to file or create a new one if not available
        /// </summary>
        /// <returns>Training network</returns>
        public static Network GetNetwork()
        {
            Network network;
            if (!Directory.GetFiles(Directory.GetCurrentDirectory()).Contains(Directory.GetCurrentDirectory() + "\\trainingNetwork"))
            {
                network = new Network();
                using (Stream stream = File.Open(Directory.GetCurrentDirectory() + "\\trainingNetwork.nwk", FileMode.Create))
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, network);
                }
            }
            else
            {
                using (Stream stream = File.Open(Directory.GetCurrentDirectory() + "\\trainingNetwork.nwk", FileMode.Open))
                {
                    network = new Network();
                    var binaryFormatter = new BinaryFormatter();
                    network = (Network)binaryFormatter.Deserialize(stream);
                }
            }
            return network;
        }

        public void AddTrainingDoc(TrainingDoc trainingDoc)
        {
            TrainingDocs.Add(trainingDoc);
            var cat = trainingDoc.Category;
            if (!WordFrequencies.TryGetValue(cat, out Dictionary<string, int> value))
            {
                WordFrequencies[cat] = trainingDoc.WordFrequencies;
            }
            else
            {
                WordFrequencies[cat] = MergeDictionaries(trainingDoc.WordFrequencies, WordFrequencies[cat]);
            }
            foreach (Category c in Enum.GetValues(typeof(Category)))
            {
                PriorProbs[c] = (double)TrainingDocs.Where(d => d.Category == c).Count() / (double)TrainingDocs.Count();
            }
            if (!Words.TryGetValue(cat, out List<Word> value1))
            {
                Words[cat] = new List<Word>();
            }
            foreach (var pair in WordFrequencies[cat])
            {
                Words[cat].Add(new Word(pair.Key, pair.Value, (double)(pair.Value + 1) / ((double)WordFrequencies[cat].Values.Sum() + (double)WordFrequencies[cat].Keys.Count)));
            }
            Save();
        }

        /// <summary>
        /// Save the network to file in its current state
        /// </summary>
        private void Save()
        {
            using (Stream stream = File.Open(Directory.GetCurrentDirectory() + "\\trainingNetwork.nwk", FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, this);
            }
        }
        /// <summary>
        /// Classify an unknown document using network knowledge from training documents
        /// </summary>
        /// <param name="doc">The unknown document to classify</param>
        /// <returns>Category of document based on prior knowledge</returns>
        public Category ClassifyUnknown(Doc doc)
        {
            var overallProbs = new Dictionary<Category, double>();
            foreach (Category c in Enum.GetValues(typeof(Category)))
            {
                foreach (var word in doc.Words)
                {
                    if (Words.TryGetValue(c, out List<Word> words))
                    {
                        var wordMatch = words.FirstOrDefault(w => w.Key == word);
                        if (wordMatch.Key != null)
                        {
                            if (overallProbs.TryGetValue(c, out double prob))
                            {
                                overallProbs[c] = prob + Math.Log(wordMatch.ConditionalProbability);
                            }
                            else
                            {
                                overallProbs[c] = Math.Log(wordMatch.ConditionalProbability);
                            }
                        }
                        overallProbs[c] = overallProbs[c] + Math.Log(PriorProbs[c]);
                    }
                }
            }
            return overallProbs.OrderBy(p => p.Value).First().Key;
        }
        /// <summary>
        /// Merge two dictionaries together summing the values for each key
        /// </summary>
        /// <param name="dictionaries">The dictionaries that are to be merged</param>
        /// <returns>Merged dictionary of specified input dictionaries</returns>
        private Dictionary<string, int> MergeDictionaries(params Dictionary<string, int>[] dictionaries)
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
