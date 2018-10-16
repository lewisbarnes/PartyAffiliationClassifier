using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static TrainingDoc[] GetTrainingDocs()
        {
            var tdocs = new List<TrainingDoc>();
            foreach(var file in Directory.GetFiles("TrainingDocs"))
            {
                
                var splitName = Regex.Split(file.Replace("TrainingDocs\\",""), @"([a-zA-Z]+[0-9]{1})");
                tdocs.Add(new TrainingDoc(file, (Category)Enum.Parse(typeof(Category), splitName[1].Remove(splitName[1].Length - 1, 1).ToUpper())));
            }
            return tdocs.ToArray();
        }
    }

    public enum Category
    {
        CONSERVATIVE,
        COALITION,
        LABOUR
    }

}
