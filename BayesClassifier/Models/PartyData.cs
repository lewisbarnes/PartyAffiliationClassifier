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
        private double _probability;
        private int _docCount;

        public List<Word> Words { get { return _words; } private set { } }
        public double Probability { get { return _probability; } private set { } }
        public int DocCount { get { return _docCount; } private set { } }

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
