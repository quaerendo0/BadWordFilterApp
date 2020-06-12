using BadWordFilterApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BadWordFilterApp.Services
{    public class WordFilterNotConfigured : Exception
    {
        public WordFilterNotConfigured()
        {
        }

        public WordFilterNotConfigured(string message)
            : base(message)
        {
        }

        public WordFilterNotConfigured(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
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
            }
            catch (System.IO.FileNotFoundException)
            {
                throw new WordFilterNotConfigured("AAAAAAAAAAAAAA");
            }
            blacklistTrie = new AxoCorasickTrie(WordBlacklist);
            whitelistTrie = new AxoCorasickTrie(WordWhitelist);
        }
        static string StarCensoredMatch(Group m) =>
            new string('*', m.Captures[0].Value.Length);

        public string FilterText(string text)
        {
            StringBuilder censoredTextBuilder = new StringBuilder(text);
            int index = 0;
            var wordIndexPair = GetNextWord(text, index);
            while (wordIndexPair.Word != "")
            {
                var blacklistSearchResult = blacklistTrie.Find(wordIndexPair.Word);
                if (blacklistSearchResult.StartingIndex != -1 && !whitelistTrie.Constains(wordIndexPair.Word))
                {
                    censoredTextBuilder.Remove(
                        wordIndexPair.StartingIndex + blacklistSearchResult.StartingIndex,
                        blacklistSearchResult.MatchedString.Length
                        );
                    censoredTextBuilder.Insert(
                        wordIndexPair.StartingIndex + blacklistSearchResult.StartingIndex,
                        new string('*', blacklistSearchResult.MatchedString.Length)
                        );
                }
                index = wordIndexPair.StartingIndex + wordIndexPair.Word.Length;
                wordIndexPair = GetNextWord(text, index);
            }
            return censoredTextBuilder.ToString();
        }
        private struct WordInfo
        {
            public string Word { get; set; }
            public int StartingIndex { get; set; }
        }
        static private WordInfo GetNextWord(string inputText, int startingPoint)
        {
            int position = startingPoint;
            while (position < inputText.Length && Char.IsWhiteSpace(inputText[position]))
            {
                position++;
            }
            StringBuilder builder = new StringBuilder();
            int newStartPoint = position;
            while (position < inputText.Length && !Char.IsWhiteSpace(inputText[position]))
            {
                builder.Append(inputText[position]);
                position++;
            }
            return new WordInfo { 
                Word = builder.ToString(),
                StartingIndex = newStartPoint 
            };
        }
    }
}
