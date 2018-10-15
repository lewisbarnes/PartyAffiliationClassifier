using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace BayesClassifier
{
    public class TrainingDoc : Doc
    {

        public Category Category;

        public TrainingDoc(string fileName, Category category) : base(fileName)
        {
            Category = category;
        }
    }

    public enum Category
    {
        CONSERVATIVE,
        COALITION,
        LABOUR
    }

}
