using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyAffiliationClassifier
{
    [Serializable]
    public class CoalitionData : PartyData
    {
        public override Category GetCategory()
        {
            return Category.COALITION;
        }

        public CoalitionData()
        {

        }
        public CoalitionData(List<Word> words) : base(words)
        {
            
        }
    }
}
