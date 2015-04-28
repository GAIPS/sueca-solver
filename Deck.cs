using System;
using System.Collections.Generic;
using Microsoft.SolverFoundation.Services;

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


		// public List<List<Card>> lol(Dictionary<int,List<int>> suitHasPlayer, int[] handSizes)
		// {
		// 	Dictionary<int,List<int>> copy = new Dictionary<int,List<int>>(suitHasPlayer);
		// 	List<List<Card>> players = new List<List<Card>>();
		// 	List<Card> deckCopy = new List<Card>(deck);

		// 	Action InnedMethod = (int cardIndex) =>
		//     {
		//     	int suit = (int) deckCopy[cardIndex].Suit;
		//     	foreach (Type in copy[suit]) 
		// 		{
		    		
		//     	}
		//     };

		//     InnedMethod();

		//     return players;
		// }


		public void LOL(Dictionary<int,List<int>> suitHasPlayer, int[] handSizes)
		{
			List<Card> deckCopy = new List<Card>(deck);
			var solver = SolverContext.GetContext();
			var model = solver.CreateModel();
			Decision[] decisions = new Decision[deckCopy.Count];
			
			for (int i = 0; i < deckCopy.Count; i++) 
			{
				Domain domain = Domain.Set(new int[]{10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29});
				decisions[i] = new Decision(domain, "c" + i);
				model.AddDecision(decisions[i]);
				// model.AddConstraint("lol" + i, suitHasPlayer[(int)deckCopy[i].Suit].Contains((int)decisions[i]));
				model.AddConstraint("lol" + i, Model.Or(suitHasPlayer[(int)deckCopy[i].Suit][0] == decisions[i] / 10, suitHasPlayer[(int)deckCopy[i].Suit][1] == decisions[i] / 10; suitHasPlayer[(int)deckCopy[i].Suit][2] == decisions[i] / 10));
			}
			model.AddConstraint("lolzinho", Model.AllDifferent(decisions));
			var solution = solver.Solve();
			for (int i = 0; i < deckCopy.Count; i++) 
			{
				Console.WriteLine(decisions[i]);
			}
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
					// int playerID = i + 1;
					// int hashCode = (playerID * 10) + (int) randomCard.Suit;

					// while (!playerHasSuit[hashCode]) 
					// {
					// 	randomIndex = random.Next(0, deckCopy.Count);
					// 	randomCard = deckCopy[randomIndex];
					// 	playerID = i + 1;
					// 	hashCode = (playerID * 10) + (int) randomCard.Suit;
					// }

					players[i].Add(randomCard);
					deckCopy.RemoveAt(randomIndex);
				}
				players[i].Sort();
			}

			return players;
		}
	}
}