using System;
namespace LewisWorld.WordSmith
{
    internal struct Node
    {
        internal static UInt32 NODE_SIZE = 11;
        internal readonly char letter;
        internal readonly UInt32 firstChild;
        internal readonly UInt32 nextSibling;
        internal readonly bool terminal;

        internal Node(char letter, UInt32 firstChild, UInt32 nextSibling, bool terminal)
        {
            this.letter = letter;
            this.firstChild = firstChild;
            this.nextSibling = nextSibling;
            this.terminal = terminal;
        }
    }

}
