using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PartyAffiliationClassifier
{
    [Serializable]
    public struct Word
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public int Frequency { get; set; }
        [XmlAttribute]
        public double ConditionalProbability { get; set; }
        public Word(string key, int frequency, double conditionalProbability)
        {
            Key = key;
            Frequency = frequency;
            ConditionalProbability = conditionalProbability;
        }
    }
}
