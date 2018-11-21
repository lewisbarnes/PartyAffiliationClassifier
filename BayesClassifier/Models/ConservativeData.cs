using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
