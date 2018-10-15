using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BayesClassifier
{
    // Conditional Probabilities: P(word/cata) = (fcata[word]) + 1) / (Ncata + Nwords)
    // Overall Probability: P(cata) = Tcata / Tdocs ... P(catn) = Tcatn / Tdocs
    // 
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            TrainingDoc[] trainingSet = {
                new TrainingDoc("Coalition9thMay2012.txt", Category.COALITION),
                new TrainingDoc("Conservative16thNov1994.txt", Category.CONSERVATIVE),
                new TrainingDoc("Conservative27thMay2015.txt", Category.CONSERVATIVE),
                new TrainingDoc("Labour6thNov2007.txt", Category.LABOUR),
                new TrainingDoc("Labour26thNov2003.txt", Category.LABOUR)};
            Doc unknownDoc = new Doc("unknownLabour.txt");
            Dictionary<Category, Dictionary<string, int>> partyWordFrequencies = new Dictionary<Category,Dictionary<string, int>>();
            Dictionary<Category, List<Word>> partyWords = new Dictionary<Category,List<Word>>();

            Dictionary<Category, double> overallProbs = new Dictionary<Category, double>();

            foreach (Category c in Enum.GetValues(typeof(Category)))
            {
                partyWordFrequencies[c] = DictionaryHelper.MergeDictionaries(trainingSet.Where(s => s.Category == c).Select(w => w.WordFrequencies).ToArray());
            }
            foreach(Category c in Enum.GetValues(typeof(Category)))
            {
                if(!partyWords.TryGetValue(c, out List<Word> value))
                {
                    partyWords[c] = new List<Word>();
                }
                foreach(var pair in partyWordFrequencies[c])
                {
                    partyWords[c].Add(new Word(pair.Key, pair.Value, (double)(pair.Value + 1) / ((double)partyWordFrequencies[c].Values.Sum() + (double)partyWordFrequencies[c].Keys.Count)));
                }
            }
            Dictionary<Category, double> priorProbs = new Dictionary<Category, double>();
            priorProbs[Category.CONSERVATIVE] = 0.4;
            priorProbs[Category.LABOUR] = 0.4;
            priorProbs[Category.COALITION] = 0.2;
            foreach(Category c in Enum.GetValues(typeof(Category)))
            {
                foreach(var word in unknownDoc.Words)
                {
                    if(partyWords[c].Where(w => w.Key == word).DefaultIfEmpty().First() != null)
                    {
                        if(overallProbs.TryGetValue(c, out double value))
                        {
                            overallProbs[c] = value * (double)partyWords[c].Where(w => w.Key == word).FirstOrDefault().ConditionalProbability;
                        }
                        else
                        {
                            overallProbs[c] = (double)partyWords[c].Where(w => w.Key == word).FirstOrDefault().ConditionalProbability;
                        }
                        Console.WriteLine(overallProbs[c]);
                    }
                    
                }
                overallProbs[c] = ((double)overallProbs[c] * (double)priorProbs[c]);
            }
            var party = overallProbs.OrderBy(p => p.Value).First().Key;
            Console.WriteLine(party);
            Console.ReadKey();
        }
    }
}
