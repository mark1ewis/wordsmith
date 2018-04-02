using System;
using System.Collections.Generic;

namespace LewisWorld.WordSmith.HardBack
{
    public class HardBackFinder
    {
        private Library library;

        public HardBackFinder(Library library)
        {
            this.library = library;
        }

        public IEnumerable<CardUse[]> Lookup(ICollection<ICard> cardsInHand)
        {
            var cards = new List<ICard>(cardsInHand).ToArray();
            bool[] masked = new bool[cards.Length];
            return Lookup(new Stack<CardUse>(), cards, masked, library.Root, 0);
        }

        internal IEnumerable<CardUse[]> Lookup(Stack<CardUse> path, ICard[] cards, bool[] masked, LibraryNode node, int wildsUsed)
        {
            // Kill if no remaining cards unspoken for by wilds
            var remainingCards = cards.Length - path.Count;

            //Console.WriteLine($"Lookup(path={string.Join<CardUse>(",", path.ToArray())}, cards={string.Join<ICard>(",", cards)}, node={node.Letter}, wildsUsed={wildsUsed}");
            if (remainingCards > wildsUsed)
            {
                var child = node.FirstChild;
                while (child != null)
                {
                    for (var i = 0; i < cards.Length; i++)
                    {
                        var card = cards[i];

                        // Try to play a card for this letter
                        if (!masked[i])
                        {
                            var next = PlayCard(card, child);
                            if (next != null)
                            {
                                masked[i] = true;
                                path.Push(new CardUse(card));
                                foreach (var result in Lookup(path, cards, masked, next, wildsUsed))
                                {
                                    yield return result;
                                }
                                path.Pop();
                                masked[i] = false;
                            }
                        }
                    }

                    // Try to play a wild for this letter
                    path.Push(new CardUse(child.Letter));
                    foreach (var result in Lookup(path, cards, masked, child, wildsUsed + 1))
                    {
                        yield return result;
                    }
                    path.Pop();

                    child = child.NextSibling;
                }
            }
            if (node.Terminal)
            {
                CardUse[] result = path.ToArray();
                Array.Reverse(result);
                yield return result;
            }
        }

        // Returns the LibraryNode after playing this card, including multi-letter cards, 
        // or null if it cannot be played
        internal LibraryNode PlayCard(ICard card, LibraryNode node)
        {
            if(card.Letters[0] != node.Letter)
            {
                return null;
            }
            for (var i = 1; i < card.Letters.Length; i++)
            {
                node = node.Get(card.Letters[i]);
                if (node == null) return null;
            }
            return node;
        }
    }
}
