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
            var sb = new StringBuilder(inputString.ToLower()); //New string builder containing input string lowercase
            var reader = new StringReader(inputString.ToLower());//New string reader containing input string lowercase
            int charPos = 0;
            int charsRead;
            while((charsRead = reader.Peek()) != -1)
            {
                var character = reader.Read(); // Read the first character in the reader
                if (Char.IsPunctuation((char)character))
                {
                        sb.Remove(charPos, 1); //Remove one character starting at specified index
                        charPos--; //Decrement the position counter as the string contains one
                }
                else
                {
                    switch(character)
                    {
                        case '\r':
                        case '\n':
                            sb[charPos] = " ".ToCharArray()[0]; //Set position at specified index to a space character
                            break;
                        default:
                            break;
                    }
                }
                charPos++; //Increment the char position counter
            }
            return sb.ToString(); // Return the StringBuilder.ToString();
        }
    }
}
