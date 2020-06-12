using System.Collections.Generic;

namespace BadWordFilterApp.Models 
{ 
    public class ObfuscatorDataBase
    {
        public static IEnumerable<char> Obfuscators(char c)
        {
            switch (c)
            {
                case '!':
                    return new char[] { 'i' };
                case '+':
                    return new char[] { 't' };
                case 'l':
                    return new char[] { 't' };
                case '4':
                    return new char[] { 'a', 'u' };
                default:
                    return new char[]{  };
            }
        }
    }
}
