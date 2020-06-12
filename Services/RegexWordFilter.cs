using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;

namespace BadWordFilterApp.Services
{
    public class RegexWordFilter : IWordFilter
    {
        public IEnumerable<string> CensoredWords { get; set; }
        private static readonly string[] defaultBadWords = new string[]
        {
            "*shit*",
            "piss*",
            "*fuck*",
            "cunt",
            "cocksuck*",
            "tits",
            "nigger*",
            "ass"
        };
        public RegexWordFilter()
        {
            try
            {
                var path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "badwords.cfg");
                string[] lines = System.IO.File.ReadAllLines(path);
                CensoredWords = lines;
            }
            catch (System.IO.FileNotFoundException)
            {
                CensoredWords = defaultBadWords;
            }
        }
        public string FilterText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var censoredText = text;

            foreach (string censoredWord in CensoredWords)
            {
                var regularExpression = ToRegexPattern(censoredWord);

                censoredText = Regex.Replace(censoredText, regularExpression, StarCensoredMatch,
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }

            return censoredText;
        }
        static string StarCensoredMatch(Group m) =>
            new string('*', m.Captures[0].Value.Length);

        static string ToRegexPattern(string wildcardSearch)
        {
            var regexPattern = Regex.Escape(wildcardSearch);

            regexPattern = regexPattern.Replace(@"\*", ".*?");
            regexPattern = regexPattern.Replace(@"\?", ".");

            regexPattern = AppendPlusToLetters(regexPattern);

            if (regexPattern.StartsWith(".*?", StringComparison.Ordinal))
            {
                regexPattern = regexPattern.Substring(3);
                regexPattern = @"(^\b)*?" + regexPattern;
            }

            regexPattern = @"\b" + regexPattern + @"\b";

            return regexPattern;
        }
        static string AppendPlusToLetters(string input)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in input)
            {
                builder.Append(c);
                if (Char.IsLetter(c))
                {
                    builder.Append('+');
                }
            }
            return builder.ToString();
        }
    }
    
}
