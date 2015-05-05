using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class Card : IComparable, IEquatable<Card>
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

		public override int GetHashCode()
		{
			return ((int) this.Suit) * 10 + (int) this.Rank;
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

		public bool Equals(Card other)
		{
			if (other == null)
			{
				return false;
			}

			if (this.Rank == other.Rank && this.Suit == other.Suit)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override bool Equals(Object obj)
		{
			if (obj == null)
			{
				return false;
			}

			Card card = obj as Card;
			if (card == null)
			{
				return false;
			}
			else
			{
				return Equals(card);
			}
		}

		// public bool Equals(List<Card> cards)
		// {
		// 	for (int i = 0; i < cards.Count; i++)
		// 	{
		// 		if (Rank == cards[i].Rank && Suit == cards[i].Suit)
		// 		{
		// 			return true;
		// 		}
		// 	}
		// 	return false;
		// }

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}

			Card card = obj as Card;

			if ((int) Suit < (int) card.Suit)
			{
				return -1;
			}
			else if ((int) Suit == (int) card.Suit && (int) Rank < (int) card.Rank)
			{
				return -1;
			}
			else
			{
				return 1;
			}
		}
	}
}