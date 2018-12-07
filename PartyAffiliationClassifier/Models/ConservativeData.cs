using System;
using System.Collections.Generic;

namespace PartyAffiliationClassifier
{
    [Serializable]
    public class ConservativeData : PartyData
    {
        public override Category GetCategory()
        {
            return Category.CONSERVATIVE;
        }
        public ConservativeData()
        {

        }
        public ConservativeData(List<Word> words) : base(words)
        {

        }
    }
}
