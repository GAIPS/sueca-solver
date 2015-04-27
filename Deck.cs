using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class Deck
	{

		public List<Card> deck = new List<Card>();
		private Random random;

		public Deck()
		{
			random = new Random();
			for (int i = 0; i < 40; i++)
			{
				deck.Add(new Card((Rank) (i % 10), (Suit) ((int) (i / 10))));
			}
		}

		public Deck(List<Card> cards)
		{
			random = new Random();
			for (int i = 0; i < 40; i++)
			{
				Card c = new Card((Rank) (i % 10), (Suit) ((int) (i / 10)));
				if (!cards.Contains(c))
				{
					deck.Add(c);
				}
			}
		}

		public int GetSize()
		{
			return deck.Count;
		}

		public void RemoveCard(Card card)
		{
			Console.WriteLine(deck.Remove(card));
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
			hand.Sort();
			return hand;
		}


		public List<Card> SampleHand(int handSize)
		{
			List<Card> hand = new List<Card>(handSize);
			List<Card> deckCopy = new List<Card>(deck);

			for (int randomIndex = 0, i = 0; i < handSize; i++)
			{
				randomIndex = random.Next(0, deckCopy.Count);
				hand.Add(deckCopy[randomIndex]);
				deckCopy.RemoveAt(randomIndex);
			}
			hand.Sort();
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
				players[i].Sort();
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
				players[i].Sort();
			}

			return players;
		}

		public List<List<Card>> SampleHands(Dictionary<int,bool> playerHasSuit, int[] handSizes)
		{
			List<List<Card>> players = new List<List<Card>>();
			List<Card> deckCopy = new List<Card>(deck);

			if (handSizes[0] + handSizes[1] + handSizes[2] != deckCopy.Count) 
			{
				Console.WriteLine("????????????????????????????????????????????");
			}

			for (int i = 0; i < handSizes.Length; i++)
			{
				players.Add(new List<Card>());
				for (int randomIndex = 0, j = 0; j < handSizes[i]; j++)
				{
					randomIndex = random.Next(0, deckCopy.Count);
					Card randomCard = deckCopy[randomIndex];
					int playerID = i + 1;
					int hashCode = (playerID * 10) + (int) randomCard.Suit;

					while (!playerHasSuit[hashCode]) 
					{
						randomIndex = random.Next(0, deckCopy.Count);
						randomCard = deckCopy[randomIndex];
						playerID = i + 1;
						hashCode = (playerID * 10) + (int) randomCard.Suit;
					}

					players[i].Add(randomCard);
					deckCopy.RemoveAt(randomIndex);
				}
				players[i].Sort();
			}

			return players;
		}
	}
}