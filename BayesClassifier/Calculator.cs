using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PartyAffiliationClassifier
{
    public class Calculator
    {
        public Calculator()
        {

        }
        public List<Word> GetWordFrequencies(List<Word> words)
        {
            Dictionary<string, int> wordFrequencies = new Dictionary<string, int>();
            List<Word> wordList = words.GroupBy(x => x.Key).Select(w => w.First()).ToList();
            foreach (Word word in words)
            {
                if (wordFrequencies.TryGetValue(word.Key, out int freq))
                {
                    wordFrequencies[word.Key] = freq + 1;
                }
                else
                {
                    wordFrequencies[word.Key] = 1;
                }
            }
            foreach (var kvp in wordFrequencies)
            {
                Word word = wordList.Where(x => x.Key == kvp.Key).First();
                word.SetFrequency(kvp.Value);
            }
            return wordList;
        }

        public List<Word> GetWordProbabilities(List<Word> words, int wordCount)

        {
            List<Word> returnWords = new List<Word>();
            foreach (Word word in words)
            {
                var newWord = new Word(word.Key, word.Frequency, (double)word.Frequency / (double)wordCount);
                returnWords.Add(newWord);
            }
            return returnWords;
        }

        public Dictionary<Category, double> GetPriorProbabilities(List<TrainingDoc> tdocs)
        {
            Dictionary<Category,double> probabilities = new Dictionary<Category, double>();
            foreach(Category cat in Enum.GetValues(typeof(Category)))
            {
                if(cat != Category.NONE)
                {
                    probabilities[cat] = (double)(tdocs.Where(x => x.Category == cat).Count() / (double)tdocs.Count());
                }
            }
            return probabilities;
        }
    }
}
