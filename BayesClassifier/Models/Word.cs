using System;

namespace PartyAffiliationClassifier
{
    [Serializable]
    public class Word
    {

        private string m_key;
        private int _frequency;
        private double _relativeFrequency;
        private int _documentFrequency;

        public string Key { get { return m_key; } set { m_key = value; } }
        public double RelativeFrequency { get { return _relativeFrequency; } set { _relativeFrequency = value; } }
        public int Frequency { get { return _frequency; } set { _frequency = value; } }
        public int DocumentFrequency { get { return _documentFrequency; } set { _documentFrequency = value; } }


        public Word(string key, int frequency, double relativeFrequency)
        {
            Key = key;
            Frequency = frequency;
            RelativeFrequency = relativeFrequency;
        }
        public Word(string key)
        {
            Key = key;
        }
        public Word()
        {

        }

        public void IncreaseDocumentFrequency()
        {
            DocumentFrequency++;
        }
        public void SetFrequency(int freq)
        {
            Frequency = freq;
        }

        public void SetProbability(double prob)
        {
            RelativeFrequency = prob;
        }
    }
}
