using System;
using LewisWorld.WordSmith;
namespace LewisWorld.WordSmith.LibraryTest
{
    public class SimpleCard : ICard
    {
        public SimpleCard(String letters)
        {
            this.Letters = letters.ToCharArray();
        }

        public char[] Letters { get; set; }
    }
}
