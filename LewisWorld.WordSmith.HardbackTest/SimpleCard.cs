using System;
using LewisWorld.WordSmith.HardBack;
namespace LewisWorld.WordSmith.HardBackTest
{
    public class SimpleCard : ICard
    {
        public SimpleCard(String letters)
        {
            this.Letters = letters.ToCharArray();
        }

        public char[] Letters { get; set; }

		public override string ToString()
		{
            return new string(Letters);
		}
	}
}
