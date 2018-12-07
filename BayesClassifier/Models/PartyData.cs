using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PartyAffiliationClassifier
{
    [XmlInclude(typeof(ConservativeData))]
    [XmlInclude(typeof(CoalitionData))]
    [XmlInclude(typeof(LabourData))]
    [Serializable]
    public abstract class PartyData
    {
        
        private List<Word> _words;
        private List<Word> _nGrams;
        private double _probability;
        private int _docCount;

        public List<Word> Words { get { return _words; } set { _words = value; } }
        public List<Word> NGrams { get { return _nGrams; } set { _nGrams = value; } }
        public double Probability { get { return _probability; } set { _probability = value; } }
        public int DocCount { get { return _docCount; } set { _docCount = value; } }

        public virtual Category GetCategory()
        {
            return Category.NONE;
        }

        protected PartyData()
        {
        }

        protected PartyData(List<Word> words)
        {
            _words = words;
        }

        public void SetWords(List<Word> words)
        {
            _words = words;
        }

        public void SetNGrams(List<Word> nGrams)
        {
            _nGrams = nGrams;
        }
        public void SetProbability(double probability)
        {
            _probability = probability;
        }

        public void IncrementDocCount()
        {
            _docCount++;
        }
    }
}
