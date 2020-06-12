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
        static string StarCensoredMatch(Group m) =>
            new string('*', m.Captures[0].Value.Length);
        private string ExctractWholeWord(string input, int start, int end)
        {
            int beginindex = start;
            while (beginindex > 0 && Char.IsLetter(input[beginindex]))
            {
                beginindex--;
            }
            beginindex++;
            int endindex = end;
            while (endindex < (input.Length - 1) && Char.IsLetter(input[endindex]))
            {
                endindex++;
            }
            return new string(input.Substring(beginindex, endindex - beginindex + 1).ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
        public string FilterText(string text)
        {
            StringBuilder censoredTextBuilder = new StringBuilder(text);
            if (blacklistTrie == null && whitelistTrie == null)
            {
                throw new FilterInitializationException("Axo corasick fuck shit");
            }
            var memeyList = blacklistTrie.FindAll(text, true, true, true, true);
            foreach (var item in memeyList)
            {
                if (!whitelistTrie.Constains(ExctractWholeWord(text, item.StartIndex, item.EndIndex)))
                {
                    censoredTextBuilder.Remove(item.StartIndex, item.EndIndex - item.StartIndex + 1);
                    censoredTextBuilder.Insert(item.StartIndex, new string('*', item.EndIndex - item.StartIndex + 1));
                }
            }
            return censoredTextBuilder.ToString();
        }
    }
}
