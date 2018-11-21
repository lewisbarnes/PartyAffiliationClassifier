using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

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
