using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PartyAffiliationClassifier
{
    [Serializable]
    public class Network
    {
        public List<PartyData> Data { get; set; }
        public List<TrainingDoc> TrainingDocs { get; set; }
        [NonSerialized]
        private Calculator calculator = new Calculator();
        public Network(int isnew)
        {
            Data = new List<PartyData>() { new ConservativeData(), new CoalitionData(), new LabourData() };
            TrainingDocs = new List<TrainingDoc>();
        }

        public Network()
        {

        }

        /// <summary>
        /// Get the training network saved to file or create a new one if not available
        /// </summary>
        /// <returns>Training network</returns>
        public static Network GetNetwork()
        {
            Network network = new Network();
            if (!Directory.GetFiles(Directory.GetCurrentDirectory()).Contains(Directory.GetCurrentDirectory() + @"\trainingNetwork.xml"))
            {
                network = new Network(1);
                using (Stream stream = File.Open(Directory.GetCurrentDirectory() + @"\trainingNetwork.xml", FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Network));
                    serializer.Serialize(stream, network);
                }
            }
            else
            {
                using (Stream stream = File.Open(Directory.GetCurrentDirectory() + @"\trainingNetwork.xml", FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Network));
                    network = (Network)serializer.Deserialize(stream);
                }
            }
            return network;
        }

        /// <summary>
        /// Add a training document to the network
        /// </summary>
        /// <param name="trainingDoc">The training doc to add to the network</param>
        public void AddTrainingDoc(TrainingDoc trainingDoc)
        {
            TrainingDocs.Add(trainingDoc);
            Category cat = trainingDoc.Category;
            PartyData partyData = Data.Where(x => x.GetCategory() == cat).First();
            Dictionary<Category, double> priorProbabilities = calculator.GetPriorProbabilities(TrainingDocs);
            foreach (KeyValuePair<Category, double> kvp in priorProbabilities)
            {
                if (kvp.Key != Category.NONE)
                {
                    var party = Data.Where(x => x.GetCategory() == kvp.Key).First();
                    party.SetProbability(kvp.Value);
                }
            }
            partyData.SetWords(MergeWords(partyData.Words, trainingDoc.Words));
            calculator.GetWordProbabilities(partyData.Words, partyData.Words.Count);
            Save();
        }

        /// <summary>
        /// Save the network to file in its current state
        /// </summary>
        private void Save()
        {
            using (Stream stream = File.Open(Directory.GetCurrentDirectory() + "\\trainingNetwork.xml", FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Network));
                serializer.Serialize(stream, this);
            }
        }
        /// <summary>
        /// Classify an unknown document using network knowledge from training documents
        /// </summary>
        /// <param name="doc">The unknown document to classify</param>
        /// <returns>Category of document based on prior knowledge</returns>
        public Tuple<Category, double> ClassifyUnknownDocument(Doc doc)
        {
            var overallProbs = new Dictionary<Category, double>();
            foreach (PartyData p in Data)
            {
                foreach (var word in doc.Words)
                {
                    var match = p.Words.Where(x => x.Key == word.Key).FirstOrDefault();
                    if (match != null)
                    {
                        if (overallProbs.TryGetValue(p.GetCategory(), out double prob))
                        {
                            overallProbs[p.GetCategory()] = prob + Math.Log(match.ConditionalProbability);
                        }
                        else
                        {
                            overallProbs[p.GetCategory()] = Math.Log(match.ConditionalProbability);
                        }
                    }
                }
                overallProbs[p.GetCategory()] += Math.Log(p.Probability);
            }
            var percent = ((overallProbs.OrderBy(p => p.Value).First().Value * -1) / overallProbs.Sum(x => x.Value) * -1) * 100;
            return new Tuple<Category, double>(overallProbs.OrderBy(p => p.Value).First().Key, percent);
        }

        private List<Word> MergeWords(List<Word> words1, List<Word> words2)
        {
            if(words1 == null)
            {
                return words2;
            }
            else if(words2 == null)
            {
                return words1;
            }
            List<Word> concatWords = words1.Concat(words2).ToList();
            List<Word> returnWords = new List<Word>();
            var groups = concatWords.GroupBy(x => x.Key).ToList();
            foreach (var group in groups)
            {
                var first = group.FirstOrDefault();
                first.Frequency = group.Sum(x => x.Frequency);
                returnWords.Add(first);
            }
            return returnWords;

        }

    }
}

