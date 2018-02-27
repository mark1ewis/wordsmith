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
        public IEnumerable<ICard[]> Lookup(ICollection<ICard> cardsInHand)
        {
            var cards = new List<ICard>(cardsInHand).ToArray();
            bool[] masked = new bool[cards.Length];
            return Lookup(new Stack<ICard>(), cards, masked, library.Root);
        }

        internal IEnumerable<ICard[]> Lookup(Stack<ICard> path, ICard [] cards, bool [] masked, LibraryNode node)
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
                    path.Push(cards[i]);
                    foreach(ICard[] result in Lookup(path, cards, masked, child))
                    {
                        yield return result;
                    }
                    path.Pop();
                    masked[i] = false;
                }
            }
            if (node.Terminal)
            {
                ICard[] result = path.ToArray();
                Array.Reverse(result);
                yield return result;
            }
        }
    }
}
