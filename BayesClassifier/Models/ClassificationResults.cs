using System.Collections.Generic;
using System;
using System.Linq;

namespace PartyAffiliationClassifier
{
    public class ClassificationResults
    {
        private Dictionary<Category, double> _probabilityPercentages;
        private double _coalitionPercentage = 0;
        private double _conservativePercentage = 0;
        private double _labourPercentage = 0;
        public double ConservativePercentage { get { return _conservativePercentage; } private set { } }
        public double CoalitionPercentage { get { return _coalitionPercentage; } private set { } }
        public double LabourPercentage { get { return _labourPercentage; } private set { } }
        public Category MostLikely { get { return _probabilityPercentages.OrderByDescending(x => x.Value).FirstOrDefault().Key; } }
        public Dictionary<Category,double> GetOthers { get { return _probabilityPercentages.Where(x => x.Key != MostLikely).OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value); } }

        public ClassificationResults()
        {
            _probabilityPercentages = new Dictionary<Category, double>();
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
            _conservativePercentage = percent;
            _probabilityPercentages[Category.CONSERVATIVE] = _conservativePercentage;
        }

        public void SetCoalitionPercentage(double percent)
        {
            _coalitionPercentage = percent;
            _probabilityPercentages[Category.COALITION] = _coalitionPercentage;
        }

        public void SetLabourPercentage(double percent)
        {
            _labourPercentage = percent;
            _probabilityPercentages[Category.LABOUR] = _labourPercentage;
        }
    }
}
