using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartyAffiliationClassifier;
using System.Collections.Generic;

namespace PartyAffiliationClassifierTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestWordStemming()
        {
            Dictionary<string, string> testData = new Dictionary<string, string>() {
                { "caresses", "caress"},
                { "assesses","assess"},
                { "crimes","crime" },
                { "dress", "dress"},
                { "mess", "mess"},
                { "diary", "diary"},
                { "topiary", "topiary"},
                { "meetings", "meet"},
                { "topiaries", "topiary"},
                { "agreed", "agree"},
                { "agrees", "agree"},
                { "pier", "pier"}};

            foreach (KeyValuePair<string, string> kvp in testData)
            {
                string word = ws.Stem(kvp.Key);
                Assert.AreEqual(kvp.Value, word);
            }
        }
    }
}
