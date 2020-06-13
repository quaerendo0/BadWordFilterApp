using BadWordFilterApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BadWordFilterApp.Services
{
    public class AxoCorasickWordFilter : IWordFilter
    {
        private readonly AxoCorasickTrie whitelistTrie;
        private readonly AxoCorasickTrie blacklistTrie;
        public IEnumerable<string> WordWhitelist { get; set; }
        public IEnumerable<string> WordBlacklist { get; set; }
        public AxoCorasickWordFilter()
        {
            try
            {
                var path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "blacklist.cfg");
                WordBlacklist = System.IO.File.ReadAllLines(path);
                path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "whitelist.cfg");
                WordWhitelist = System.IO.File.ReadAllLines(path);
                blacklistTrie = new AxoCorasickTrie(WordBlacklist);
                whitelistTrie = new AxoCorasickTrie(WordWhitelist);
            }
            catch (System.IO.FileNotFoundException)
            {

            }
        }
        private string ExctractWholeWord(string input, int start, int end)
        {
            int beginIndex = start;
            while ((beginIndex - 1) > 0 && Char.IsLetter(input[beginIndex - 1]))
            {
                beginIndex--;
            }
            int endIndex = end;
            while (endIndex < (input.Length - 1) && Char.IsLetter(input[endIndex]))
            {
                endIndex++;
            }
            return new string(input.Substring(beginIndex, endIndex - beginIndex + 1).ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
        public string FilterText(string text)
        {
            StringBuilder censoredTextBuilder = new StringBuilder(text);
            if (blacklistTrie == null || whitelistTrie == null)
            {
                throw new FilterInitializationException("Axo corasick fuck shit");
            }
            var memeyList = blacklistTrie.FindAll(text, 
                AxoCorasickTrie.Options.ConsiderDuplicates | 
                AxoCorasickTrie.Options.ConsiderObfuscators |
                AxoCorasickTrie.Options.ConsiderWhitespaces |
                AxoCorasickTrie.Options.IgnoreCase);
            foreach (var item in memeyList)
            {
                if (!whitelistTrie.Contains(ExctractWholeWord(text, item.StartIndex, item.EndIndex)))
                {
                    int length = item.EndIndex - item.StartIndex + 1;
                    censoredTextBuilder.Remove(item.StartIndex, length);
                    censoredTextBuilder.Insert(item.StartIndex, new string('*', length));
                }
            }
            return censoredTextBuilder.ToString();
        }
    }
}
