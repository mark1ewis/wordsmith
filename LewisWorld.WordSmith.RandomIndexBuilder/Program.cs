using System;
using LewisWorld.WordSmith.RandomIndex;

namespace LewisWorld.WordSmith.RandomIndexBuilder
{
    public class Program
    {
        static void Main(string[] args)
        {
            RandomLibraryBuilder builder = new RandomLibraryBuilder();
            builder.Build("/usr/share/dict/words", "/Users/mark/words.random.index");
        }
    }
}
