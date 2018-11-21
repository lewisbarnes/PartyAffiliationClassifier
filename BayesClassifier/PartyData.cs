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
        public List<Word> Words { get; set; }
        public double Probability { get; set; }

        public int DocCount { get; set; }
        public virtual Category GetCategory()
        {
            return Category.NONE;
        }
        protected PartyData()
        {
        }

        protected PartyData(List<Word> words)
        {
            Words = words;
        }

        public void SetWords(List<Word> words)
        {
            Words = words;
        }

        public void SetProbability(double prob)
        {
            Probability = prob;
        }
    }
}
