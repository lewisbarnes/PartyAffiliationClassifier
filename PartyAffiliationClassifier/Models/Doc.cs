using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PartyAffiliationClassifier
{
    [Serializable]
    public class Doc : IDisposable
    {
        private static WordStemmer _ws = new WordStemmer();
        private static StringScanner _sc = new StringScanner();
        private string _fileName;
        private static Calculator _calculator = new Calculator();
        private static List<string> _stopWords;
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public List<Word> Words { get; set; }

        public List<Word> NGrams { get; set; }
        
        public Doc()
        {

        }

        public Doc(string fileName)
        {
            NGramMaker nm = new NGramMaker();
            _fileName = fileName;

            if (_stopWords == null)
            {
                AddStopWords();
            }
            NGrams = new List<Word>();
            Words = new List<Word>();

            string wordString = new string(File.ReadAllText(FileName).ToCharArray());

            
            _sc.RemovePunctuation(ref wordString);
            // Generate word-level trigrams from text
            IEnumerable<string> nGrams = nm.MakeNGrams(wordString, 3);
            foreach(string n in nGrams)
            {
                Word nw = new Word(n);
                NGrams.Add(nw);
            }
            wordString = wordString.Replace("\0", string.Empty);

            foreach (string word in wordString.Split(' '))
            {
                if (word != string.Empty)
                {
                    Word w = new Word(_ws.Stem(word).ToLower());
                    // All words not in stopwords, stem word and add to list
                    if (!_stopWords.Any(s => s == word)) Words.Add(w);
                }
            }

            Words = _calculator.GetWordFrequencies(Words);
            Words = _calculator.GetRelativeFrequencies(Words);
            NGrams = _calculator.GetWordFrequencies(NGrams);
            NGrams = _calculator.GetRelativeFrequencies(NGrams);
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Doc() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.Collect();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
