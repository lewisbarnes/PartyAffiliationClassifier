using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PartyAffiliationClassifier
{
    [Serializable]
    public class Word
    {
        public string Key { get; set; }
        public int Frequency { get; set; }
        public double ConditionalProbability { get; set; }

        public Word(string key, int frequency, double conditionalProbability)
        {
            Key = key;
            Frequency = frequency;
            ConditionalProbability = conditionalProbability;
        }
        public Word(string key)
        {
            Key = key;
        }
        public Word()
        {

        }

        public void SetFrequency(int freq)
        {
            Frequency = freq;
        }

        public void SetProbability(double prob)
        {
            ConditionalProbability = prob;
        }
    }
}
