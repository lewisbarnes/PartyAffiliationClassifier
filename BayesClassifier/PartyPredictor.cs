using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayesClassifier
{
    public class PartyClassifier
    {
        public Dictionary<Category, Dictionary<string, int>> WordFrequencies;
        public Dictionary<Category, List<Word>> Words;
        public Dictionary<Category, double> PriorProbs;

        public PartyClassifier()
        {
            WordFrequencies = new Dictionary<Category, Dictionary<string, int>>();
            Words = new Dictionary<Category, List<Word>>();
            PriorProbs = new Dictionary<Category, double>();
        }

        public void GetBaseProbabilities(TrainingDoc[] trainingDocs)
        {
            foreach (Category c in Enum.GetValues(typeof(Category)))
            {
                PriorProbs[c] = trainingDocs.Select(d => d.Category == c).Count() / trainingDocs.Count();
                WordFrequencies[c] = DictionaryHelper.MergeDictionaries(trainingDocs.Where(s => s.Category == c).Select(w => w.WordFrequencies).ToArray());
            }
            foreach (Category c in Enum.GetValues(typeof(Category)))
            {
                if (!Words.TryGetValue(c, out List<Word> value))
                {
                    Words[c] = new List<Word>();
                }
                foreach (var pair in WordFrequencies[c])
                {
                    Words[c].Add(new Word(pair.Key, pair.Value, (double)(pair.Value + 1) / ((double)WordFrequencies[c].Values.Sum() + (double)WordFrequencies[c].Keys.Count)));
                }
            }
        }

        public Category ClassifyUnknown(Doc doc)
        {
            var overallProbs = new Dictionary<Category, double>();
            foreach (Category c in Enum.GetValues(typeof(Category)))
            {
                foreach (var word in doc.Words)
                {
                    var wordMatch = Words[c].FirstOrDefault(w => w.Key == word);
                    if (wordMatch.Key != null)
                    {
                        if (overallProbs.TryGetValue(c, out double value))
                        {
                            overallProbs[c] = value + Math.Log(Math.E, wordMatch.ConditionalProbability);
                        }
                        else
                        {
                            overallProbs[c] = Math.Log(Math.E, wordMatch.ConditionalProbability);
                        }
                    }
                }
                overallProbs[c] = overallProbs[c] + Math.Log(PriorProbs[c]);
            }
            return overallProbs.OrderBy(p => p.Value).First().Key;
        }

    }
}
