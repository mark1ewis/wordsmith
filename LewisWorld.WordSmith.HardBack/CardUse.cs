using System;
namespace LewisWorld.WordSmith.HardBack
{
    public class CardUse
    {
        public CardUse(ICard card) : this(card, '\0') {}
        public CardUse(char wildChar) : this(null, wildChar) {}
        public CardUse(ICard card, char wildChar)
        {
            Card = card;
            Wild = wildChar;
        }

        public ICard Card { get; private set; }
        public char Wild { get; private set; }

		public override string ToString()
        {
            if (Card == null) return "Wild("+Wild+")";
            else return Card.ToString();
        }
	}
}
