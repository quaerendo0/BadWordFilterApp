using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadWordFilterApp.Models
{
    public struct TrieSearchResult
    {
        public string MatchedString { get; set; }
        public int StartingIndex { get; set; }
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
            return Find(word).MatchedString != "";
        }
        public TrieSearchResult Find(string word)
        {
            TrieNode node = root;
            StringBuilder wordB = new StringBuilder(word);
            while (wordB.Length > 0)
            {
                char letter = wordB[0];
                wordB.Remove(0, 1);
                TrieNode newnode = node.Children.Find(n => n.Data == letter);
                if (newnode == null)
                {
                    if (node != root)
                    {
                        wordB.Insert(0, letter);
                        node = node.FallbackNode;
                    }
                }
                else
                {
                    if (newnode.Children.Count > 0)
                    {
                        node = newnode;
                    }
                    else
                    {
                        StringBuilder outStringBuilder = new StringBuilder();
                        TrieNode builderNode = newnode;
                        while (builderNode != root)
                        {
                            outStringBuilder.Insert(0, builderNode.Data);
                            builderNode = builderNode.Parent;
                        }
                        return new TrieSearchResult{
                            MatchedString = outStringBuilder.ToString(),
                            StartingIndex = word.Length - wordB.Length - outStringBuilder.Length 
                        };
                    }
                }
            }
            return new TrieSearchResult
            {
                MatchedString = "",
                StartingIndex = -1
            };
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
        private void add(TrieNode node, string word)
        {
            if (word.Length > 0)
            {
                TrieNode n = node.Children.Find(nd => nd.Data == word[0]);
                if (n == null)
                {
                    n = new TrieNode(word[0]);
                    n.Parent = node;
                    node.Children.Add(n);
                }
                add(n, word.Remove(0, 1));
            }
        }
        private void add(string word)
        {
            add(root, word);
        }
    }    
}
