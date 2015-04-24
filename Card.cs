using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class Card
	{

		//cardValue are assured by Rank enum order
		private static int[] cardValues = new int[10] {0, 0, 0, 0, 0, 2, 3, 4, 10, 11};
		private static string[] cardRanks = new string[10] {"2", "3", "4", "5", "6", "Q", "J", "K", "7", "A"};

		public Rank Rank;
		public Suit Suit;
		public int Value;
		public bool HasBeenPlayed;
		public int ID;

		public Card(Rank rank, Suit suit)
		{
			ID = ((int) suit) * 10 + (int) rank;
			Rank = rank;
			Suit = suit;
			Value = cardValues[(int)rank];
			HasBeenPlayed = false;
		}

		public override string ToString()
		{
			string rank = cardRanks[(int) Rank];
			string suit = "";
			switch (Suit)
			{
				case Suit.Hearts:
					suit += "H";
					break;
				case Suit.Diamonds:
					suit += "D";
					break;
				case Suit.Spades:
					suit += "S";
					break;
				case Suit.Clubs:
					suit += "C";
					break;
			}
			return rank + suit;
		}

		public bool Equals(List<Card> cards)
		{
			for (int i = 0; i < cards.Count; i++)
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