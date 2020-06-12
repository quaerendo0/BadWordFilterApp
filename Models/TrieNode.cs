using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BadWordFilterApp.Models
{
    public class TrieNode
    {
        public TrieNode(char data = '\0')
        {
            Data = data;
        }
        public List<TrieNode> Children { get; set; } = new List<TrieNode>();
        public TrieNode Parent { get; set; }
        public TrieNode FallbackNode { get; set; }
        public char Data { get; set; }
        public int Depth { get; set; }
    }
}
