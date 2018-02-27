using System;
using System.Collections.Generic;

namespace LewisWorld.WordSmith
{
    public class WordLookup
    {
        private Library library;

        public WordLookup(Library library)
        {
            this.library = library;
        }

        /// <summary>
        /// This is a simple sample call that uses the index for looking 
        /// up all possible words with a given starting hand.  A more
        /// sophisticated algorithm for a specific game would likely
        /// use something fancier.
        /// </summary>
        /// <returns>An enumerator for all possible words</returns>
        /// <param name="cardsInHand">Cards in hand.</param>
        public IEnumerable<String> Lookup(ICollection<ICard> cardsInHand)
        {
            var cards = new List<ICard>(cardsInHand).ToArray();
            bool[] masked = new bool[cards.Length];
            return Lookup(new Stack<char>(), cards, masked, library.Root);
        }

        internal IEnumerable<String> Lookup(Stack<char> path, ICard [] cards, bool [] masked, LibraryNode node)
        {
            for (var i = 0; i < cards.Length; i++)
            {
                if (masked[i]) continue;

                // Deal with cards with multiple letters on them
                int letterIndex = 0;
                var child = node.Get(cards[i].Letters[letterIndex]);
                while(child != null && letterIndex < cards[i].Letters.Length-1)
                {
                    letterIndex++;
                    child = child.Get(cards[i].Letters[letterIndex]);
                }

                if(child != null)
                {
                    masked[i] = true;
                    foreach(char ch in cards[i].Letters)
                    {
                        path.Push(ch);
                    }
                    foreach(String result in Lookup(path, cards, masked, child))
                    {
                        yield return result;
                    }
                    foreach(char ch in cards[i].Letters)
                    {
                        path.Pop();
                    }
                    masked[i] = false;
                }
            }
            if (node.Terminal)
            {
                char[] chars = path.ToArray();
                Array.Reverse(chars);
                yield return new string(chars);
            }
        }
    }
}
