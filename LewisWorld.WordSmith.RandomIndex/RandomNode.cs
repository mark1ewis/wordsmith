using System;
namespace LewisWorld.WordSmith.RandomIndex
{
    public class RandomNode : INode
    {
        private NodeCache cache;
        private UInt32 firstChild;
        private UInt32 nextSibling;

        public RandomNode(NodeCache cache, char letter, UInt32 firstChild, UInt32 nextSibling, bool terminal)
        {
            this.cache = cache;
            this.firstChild = firstChild;
            this.nextSibling = nextSibling;
            Letter = letter;
            Terminal = terminal;
        }

        public char Letter { get; set; }

        public INode FirstChild 
        { 
            get {
                return cache.Get(firstChild);    
            } 
        }

        public INode NextSibling
        { 
            get
            {
                return cache.Get(nextSibling);
            }
        }

        public bool Terminal { get; private set; }

        public INode Get(char letter)
        {
            RandomNode node = cache.Get(firstChild);
            while(node != null)
            {
                if (node.Letter == letter) return node;
                else node = cache.Get(node.nextSibling);
            }
            return null;
        }
    }
}
