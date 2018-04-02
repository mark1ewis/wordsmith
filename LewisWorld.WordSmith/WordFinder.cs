using System;
using System.Collections.Generic;

namespace LewisWorld.WordSmith
{
    public class WordFinder
    {
        private Library library;

        public WordFinder(Library library)
        {
            this.library = library;
        }

        public bool IsInLibrary(string word)
        {
            var node = library.Root;
            foreach (var ch in word)
            {
                node = node.Get(ch);
                if (node == null) return false;
            }
            return node.Terminal;
        }
    }
}
