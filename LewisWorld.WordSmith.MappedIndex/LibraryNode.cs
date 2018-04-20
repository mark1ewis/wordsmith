using System;
using System.Collections;
using System.Collections.Generic;

namespace LewisWorld.WordSmith.MemMapIndex
{
    public class LibraryNode : INode
    {
        private MappedLibrary library;
        private Node node;

        internal LibraryNode(MappedLibrary library, Node node)
        {
            this.library = library;
            this.node = node;
        }

        public INode Get(char letter)
        {
            if (node.firstChild == 0) return null;
            Node n = library.GetNode(node.firstChild);
            while (n.letter < letter && n.nextSibling > 0)
            {
                n = library.GetNode(n.nextSibling);
            }
            if (n.letter == letter) return new LibraryNode(library, n);
            else return null;
        }

        public IEnumerator<INode> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<INode> Children()
        {
            var nextOffset = node.firstChild;
            while (nextOffset > 0)
            {
                Node n = library.GetNode(nextOffset);
                yield return new LibraryNode(library, n);
                nextOffset = n.nextSibling;
            }
        }

        public char Letter => node.letter;
        public INode FirstChild => ResolveNotRoot(node.firstChild);
        public INode NextSibling => ResolveNotRoot(node.nextSibling);
        public bool Terminal => node.terminal;

        private LibraryNode ResolveNotRoot(UInt32 offset)
        {
            if (offset > 0) return new LibraryNode(library, library.GetNode(offset));
            else return null;
        }
    }
}
