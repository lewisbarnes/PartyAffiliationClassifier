using System;
using System.Collections.Generic;

namespace PartyAffiliationClassifier
{
   [Serializable]
   public class LabourData : PartyData
    {
        public override Category GetCategory()
        {
            return Category.LABOUR;
        }

        public LabourData()
        {

        }
        public LabourData(List<Word> words) : base(words)
        {
            
        }
    }
}
