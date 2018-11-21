using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyAffiliationClassifier
{
    public class ClassificationResults
    {
        private Dictionary<Category, double> probabilityPercentages;
        private double coalitionPercentage;
        private double conservativePercentage;
        private double labourPercentage;
        public double ConservativePercentage { get { return conservativePercentage; } }
        public double CoalitionPercentage { get { return coalitionPercentage; } }
        public double LabourPercentage { get { return labourPercentage; } }
        public Category MostLikely { get { return probabilityPercentages.OrderByDescending(x => x.Value).FirstOrDefault().Key; } }

        public ClassificationResults()
        {
            probabilityPercentages = new Dictionary<Category, double>();
        }
        public double GetPartyPercentage(Category cat)
        {
            switch (cat)
            {
                case Category.COALITION:
                    return CoalitionPercentage;
                case Category.CONSERVATIVE:
                    return ConservativePercentage;
                case Category.LABOUR:
                    return LabourPercentage;
                default:
                    return new double();
            }
        }
        public void SetConservativePercentage(double percent)
        {
            conservativePercentage = percent;
            probabilityPercentages[Category.CONSERVATIVE] = conservativePercentage;
        }

        public void SetCoalitionPercentage(double percent)
        {
            coalitionPercentage = percent;
            probabilityPercentages[Category.COALITION] = coalitionPercentage;
        }

        public void SetLabourPercentage(double percent)
        {
            labourPercentage = percent;
            probabilityPercentages[Category.LABOUR] = labourPercentage;
        }
    }
}
