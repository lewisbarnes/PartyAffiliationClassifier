using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyAffiliationClassifier
{
    public class WordStemmer
    {

        private string[] doubles = { "bb", "dd", "ff", "gg", "mm", "nn", "pp", "rr", "tt" };
        private char[] liEndings = { 'c', 'd', 'e', 'g', 'h', 'k', 'm', 'n', 'r', 't' };
        public WordStemmer()
        {

        }

        public string StemWord(string word)
        {
            if(word.Length <= 2)
            {
                return word;
            }

            int region1 = GetRegionOne(word);
            Console.WriteLine(region1);

            int region2 = GetRegionTwo(word);
            Console.WriteLine(region2);
            return word;

        }
        private int GetRegionOne(string word)
        {
            bool hasVowel = false;
            for(int i = 0; i < word.Length; i++ )
            {
                if(word[i].IsVowel())
                {
                    hasVowel = true;
                }

                if (hasVowel && word[i].IsConsonant())
                {
                    return i + 1;
                }
            }
            return word.Length;
        }

        private int GetRegionTwo(string word)
        {
            bool hasVowel = false;
            for (int i = GetRegionOne(word); i < word.Length; i++)
            {
                if (word[i].IsVowel())
                {
                    hasVowel = true;
                }

                if (hasVowel && word[i].IsConsonant())
                {
                    return i + 1;
                }
            }
            return word.Length;
        }


    }

    public static class Extensions
    {
        private static char[] alphabet = Enumerable.Range('a', 'z' - 'a' + 1).Select(c => (char)c).Concat(new[] { '\'' }).ToArray();
        private static char[] vowels = "aeiou".ToCharArray();
        public static bool IsVowel(this char c)
        {
            return vowels.Contains(c);
        }

        public static bool IsConsonant(this char c)
        {
            return !vowels.Contains(c);
        }
    }

}
