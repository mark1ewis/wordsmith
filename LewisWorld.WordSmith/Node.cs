using System;
namespace LewisWorld.WordSmith
{
    public struct Node
    {
        public const int SIZE = 13;

        public readonly char letter;
        public readonly UInt32 firstChild;
        public readonly UInt32 nextSibling;
        public readonly bool terminal;

        public Node(char letter, UInt32 firstChild, UInt32 nextSibling, bool terminal)
        {
            this.letter = letter;
            this.firstChild = firstChild;
            this.nextSibling = nextSibling;
            this.terminal = terminal;
        }
    }

}
