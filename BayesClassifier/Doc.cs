using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PartyAffiliationClassifier
{
    public class Doc
    {
        public string FileName;
        private static List<string> stopWords;

        public List<string> Words;
        public Dictionary<string, int> WordFrequencies;
        public Dictionary<string, float> WordProbabilities;

        public Doc(string fileName)

        {
            AddStopWords();
            FileName = fileName;

            Words = new List<string>();
            WordFrequencies = new Dictionary<string, int>();
            WordProbabilities = new Dictionary<string, float>();

            var wordString = new string(File.ReadAllText(FileName).ToCharArray());
            var sc = new StringScanner();
            wordString = sc.RemovePunctuation(ref wordString);
            wordString = wordString.Replace("\0", string.Empty);
            foreach (string word in wordString.Split(' '))
            {
                if (word != string.Empty)
                {
                    if (!stopWords.Any(s => s == word)) Words.Add(word);
                }
            }
            GetWordFrequencies(Words);
            foreach (var kvp in WordFrequencies)
            {
                WordProbabilities[kvp.Key] = (float)kvp.Value / (float)Words.Count();
            }
            WordFrequencies = WordFrequencies.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private void GetWordFrequencies(List<string> words)
        {
            foreach (string word in Words)
            {
                if (WordFrequencies.TryGetValue(word, out int freq))
                {
                    WordFrequencies[word] = freq + 1;
                }
                else
                {
                    WordFrequencies[word] = 1;
                }
            }
        }

        private static void AddStopWords()
        {
            if (stopWords == null)
            {
                stopWords = new List<string>();
                var words = File.ReadAllText("stopwords.txt").Replace("\r\n", " ");
                foreach (string w in words.Split(' '))
                {
                    stopWords.Add(w);
                }
            }
        }
    }
}
