using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PartyAffiliationClassifier
{
    // Conditional Probabilities: P(word/cata) = (fcata[word]) + 1) / (Ncata + Nwords)
    // Overall Probability: P(cata) = Tcata / Tdocs ... P(catn) = Tcatn / Tdocs
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var trainingSet = TrainingDoc.GetTrainingDocs();
            PartyClassifier p = new PartyClassifier();
            p.GetBaseProbabilities(trainingSet);
            Console.WriteLine(p.ClassifyUnknown(new Doc("unknownLabour2.txt")));
            var uim = new UserInteractionMenu();
            uim.Go();
            Console.ReadKey();
        }
    }
}
