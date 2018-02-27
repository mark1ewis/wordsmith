using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LewisWorld.WordSmith
{
    public class LibraryBuilder
    {
        Int64 nodeSize = Marshal.SizeOf(typeof(Node));
        private UInt32 nextOffset;
        private BuildNode root;

        public async Task Build(String wordsPath, String indexPath)
        {
            using (var reader = File.OpenText(wordsPath))
            {
                await Build(reader, indexPath);
            }
        }

        public async Task Build(StreamReader reader, String indexPath)
        {
            root = new BuildNode('\0');
            String line;
            int count = 0;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                line = line.Trim();
                if(IsValid(line))
                {
                    if (count % 1000 == 0)
                    {
                        Console.WriteLine(line);
                    }
                    Add(line);
                    count++;
                }
            }
            Console.WriteLine($"Done, added {count} words, {BuildNode.Count} nodes");
            Console.WriteLine($"GetCount() is {root.GetCount(1)}");
            Deduplicate();
            Console.WriteLine($"GetCount() is {root.GetCount(2)}");
            AssignOffsets();
            Console.WriteLine("Offsets assigned");
            using (var indexFile = MemoryMappedFile.CreateFromFile(indexPath, FileMode.Create, null, nodeSize*nextOffset))
            {
                Console.WriteLine($"Attempting to create view of size {nodeSize * nextOffset}");
                using (var indexView = indexFile.CreateViewAccessor(0, nodeSize*nextOffset))
                {
                    WriteNode(root, indexView, 0);
                    indexView.Flush();
                }
            }
        }

        internal UInt32 WriteNode(BuildNode bn, MemoryMappedViewAccessor indexView, UInt32 offset)
        {
            if (bn == null) return offset; // No node
            if (bn.Offset > offset) throw new ArgumentException($"Offset {offset} does not match node at {bn.Offset}");
            if (bn.Offset < offset) return offset; // Already written
            Node node = bn.ToNode();
            Console.WriteLine($"Writing at {offset*nodeSize}");
            indexView.Write(nodeSize * offset, ref node);
            offset++;
            offset = WriteNode(bn.NextSibling, indexView, offset);
            offset = WriteNode(bn.FirstChild, indexView, offset);
            return offset;
        }

        private void Deduplicate()
        {
            var canonicalNodes = new Dictionary<BuildNode, BuildNode>();
            int processed = root.Deduplicate(canonicalNodes);
            Console.WriteLine($"Deduplicated to {canonicalNodes.Count} nodes, processed {processed} nodes");
        }

        private void AssignOffsets()
        {
            nextOffset = 0;
            var offsets = new Dictionary<BuildNode, UInt32>();
            AssignOffsets(root, offsets);
        }

        private void AssignOffsets(BuildNode bn, Dictionary<BuildNode,UInt32> offsets)
        {
            if(bn != null && bn.Offset < 1)
            {
                bn.Offset = nextOffset++;
                AssignOffsets(bn.NextSibling, offsets);
                AssignOffsets(bn.FirstChild, offsets);
            }
        }

        private void Add(string word)
        {
            BuildNode node = root;
            foreach(var letter in word)
            {
                node = node.GetOrCreateChild(letter);
            }
            node.Terminal = true;
        }

        private bool IsValid(string word)
        {
            foreach(var letter in word)
            {
                if (letter < 'a' || letter > 'z') return false;
            }
            return true;
        }
    }

    internal class BuildNode
    {
        internal static int Count { get; private set; }
        internal UInt32 Offset { get; set; }
        internal char Letter 
        {
            get {
                return letter;
            }    
        }
        internal bool Terminal { 
            get {
                return terminal;
            }
            set {
                if (hashCalculated) throw new InvalidOperationException("Hashcode already calculated");
                terminal = value;
            }
        }
        internal BuildNode FirstChild { get; set; }
        internal BuildNode NextSibling { get; set; }

        readonly char letter;
        bool terminal;
        int mark;
        bool hashCalculated = false;
        int hashCode = -1;

        public Node ToNode()
        {
            UInt32 firstChildOffset = FirstChild!=null ? FirstChild.Offset : 0;
            UInt32 nextSiblingOffset = NextSibling != null ? NextSibling.Offset : 0;
            return new Node(letter, firstChildOffset, nextSiblingOffset, terminal);
        }

        [SuppressMessage("Fields become readonly before hash is calculated", "RECS0025")]
        public override int GetHashCode()
        {
            if (!hashCalculated) 
            {
                hashCode = letter.GetHashCode() * 7;
                if (terminal) hashCode += 97;
                var node = FirstChild;
                while (node != null)
                {
                    hashCode += node.GetHashCode() * 13;
                    node = node.NextSibling;
                }
                hashCalculated = true;
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            BuildNode other = obj as BuildNode;
            if(other != null)
            {
                if (letter != other.letter) return false;
                if (terminal != other.terminal) return false;
                BuildNode myChild = FirstChild;
                BuildNode otherChild = other.FirstChild;
                while(myChild != null)
                {
                    if (!myChild.Equals(otherChild)) return false;
                    myChild = myChild.NextSibling;
                    otherChild = otherChild.NextSibling;
                }
                if (otherChild != null) return false;
                return true;
            }
            else
            {
                return false;
            }
        }

        internal BuildNode(char letter)
        {
            this.letter = letter;
            Count++;
        }

        internal int GetCount(int nextMark)
        {
            if(mark == nextMark)
            {
                return 0;
            }
            mark = nextMark;
            int count = 1;
            BuildNode n = FirstChild;
            while(n != null)
            {
                count += n.GetCount(mark);
                n = n.NextSibling;
            }
            return count;
        }

        internal int Deduplicate(Dictionary<BuildNode,BuildNode> canonicalNodes)
        {
            var processed = 1;
            if(FirstChild != null)
            {
                if (canonicalNodes.ContainsKey(FirstChild))
                {
                    FirstChild = canonicalNodes[FirstChild];
                }
                else
                {
                    canonicalNodes[FirstChild] = FirstChild;
                    processed += FirstChild.Deduplicate(canonicalNodes);
                }
            }

            if(NextSibling != null)
            {
                if (canonicalNodes.ContainsKey(NextSibling))
                {
                    NextSibling = canonicalNodes[NextSibling];
                }
                else
                {
                    canonicalNodes[NextSibling] = NextSibling;
                    processed += NextSibling.Deduplicate(canonicalNodes);
                }
            }

            return processed;
        }

        internal BuildNode GetOrCreateChild(char l)
        {
            if (hashCalculated) throw new InvalidOperationException("Hash already calculated");

            // Case 1, no children yet, start children
            if(FirstChild == null)
            {
                FirstChild = new BuildNode(l);
                return FirstChild;
            }

            // Handle case when the first child is the matching one
            if (FirstChild.letter == l) return FirstChild;

            // Scan through children
            BuildNode n = FirstChild;
            while(n.NextSibling != null && n.NextSibling.letter < l)
            {
                n = n.NextSibling;
            }

            // If I found the child, return it
            if (n.NextSibling != null && n.NextSibling.letter == l) 
            {
                return n.NextSibling;   
            }

            // If not, this is where I need to insert it
            else 
            {
                BuildNode oldNextSibling = n.NextSibling;
                n.NextSibling = new BuildNode(l);
                n.NextSibling.NextSibling = oldNextSibling;
                return n.NextSibling;
            }
        }
    }
}
