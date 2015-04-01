namespace SuecaSolver
{
	public class Card
	{

		public Rank Rank;
		public Suit Suit;

		public Card(Rank rank, Suit suit)
		{
			Rank = rank;
			Suit = suit;
		}

		public override string ToString()
		{
			return Rank.ToString() + ' ' + Suit.ToString();
		}

		public bool Equals(Card[] cards)
		{
			for (int i = 0; i < 10; i++)
			{
				if (Rank == cards[i].Rank && Suit == cards[i].Suit)
				{
					return true;
				}
			}
			return false;
		}
	}
}