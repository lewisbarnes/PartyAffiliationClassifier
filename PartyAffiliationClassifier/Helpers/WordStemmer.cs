using System.Collections.Generic;
using System.Linq;
namespace PartyAffiliationClassifier
{
    public class WordStemmer
    {
        // Implements algorithm http://snowball.tartarus.org/algorithms/english/stemmer.html
        private readonly char[] _alphabet = Enumerable.Range('a', 'z' - 'a' + 1).Select(c => (char)c).Concat(new[] { '\'' }).ToArray();
        private readonly char[] _vowels = "aeiouy".ToArray();
        private readonly string[] _doubles = { "bb", "dd", "ff", "gg", "mm", "nn", "pp", "rr", "tt" };
        private char[] _liEndings = "cdeghkmnrt".ToArray();
        private readonly char[] _nonShortConsonants = "wxY".ToArray();
        public char[] Alphabet { get { return _alphabet; } }
        public char[] Vowels { get { return _vowels; } }
        public char[] LiEndings { get { return _liEndings; } }

        public string[] Doubles { get { return _doubles; } }

        // Special cases that do not stem as expected
        public readonly Dictionary<string, string> _exceptions = new Dictionary<string, string>()
        {
            {"skis", "ski"},
            {"skies", "sky"},
            {"dying", "die"},
            {"lying", "lie"},
            {"tying", "tie"},
            {"idly", "idl"},
            {"gently", "gentl"},
            {"ugly", "ugli"},
            {"early", "earli"},
            {"only", "onli"},
            {"singly", "singl"},
            {"sky", "sky"},
            {"news", "news"},
            {"howe", "howe"},
            {"atlas", "atlas"},
            {"cosmos", "cosmos"},
            {"bias", "bias"},
            {"andes", "andes"}
        };

        private readonly string[] _exceptionsPart2 = new[]
        {
            "inning", "outing","canning","herring","earring","proceed","exceed","succeed"
        };

        private readonly string[] _exceptionsRegion1 = new[]
        {
            "gener","arsen","commun"
        };

        public WordStemmer()
        {

        }

        public string Stem(string word)
        {
            string original = word;
            if (word.Length <= 2)
            {
                return word;
            }

            word = TrimStartingApostrophe(word);

            if (_exceptions.TryGetValue(word, out string excpt))
            {
                return word;
            }

            word = MarkYsAsConsonants(word);

            int r1 = GetRegion1(word);
            int r2 = GetRegion2(word);

            word = Step0RemoveSPluralSuffix(word);
            word = Step1ARemoveOtherSPluralSuffixes(word);

            if (_exceptionsPart2.Contains(word))
            {
                return word;
            }

            word = Step1BRemoveLySuffixes(word, r1);
            word = Step1CReplaceSuffixYWithIIfPreceededWithConsonant(word);
            word = Step2ReplaceSuffixes(word, r1);
            word = Step3ReplaceSuffixes(word, r1, r2);
            word = Step4RemoveSomeSuffixesInR2(word, r2);
            word = Step5RemoveEorLSuffixes(word, r1, r2);

            return word;

        }

        private static string TrimStartingApostrophe(string word)
        {
            if (word.StartsWith("'"))
            {
                word = word.Substring(1);
            }
            return word;
        }

