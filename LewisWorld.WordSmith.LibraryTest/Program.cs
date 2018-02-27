using System;
using System.Collections.Generic;

namespace LewisWorld.WordSmith.LibraryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Library library = new Library("/Users/mark/words.index"))
                {
                    WordLookup lookup = new WordLookup(library);
                    var cards = new List<ICard>();
                    cards.Add(new SimpleCard("a"));
                    cards.Add(new SimpleCard("b"));
                    cards.Add(new SimpleCard("br"));
                    cards.Add(new SimpleCard("e"));
                    cards.Add(new SimpleCard("v"));
                    cards.Add(new SimpleCard("i"));
                    cards.Add(new SimpleCard("a"));
                    cards.Add(new SimpleCard("t"));
                    cards.Add(new SimpleCard("e"));
                    cards.Add(new SimpleCard("d"));
                    foreach(String word in lookup.Lookup(cards))
                    {
                        Console.WriteLine(word);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
