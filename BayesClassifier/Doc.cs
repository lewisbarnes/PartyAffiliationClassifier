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
        private static WordStemmer ws = new WordStemmer();
        private string fileName;
        public string FileName { get { return fileName; } private set { } }
        private static List<string> stopWords { get; set; }
        public List<Word> Words { get; set; }
        private static Calculator calculator = new Calculator();
        public Doc()
        {

        }

        public Doc(string fileName)
        {
            if (stopWords == null)
            {
                AddStopWords();
            }
            this.fileName = fileName;

            Words = new List<Word>();

            string wordString = new string(File.ReadAllText(FileName).ToCharArray());

            StringScanner sc = new StringScanner();
            sc.RemovePunctuation(ref wordString);

            wordString = wordString.Replace("\0", string.Empty);
            foreach (string word in wordString.Split(' '))
            {
                if (word != string.Empty)
                {
                    if (!stopWords.Any(s => s == word)) Words.Add(new Word(ws.Stem(word).ToLower()));
                }
            }
            Words = calculator.GetWordFrequencies(Words);
            Words = calculator.GetRelativeFrequencies(Words);
        }

        private static void AddStopWords()
        {
            if (stopWords == null)
            {
                stopWords = new List<string>();
                string words = File.ReadAllText("stopwords.txt").Replace("\r\n", " ");
                foreach (string w in words.Split(' '))
                {
                    stopWords.Add(w);
                }
            }
        }
    }
}
