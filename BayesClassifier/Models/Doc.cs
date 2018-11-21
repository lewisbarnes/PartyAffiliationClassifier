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
        private static WordStemmer _ws = new WordStemmer();
        private string _fileName;
        private static Calculator _calculator = new Calculator();
        private static List<string> _stopWords { get; set; }

        public string FileName { get { return _fileName; } private set { } }
        public List<Word> Words { get; set; }
        
        public Doc()
        {

        }

        public Doc(string fileName)
        {
            _fileName = fileName;

            if (_stopWords == null)
            {
                AddStopWords();
            }

            Words = new List<Word>();

            string wordString = new string(File.ReadAllText(FileName).ToCharArray());

            StringScanner sc = new StringScanner();
            sc.RemovePunctuation(ref wordString);

            wordString = wordString.Replace("\0", string.Empty);

            foreach (string word in wordString.Split(' '))
            {
                if (word != string.Empty)
                {
                    // All words not in stopwords, stem word and add to list
                    if (!_stopWords.Any(s => s == word)) Words.Add(new Word(_ws.Stem(word).ToLower()));
                }
            }

            Words = _calculator.GetWordFrequencies(Words);
            Words = _calculator.GetRelativeFrequencies(Words);
        }

        /// <summary>
        /// Add all of the stopwords to the list from file.
        /// </summary>
        private static void AddStopWords()
        {
            // If stopwords does not exist, add all stopwords to list from file
            if (_stopWords == null)
            {
                _stopWords = new List<string>();
                string words = File.ReadAllText("stopwords.txt").Replace("\r\n", " ");
                foreach (string w in words.Split(' '))
                {
                    _stopWords.Add(w);
                }
            }
        }
    }
}
