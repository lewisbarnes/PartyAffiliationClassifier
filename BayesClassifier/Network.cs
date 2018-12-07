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
        private Calculator _calculator = new Calculator();
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
            partyData.IncrementDocCount();
            Dictionary<Category, double> priorProbabilities = _calculator.GetPriorProbabilities(Data);
            foreach (KeyValuePair<Category, double> kvp in priorProbabilities)
            {
                if (kvp.Key != Category.NONE)
                {
                    PartyData party = Data.Where(x => x.GetCategory() == kvp.Key).FirstOrDefault();
                    party.SetProbability(kvp.Value);
                }
            }
            partyData.SetWords(MergeWords(partyData.Words, trainingDoc.Words));
            partyData.SetNGrams(MergeWords(partyData.NGrams, trainingDoc.NGrams));
            _calculator.GetRelativeFrequencies(partyData.Words);
            _calculator.GetRelativeFrequencies(partyData.NGrams);
        }

        /// <summary>
        /// Save the network to file in its current state
        /// </summary>
        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Network));

            using (Stream stream = File.Open(Directory.GetCurrentDirectory() + "\\trainingNetwork.xml", FileMode.Create))
            {
                
                serializer.Serialize(stream, this);
            }
        }
        /// <summary>
        /// Classify an unknown document using network knowledge from training documents
        /// </summary>
        /// <param name="doc">The unknown document to classify</param>
        /// <returns>Category of document based on prior knowledge</returns>
        public Dictionary<string,ClassificationResults> ClassifyUnknownDocument(Doc doc)
        {
            Dictionary<string, ClassificationResults> returnResults = new Dictionary<string, ClassificationResults>();
            Dictionary<Category, double> overallProbs = new Dictionary<Category, double>();
            Dictionary<Category, double> overallProbsNGrams = new Dictionary<Category, double>();
            Dictionary<Category, double> overallProbsTfIdf = new Dictionary<Category, double>();
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
                           
                        }
                    }
                }
                overallProbs[p.GetCategory()] += Math.Log(p.Probability);

                foreach (Word ngram in doc.NGrams)
                {
                    Word match = p.NGrams.Where(x => x.Key == ngram.Key).FirstOrDefault();
                    if (match != null)
                    {
                        if (overallProbsNGrams.TryGetValue(p.GetCategory(), out double prob))
                        {
                            overallProbsNGrams[p.GetCategory()] = prob + Math.Log(match.RelativeFrequency);
                        }
                        else
                        {
                            overallProbsNGrams[p.GetCategory()] = Math.Log(match.RelativeFrequency);
                        }
                    }
                }
                overallProbsNGrams[p.GetCategory()] += Math.Log(p.Probability);

                foreach (Word word in doc.Words)
                {
                    Word match = p.Words.Where(x => x.Key == word.Key).FirstOrDefault();
                    if (match != null)
                    {
                        if (overallProbsTfIdf.TryGetValue(p.GetCategory(), out double prob))
                        {
                            overallProbsTfIdf[p.GetCategory()] = prob + Math.Log(match.Frequency * (TotalDocs / match.DocumentFrequency));
                        }
                        else
                        {
                            overallProbsTfIdf[p.GetCategory()] = Math.Log(match.Frequency * (TotalDocs / match.DocumentFrequency));
                        }
                    }
                }
                overallProbsTfIdf[p.GetCategory()] += Math.Log(p.Probability);
            }
            ClassificationResults results = new ClassificationResults();
            ClassificationResults nGramResults = new ClassificationResults();
            ClassificationResults tfIdfResults = new ClassificationResults();
            results.SetConservativePercentage(((overallProbs.Where(x => x.Key == Category.CONSERVATIVE).FirstOrDefault().Value * -1) / overallProbs.Sum(x => x.Value) * -1) * 100);
            results.SetCoalitionPercentage(((overallProbs.Where(x => x.Key == Category.COALITION).FirstOrDefault().Value * -1) / overallProbs.Sum(x => x.Value) * -1) * 100);
            results.SetLabourPercentage(((overallProbs.Where(x => x.Key == Category.LABOUR).FirstOrDefault().Value * -1) / overallProbs.Sum(x => x.Value) * -1) * 100);
            nGramResults.SetConservativePercentage(((overallProbsNGrams.Where(x => x.Key == Category.CONSERVATIVE).FirstOrDefault().Value * -1) / overallProbsNGrams.Sum(x => x.Value) * -1) * 100);
            nGramResults.SetCoalitionPercentage(((overallProbsNGrams.Where(x => x.Key == Category.COALITION).FirstOrDefault().Value * -1) / overallProbsNGrams.Sum(x => x.Value) * -1) * 100);
            nGramResults.SetLabourPercentage(((overallProbsNGrams.Where(x => x.Key == Category.LABOUR).FirstOrDefault().Value * -1) / overallProbsNGrams.Sum(x => x.Value) * -1) * 100);
            tfIdfResults.SetConservativePercentage(((overallProbsTfIdf.Where(x => x.Key == Category.CONSERVATIVE).FirstOrDefault().Value * -1) / overallProbsTfIdf.Sum(x => x.Value) * -1) * 100);
            tfIdfResults.SetCoalitionPercentage(((overallProbsTfIdf.Where(x => x.Key == Category.COALITION).FirstOrDefault().Value * -1) / overallProbsTfIdf.Sum(x => x.Value) * -1) * 100);
            tfIdfResults.SetLabourPercentage(((overallProbsTfIdf.Where(x => x.Key == Category.LABOUR).FirstOrDefault().Value * -1) / overallProbsTfIdf.Sum(x => x.Value) * -1) * 100);
            returnResults["normal"] = results;
            returnResults["ngram"] = nGramResults;
            returnResults["tfidf"] = tfIdfResults;
            return returnResults;
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
                first.DocumentFrequency = group.Sum(x => x.DocumentFrequency);
                returnWords.Add(first);
            }

            return returnWords;
        }

    }
}

