using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace PartyAffiliationClassifier
{
    [Serializable]
    public class Doc
    {
        
        public string FileName { get; set; }
        public static List<string> stopWords { get; set; }
        public List<Word> Words { get; set; }
        public int wordCount { get; set; }
        private static Calculator calculator = new Calculator();
        public Doc()
        {

        }

        public Doc(string fileName)

        {
            if(stopWords == null)
            {
                AddStopWords();
            }
            FileName = fileName;

            Words = new List<Word>();

            var wordString = new string(File.ReadAllText(FileName).ToCharArray());
            var sc = new StringScanner();
            sc.RemovePunctuation(ref wordString);
            wordString = wordString.Replace("\0", string.Empty);
            foreach (string word in wordString.Split(' '))
            {
                if (word != string.Empty)
                {
                    if (!stopWords.Any(s => s == word)) Words.Add(new Word(word));
                }
            }
            wordCount = Words.Count();
            Words = calculator.GetWordFrequencies(Words);
            Words = calculator.GetWordProbabilities(Words, wordCount);
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
