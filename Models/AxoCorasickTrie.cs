using BadWordFilterApp.Services;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadWordFilterApp.Models
{
    public struct MatchPosition
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
    public class AxoCorasickTrie
    {
        private readonly ObfuscatorDatabase obfuscatorDb = new ObfuscatorDatabase();
        public enum Options
        {
            Default = 1,
            ConsiderDuplicates = 2,
            ConsiderObfuscators = 4,
            ConsiderWhitespaces = 8,
            IgnoreCase = 16
        }
        public AxoCorasickTrie(IEnumerable<string> words)
        {
            Populate(words);
            UpdateFallbackLinks();
        }
        public bool Contains(string word)
        {
            return FindAll(word, Options.Default).Count > 0;
        }
        private bool CheckForDuplicate(char letter1, char letter2, Options options)
        {
            bool isDuplicate = letter1 == letter2;
            if (!isDuplicate && (options.HasFlag(Options.ConsiderObfuscators)))
            {
                isDuplicate = obfuscatorDb.Obfuscators(letter2).Contains(letter1);
            }
            return isDuplicate;
        }
        public List<MatchPosition> FindAll(string input, Options options)
        {
            List<MatchPosition> list = new List<MatchPosition>();
            TrieNode node = root;
            int beginIndex = 0;
            int endIndex = 0;
            int index = 0;
            while (index < input.Length)
            {
                char character = options.HasFlag(Options.IgnoreCase) ? Char.ToLower(input[index]) : input[index];
                bool enterCheck = options.HasFlag(Options.ConsiderWhitespaces) ? !Char.IsWhiteSpace(character) : true;
                if (enterCheck)
                {
                    TrieNode newnode = node.Children.Find(n => n.Data == character);
                    if (newnode == null && options.HasFlag(Options.ConsiderDuplicates))
                    {
                        if (CheckForDuplicate(node.Data, character, options))
                        {
                            index++;
                            continue;
                        }
                    }
                    if (newnode == null && options.HasFlag(Options.ConsiderObfuscators))
                    {
                        newnode = node.Children.Find(n => obfuscatorDb.Obfuscators(character).Contains(n.Data));
                    }
                    if (newnode != null)
                    {
                        if (newnode.Children.Count == 0) // Дошли до конца - слово найдено в трие
                        {
                            if (options.HasFlag(Options.ConsiderDuplicates))
                            {
                                while ((index + 1) < input.Length && CheckForDuplicate(input[index + 1], newnode.Data, options))
                                {
                                    index++;
                                }
                            }
                            endIndex = index;
                            list.Add(new MatchPosition { StartIndex = beginIndex, EndIndex = endIndex });
                        }
                        else if (node == root) // Первый символ в трие найден, можно отсчитывать начало
                        {
                            beginIndex = index;
                        }
                        node = newnode;
                    }
                    else
                    {
                        if (node != root) // Нет смысла отматывать индекс назад если текущий узел - корень, т.е. даже первый символ не совпал
                        {
                            index--;
                        }
                        node = node.FallbackNode;
                    }
                }
                index++;
            }
            return list;
        }
        private readonly TrieNode root = new TrieNode();
        private TrieNode Bfs(TrieNode subroot, TrieNode searchedNode)
        {
            Queue<TrieNode> queue = new Queue<TrieNode>();
            queue.Enqueue(subroot);
            while (queue.Count > 0)
            {
                TrieNode n = queue.Dequeue();
                if (n.Data == searchedNode.Data)
                {
                    return n;
                }
                else
                {
                    foreach (var child in n.Children)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
            return null;
        }
        private void UpdateFallbackLinks()
        {
            UpdateFallbackLink(root);
        }
        private void UpdateFallbackLink(TrieNode node)
        {
            if (node == root)
            {
                node.FallbackNode = root;
            }
            else
            {
                TrieNode fallbackNode = Bfs(node.Parent.FallbackNode, node);
                if (fallbackNode == null || fallbackNode == node)
                {
                    fallbackNode = root;
                }
                node.FallbackNode = fallbackNode;                
            }
            foreach (var child in node.Children)
            {
                UpdateFallbackLink(child);
            }
        }
        private void Populate(IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                Add(word);
            }
        }
        private void Add(TrieNode node, string word, int depth)
        {
            if (word.Length > 0)
            {
                TrieNode n = node.Children.Find(nd => nd.Data == word[0]);
                if (n == null)
                {
                    n = new TrieNode(word[0]);
                    n.Parent = node;
                    n.Depth = depth + 1;
                    node.Children.Add(n);
                }
                Add(n, word.Remove(0, 1), depth + 1);
            }
        }
        private void Add(string word)
        {
            Add(root, word, 0);
        }
    }
}
