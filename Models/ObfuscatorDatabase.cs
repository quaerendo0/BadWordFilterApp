using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BadWordFilterApp.Models
{
    public class ObfuscatorDatabase
    {
        private readonly Dictionary<char, IEnumerable<char>> obfuscators;
        public ObfuscatorDatabase()
        {
            var path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "obfuscators.cfg");
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                obfuscators = new Dictionary<char, IEnumerable<char>>();
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    char keySymbol = line.Split(':')[0][0];
                    IEnumerable<char> possibleMeanings = line.Split(':')[1].ToCharArray().Where(c => c != ',');
                    obfuscators.Add(keySymbol, possibleMeanings);
                }
            }
            catch (FileNotFoundException)
            {

            }
        }
        public IEnumerable<char> Obfuscators(char c)
        {
            if (obfuscators == null)
            {
                throw new ObfuscatorDatabaseNotInitException("Obfuscators were not initialized, file missing?");
            }
            if (obfuscators.ContainsKey(c))
            {
                return obfuscators[c];
            } 
            else
            {
                return new char[] { };
            }
        }
    }
}