        private string MarkYsAsConsonants(string word)
        {
            char[] chars = word.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 0)
                {
                    if (chars[i] == 'y')
                    {
                        chars[i] = 'Y';
                    }
                }
                else if (Vowels.Contains(chars[i - 1]) && chars[i] == 'y')
                {
                    chars[i] = 'Y';
                }
            }
            return new string(chars);
        }
        public int GetRegion1(string word)
        {
            foreach (string except in _exceptionsRegion1.Where(word.StartsWith))
            {
                return except.Length;
            }
            return GetRegion(word, 0);
        }

        public int GetRegion2(string word)
        {
            int r1 = GetRegion1(word);
            return GetRegion(word, r1);
        }

        private int GetRegion(string word, int begin)
        {
            bool foundVowel = false;
            for (int i = begin; i < word.Length; i++)
            {
                if (IsVowel(word[i]))
                {
                    foundVowel = true;
                    continue;
                }
                if (foundVowel && IsConsonant(word[i]))
                {
                    return i + 1;
                }
            }
            return word.Length;
        }

        private bool IsConsonant(char c)
        {
            return !Vowels.Contains(c);
        }

        private bool IsVowel(char c)
        {
            return Vowels.Contains(c);
        }

        private static bool SuffixInR1(string word, int r1, string suffix)
        {
            return r1 <= word.Length - suffix.Length;
        }

        private bool SuffixInR2(string word, int r2, string suffix)
        {
            return r2 <= word.Length - suffix.Length;
        }

        public bool EndsInShortSyllable(string word)
        {
            if (word.Length < 2)
            {
                return false;
            }

            // a vowel at the beginning of the word followed by a non-vowel
            if (word.Length == 2)
            {
                return IsVowel(word[0]) && IsConsonant(word[1]);
            }

            return IsVowel(word[word.Length - 2])
                   && IsConsonant(word[word.Length - 1])
                   && !_nonShortConsonants.Contains(word[word.Length - 1])
                   && IsConsonant(word[word.Length - 3]);
        }
        public bool IsShortWord(string word)
        {
            return EndsInShortSyllable(word) && GetRegion1(word) == word.Length;
        }


        public string Step0RemoveSPluralSuffix(string word)
        {
            // Longest to shortest order
            string[] suffixes = new[] { "'s'", "'s", "'" };
            foreach (string suffix in suffixes)
            {
                if (word.EndsWith(suffix))
                {
                    return ReplaceSuffix(word, suffix);
                }
            }
            return word;
        }

        public string Step1ARemoveOtherSPluralSuffixes(string word)
        {
            if (word.EndsWith("sses"))
            {
                return ReplaceSuffix(word, "sses", "ss");
            }
            if (word.EndsWith("ied") || word.EndsWith("ies"))
            {
                string restOfWord = word.Substring(0, word.Length - 3);
                if (word.Length > 4)
                {
                    return restOfWord + "i";
                }
                return restOfWord + "ie";
            }
            if (word.EndsWith("us") || word.EndsWith("ss"))
            {
                return word;
            }
            if (word.EndsWith("s"))
            {
                if (word.Length < 3)
                {
                    return word;
                }

                // Skip last two letters in word
                for (int i = 0; i < word.Length - 2; i++)
                {
                    if (IsVowel(word[i]))
                    {
                        return word.Substring(0, word.Length - 1);
                    }
                }
            }
            return word;
        }

        public string Step1BRemoveLySuffixes(string word, int r1)
        {
            foreach (string suffix in new[] { "eedly", "eed" }.Where(word.EndsWith))
            {
                if (SuffixInR1(word, r1, suffix))
                {
                    return ReplaceSuffix(word, suffix, "ee");
                }
                return word;
            }

            foreach (string suffix in new[] { "ed", "edly", "ing", "ingly" }.Where(word.EndsWith))
            {
                string trunc = ReplaceSuffix(word, suffix);
                if (trunc.Any(IsVowel))
                {
                    if (new[] { "at", "bl", "iz" }.Any(trunc.EndsWith))
                    {
                        return trunc + "e";
                    }
                    if (Doubles.Any(trunc.EndsWith))
                    {
                        return trunc.Substring(0, trunc.Length - 1);
                    }
                    if (IsShortWord(trunc))
                    {
                        return trunc + "e";
                    }
                    return trunc;
                }
                return word;
            }
            return word;
        }

        public string Step1CReplaceSuffixYWithIIfPreceededWithConsonant(string word)
        {
            if ((word.EndsWith("y") || word.EndsWith("Y"))
                && word.Length > 2
                && IsConsonant(word[word.Length - 2]))
            {
                return word.Substring(0, word.Length - 1) + "i";
            }
            return word;
        }

        public string Step2ReplaceSuffixes(string word, int r1)
        {
            Dictionary<string, string> suffixes = new Dictionary<string, string>
                {
                    {"ization", "ize"},
                    {"ational", "ate"},
                    {"ousness", "ous"},
                    {"iveness", "ive"},
                    {"fulness", "ful"},
                    {"tional", "tion"},
                    {"lessli", "less"},
                    {"biliti", "ble"},
                    {"entli", "ent"},
                    {"ation", "ate"},
                    {"alism", "al"},
                    {"aliti", "al"},
                    {"fulli", "ful"},
                    {"ousli", "ous"},
                    {"iviti", "ive"},
                    {"enci", "ence"},
                    {"anci", "ance"},
                    {"abli", "able"},
                    {"izer", "ize"},
                    {"ator", "ate"},
                    {"alli", "al"},
                    {"bli", "ble"}
                };
            foreach (KeyValuePair<string, string> suffix in suffixes)
            {
                if (word.EndsWith(suffix.Key))
                {
                    string final;
                    if (SuffixInR1(word, r1, suffix.Key)
                        && TryReplace(word, suffix.Key, suffix.Value, out final))
                    {
                        return final;
                    }
                    return word;
                }
            }

            if (word.EndsWith("ogi")
                && SuffixInR1(word, r1, "ogi")
                && word[word.Length - 4] == 'l')
            {
                return ReplaceSuffix(word, "ogi", "og");
            }

            if (word.EndsWith("li") & SuffixInR1(word, r1, "li"))
            {
                if (LiEndings.Contains(word[word.Length - 3]))
                {
                    return ReplaceSuffix(word, "li");
                }
            }

            return word;
        }

        public string Step3ReplaceSuffixes(string word, int r1, int r2)
        {
            Dictionary<string, string> suffixes = new Dictionary<string, string>
                {
                    {"ational", "ate"},
                    {"tional", "tion"},
                    {"alize", "al"},
                    {"icate", "ic"},
                    {"iciti", "ic"},
                    {"ical", "ic"},
                    {"ful", null},
                    {"ness", null}
                };
            foreach (KeyValuePair<string, string> suffix in suffixes.Where(s => word.EndsWith(s.Key)))
            {
                string final;
                if (SuffixInR1(word, r1, suffix.Key)
                    && TryReplace(word, suffix.Key, suffix.Value, out final))
                {
                    return final;
                }
            }

            if (word.EndsWith("ative"))
            {
                if (SuffixInR1(word, r1, "ative") && SuffixInR2(word, r2, "ative"))
                {
                    return ReplaceSuffix(word, "ative", null);
                }
            }

            return word;
        }

        public string Step4RemoveSomeSuffixesInR2(string word, int r2)
        {
            foreach (string suffix in new[]
                {
                    "al", "ance", "ence", "er", "ic", "able", "ible", "ant",
                    "ement", "ment", "ent", "ism", "ate", "iti", "ous",
                    "ive", "ize"
                })
            {
                if (word.EndsWith(suffix))
                {
                    if (SuffixInR2(word, r2, suffix))
                    {
                        return ReplaceSuffix(word, suffix);
                    }
                    return word;
                }
            }

            if (word.EndsWith("ion") &&
                SuffixInR2(word, r2, "ion") &&
                new[] { 's', 't' }.Contains(word[word.Length - 4]))
            {
                return ReplaceSuffix(word, "ion");
            }
            return word;
        }

        public string Step5RemoveEorLSuffixes(string word, int r1, int r2)
        {
            // Remove 'e' if suffix in R1 or R2 and not ending in short syllable
            if (word.EndsWith("e") &&
                (SuffixInR2(word, r2, "e") ||
                    (SuffixInR1(word, r1, "e") &&
                        !EndsInShortSyllable(ReplaceSuffix(word, "e")))))
            {
                return ReplaceSuffix(word, "e");
            }
            // Remove 'l' if suffix in R2 and word length greater than one
            if (word.EndsWith("l") &&
                SuffixInR2(word, r2, "l") &&
                word.Length > 1 &&
                word[word.Length - 2] == 'l')
            {
                return ReplaceSuffix(word, "l");
            }
            return word;
        }
        // Replace suffix in string with new provided suffix
        private string ReplaceSuffix(string word, string oldSuffix, string newSuffix = null)
        {
            if (oldSuffix != null)
            {
                word = word.Substring(0, word.Length - oldSuffix.Length);
            }

            if (newSuffix != null)
            {
                word += newSuffix;
            }
            return word;
        }

        private bool TryReplace(string word, string oldSuffix, string newSuffix, out string final)
        {
            if (word.Contains(oldSuffix))
            {
                final = ReplaceSuffix(word, oldSuffix, newSuffix);
                return true;
            }
            final = word;
            return false;

        }
    }
}
