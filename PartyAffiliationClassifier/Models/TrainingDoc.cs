using System;

namespace PartyAffiliationClassifier
{
    /// <summary>
    /// Training Document class used for the training set.
    /// </summary>
    [Serializable]
    public class TrainingDoc : Doc
    {
        
        private Category _category;

        public Category Category { get { return _category; } private set { } }

        public TrainingDoc()
        {

        }

        public TrainingDoc(string fileName, Category category) : base(fileName)
        {
            _category = category;
        }
    }

    public enum Category
    {
        CONSERVATIVE,
        COALITION,
        LABOUR,
        NONE
    }

}
