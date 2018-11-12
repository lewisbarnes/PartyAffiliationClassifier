using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
