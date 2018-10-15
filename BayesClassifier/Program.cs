using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BayesClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            // Read speech from file.
            TrainingDoc[] trainingSet = {
                new TrainingDoc("Coalition9thMay2012.txt", Category.COALITION),
                new TrainingDoc("Conservative16thNov1994.txt", Category.CONSERVATIVE),
                new TrainingDoc("Conservative27thMay2015.txt", Category.CONSERVATIVE),
                new TrainingDoc("Labour6thNov2007.txt", Category.LABOUR),
                new TrainingDoc("Labour26thNov2003.txt", Category.LABOUR)};

            var ProbCoalition = (double)trainingSet.Where(d => d.Category == Category.COALITION).Count() / (double)trainingSet.Count();
            var ProbConservative = (double)trainingSet.Where(d => d.Category == Category.CONSERVATIVE).Count() / (double)trainingSet.Count();
            var ProbLabour = (double)trainingSet.Where(d => d.Category == Category.LABOUR).Count() / (double)trainingSet.Count();
            var result = trainingSet[1].WordFrequencies.Union(trainingSet[2].WordFrequencies);
            foreach (var t in trainingSet)
            {
                Console.WriteLine($"{t.FileName} - {t.Words.Count()} words - {t.WordFrequencies.Count()} unique - {t.Category}");
                foreach (var kvp in t.WordFrequencies)
                {
                    Console.WriteLine($"{{{kvp.Key} : {kvp.Value} : {t.WordProbabilities[kvp.Key] * 100:f2}%}}");
                }
                Console.WriteLine();
            }
            var conservativeDicts = trainingSet.Where(s => s.Category == Category.CONSERVATIVE).Select(c => c.WordFrequencies).ToArray();
            var newDict = DictionaryHelper.MergeDictionaries(conservativeDicts);
            foreach (var kvp in newDict)
            {
                Console.WriteLine($"{{{kvp.Key} : {kvp.Value}");
            }
            Console.WriteLine(ProbCoalition);
            Console.WriteLine(ProbConservative);
            Console.WriteLine(ProbLabour);
            Console.ReadKey();
        }
    }
}
