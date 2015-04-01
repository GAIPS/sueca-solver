using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class Deck
	{

		private List<Card> deck = new List<Card>();

		public Deck()
		{
			for (int i = 0; i < 40; i++)
			{
				deck.Add(new Card((Rank) (i % 10), (Suit) ((int) (i / 10))));
			}
		}

		public Deck(Card[] cards)
		{
			for (int i = 0; i < 40; i++)
			{
				Card c = new Card((Rank) (i % 10), (Suit) ((int) (i / 10)));
				if (!c.Equals(cards))
				{
					deck.Add(c);
				}
			}
		}

		public Card[] getHand()
		{
			Card[] hand = new Card[10];
			Random r = new Random();
			for (int randomIndex = 0, i = 0; i < 10; i++)
			{
				randomIndex = r.Next(0, deck.Count);
				hand[i] = deck[randomIndex];
				deck.RemoveAt(randomIndex);
			}
			return hand;
		}
	}
}