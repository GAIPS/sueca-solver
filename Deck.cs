using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class Deck
	{

		private List<Card> deck = new List<Card>();
		private Random random;

		public Deck()
		{
			random = new Random();
			for (int i = 0; i < 40; i++)
			{
				deck.Add(new Card((Rank) (i % 10), (Suit) ((int) (i / 10))));
			}
		}

		public Deck(Card[] cards)
		{
			random = new Random();
			for (int i = 0; i < 40; i++)
			{
				Card c = new Card((Rank) (i % 10), (Suit) ((int) (i / 10)));
				if (!c.Equals(cards))
				{
					deck.Add(c);
				}
			}
		}

		public List<Card> GetHand(int handSize)
		{
			List<Card> hand = new List<Card>(handSize);
			for (int randomIndex = 0, i = 0; i < handSize; i++)
			{
				randomIndex = random.Next(0, deck.Count);
				hand.Add(deck[randomIndex]);
				deck.RemoveAt(randomIndex);
			}
			return hand;
		}

		public List<List<Card>> Sample(int handSize)
		{

			List<List<Card>> players = new List<List<Card>>();
			List<Card> deckCopy = new List<Card>(deck);

			for (int i = 0; i < 3; i++)
			{
				players.Add(new List<Card>());
				for (int randomIndex = 0, j = 0; j < handSize; j++)
				{
					randomIndex = random.Next(0, deckCopy.Count);
					players[i].Add(deckCopy[randomIndex]);
					deckCopy.RemoveAt(randomIndex);
				}
			}

			return players;
		}


		public List<List<Card>> SampleAll(int n)
		{
			List<List<Card>> players = new List<List<Card>>();
			List<Card> deckCopy = new List<Card>(deck);

			for (int i = 0; i < 4; i++)
			{
				players.Add(new List<Card>());
				for (int randomIndex = 0, j = 0; j < n; j++)
				{
					randomIndex = random.Next(0, deckCopy.Count);
					players[i].Add(deckCopy[randomIndex]);
					deckCopy.RemoveAt(randomIndex);
				}
			}

			return players;
		}
	}
}