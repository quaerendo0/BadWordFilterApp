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
        public AxoCorasickTrie(IEnumerable<string> words)
        {
            Populate(words);
            UpdateFallbackLinks();
        }
        public bool Constains(string word)
        {
            return FindAll(word).Count > 0;
        }
        public List<MatchPosition> FindAll(string input, bool considerDuplicateSymbols = false, bool considerObfuscators = false, bool considerWhitespaces = false, bool ignoreCase = false)
        {
            List<MatchPosition> list = new List<MatchPosition>();
            TrieNode node = root;
            int beginIndex = 0;
            int endIndex = 0;
            int index = 0;
            while (index < input.Length)
            {
                char character = ignoreCase ? Char.ToLower(input[index]) : input[index];
                bool enterCheck = considerWhitespaces ? !Char.IsWhiteSpace(character) : true;
                if (enterCheck)
                {
                    TrieNode newnode = node.Children.Find(n => n.Data == character);
                    if (considerDuplicateSymbols && newnode == null)
                    {
                        bool isDuplicate = node.Data == character;
                        if (!isDuplicate && considerObfuscators)
                        {
                            isDuplicate = ObfuscatorDataBase.Obfuscators(node.Data).Contains(character);
                        }
                        if (isDuplicate)
                        {
                            index++;
                            continue;
                        }
                    }
                    if (considerObfuscators && newnode == null)
                    {
                        foreach (var chr in ObfuscatorDataBase.Obfuscators(character))
                        {
                            newnode = node.Children.Find(n => n.Data == chr);
                            if (newnode != null)
                            {
                                break;
                            }
                        }
                    }
                    if (newnode != null)
                    {
                        if (newnode.Children.Count == 0)
                        {
                            while ((index+1) < input.Length && (input[(index+1)] == newnode.Data || ObfuscatorDataBase.Obfuscators(newnode.Data).Contains(input[index])))
                            {
                                index++;
                            }
                            endIndex = index;
                            var aa = new Tuple<int, int>(beginIndex, endIndex);
                            list.Add(new MatchPosition { StartIndex = beginIndex, EndIndex = endIndex });
                        }
                        else if (node == root)
                        {
                            beginIndex = index;
                        }
                        node = newnode;
                    }
                    else
                    {
                        if (node != root)
                        {
                            node = node.FallbackNode;
                            index--;
                        }
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
            foreach (var child in root.Children)
            {
                UpdateFallbackLink(child);
            }
        }
        private void UpdateFallbackLink(TrieNode node)
        {
            TrieNode fallbackNode;
            if (node.Parent == root)
            {
                fallbackNode = root;
            }
            else
            {
                fallbackNode = Bfs(node.Parent.FallbackNode, node);
            }
            if (fallbackNode != null && fallbackNode != node)
            {
                node.FallbackNode = fallbackNode;
            }
            else
            {
                node.FallbackNode = root;
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
                add(word);
            }
        }
        private void add(TrieNode node, string word, int depth)
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
                add(n, word.Remove(0, 1), depth + 1);
            }
        }
        private void add(string word)
        {
            add(root, word, 0);
        }
    }
}
