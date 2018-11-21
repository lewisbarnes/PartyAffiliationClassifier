using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace PartyAffiliationClassifier
{
    [Serializable]
    public class Network
    {
        public List<PartyData> Data { get; set; }

        public int TotalDocs { get { return Data.Sum(x => x.DocCount); } }
        [NonSerialized]
        private Calculator calculator = new Calculator();
        public Network(int isnew)
        {
            Data = new List<PartyData>() { new ConservativeData(), new CoalitionData(), new LabourData() };
        }
        private bool NetworkExists()
        {
            return Directory.GetFiles(Directory.GetCurrentDirectory()).Contains(Directory.GetCurrentDirectory()+@"\trainingNetwork.xml");
        }
        public Network()
        {

        }

        /// <summary>
        /// Get the training network saved to file or create a new one if not available
        /// </summary>
        /// <returns>Training network</returns>
        public Network GetNetwork()
        {
            Network network = new Network();
            if (!NetworkExists())
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
            Category cat = trainingDoc.Category;
            PartyData partyData = Data.Where(x => x.GetCategory() == cat).First();
            partyData.DocCount++;
            Dictionary<Category, double> priorProbabilities = calculator.GetPriorProbabilities(Data);
            foreach (KeyValuePair<Category, double> kvp in priorProbabilities)
            {
                if (kvp.Key != Category.NONE)
                {
                    PartyData party = Data.Where(x => x.GetCategory() == kvp.Key).FirstOrDefault();
                    party.SetProbability(kvp.Value);
                }
            }
            partyData.SetWords(MergeWords(partyData.Words, trainingDoc.Words));
            calculator.GetRelativeFrequencies(partyData.Words);
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
        public ClassificationResults ClassifyUnknownDocument(Doc doc)
        {
            Dictionary<Category, double> overallProbs = new Dictionary<Category, double>();
            foreach (PartyData p in Data)
            {
                foreach (Word word in doc.Words)
                {
                    Word match = p.Words.Where(x => x.Key == word.Key).FirstOrDefault();
                    if (match != null)
                    {
                        if (overallProbs.TryGetValue(p.GetCategory(), out double prob))
                        {
                            overallProbs[p.GetCategory()] = prob + Math.Log(match.RelativeFrequency);
                        }
                        else
                        {
                            overallProbs[p.GetCategory()] = Math.Log(match.RelativeFrequency);
                            overallProbs[p.GetCategory()] += Math.Log(p.Probability);
                        }
                    }
                }
            }
            ClassificationResults results = new ClassificationResults();
            results.SetConservativePercentage(((overallProbs.Where(x => x.Key == Category.CONSERVATIVE).FirstOrDefault().Value * -1) / overallProbs.Sum(x => x.Value) * -1) * 100);
            results.SetCoalitionPercentage(((overallProbs.Where(x => x.Key == Category.COALITION).FirstOrDefault().Value * -1) / overallProbs.Sum(x => x.Value) * -1) * 100);
            results.SetLabourPercentage(((overallProbs.Where(x => x.Key == Category.LABOUR).FirstOrDefault().Value * -1) / overallProbs.Sum(x => x.Value) * -1) * 100);
            return results;
        }

        /// <summary>
        /// Merge two lists of words together summing the frequency on same keys
        /// </summary>
        /// <param name="words1"></param>
        /// <param name="words2"></param>
        /// <returns></returns>
        private List<Word> MergeWords(List<Word> words1, List<Word> words2)
        {
            if (words1 == null) return words2;
            else if (words2 == null) return words1;

            List<Word> concatWords = words1.Concat(words2).ToList();
            List<Word> returnWords = new List<Word>();
            List<IGrouping<string, Word>> groups = concatWords.GroupBy(x => x.Key).ToList();

            foreach (IGrouping<string, Word> group in groups)
            {
                Word first = group.FirstOrDefault();
                first.SetFrequency(group.Sum(x => x.Frequency));
                returnWords.Add(first);
            }

            return returnWords;
        }

    }
}

