using System;
using System.Collections.Generic;
using LewisWorld.WordSmith.HardBack;

namespace LewisWorld.WordSmith.HardBackTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Library library = new Library("/Users/mark/words.index"))
                {
                    var verifier = new WordFinder(library);
                    var lookup = new HardBackFinder(library);
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

                    //cards.Add(new SimpleCard("b"));
                    //cards.Add(new SimpleCard("ea"));
                    //cards.Add(new SimpleCard("r"));

                    int resultCount = 0;
                    var startTime = DateTime.Now;
                    foreach(CardUse[] result in lookup.Lookup(cards))
                    {
                        resultCount++;
                        var letters = new List<char>();
                        foreach(var cardUse in result)
                        {
                            if(cardUse.Card != null)
                            {
                                letters.AddRange(cardUse.Card.Letters);
                            }
                            else {
                                letters.Add(Char.ToUpper(cardUse.Wild));
                            }
                        }
                        String word = new string(letters.ToArray());
                        Console.WriteLine(word);
                    }
                    var millis = DateTime.Now.Subtract(startTime).TotalMilliseconds;
                    Console.WriteLine($"Found {resultCount} results in {millis} milliseconds");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
