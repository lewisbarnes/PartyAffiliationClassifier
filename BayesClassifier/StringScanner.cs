using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PartyAffiliationClassifier
{
    class StringScanner
    {
        public StringScanner()
        {
        }

        public string RemovePunctuation(ref string inputString)
        {
            var sb = new StringBuilder(inputString.ToLower());
            var reader = new StringReader(inputString.ToLower());
            int charPos = 0;
            int charsRead;
            while((charsRead = reader.Peek()) != -1)
            {
                var character = reader.Read();
                if (Char.IsPunctuation((char)character))
                {
                        sb.Remove(charPos, 1);
                        charPos--;
                }
                else
                {
                    switch(character)
                    {
                        case '\r':
                        case '\n':
                            sb[charPos] = " ".ToCharArray()[0];
                            break;
                        default:
                            break;
                    }
                }
                charPos++;
            }
            return sb.ToString(); ;
        }
    }
}
