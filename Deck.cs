using System;
using System.Linq;
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

		private int getNumPlayer(Decision[] decisions, Decision id)
		{
			int count = 0;
			foreach (Decision d in decisions)
			{
				if (d.Equals(id))
				{
					count++;
				}
			}
			Console.WriteLine("count " + count);
			if (count <= 10)
			{
				return 1;
			}
			return -1;
		}


		private List<List<int>> getDomains(int[] handSizes)
		{
			List<List<int>> list = new List<List<int>>(3);
			for (int i = 0; i < 3; i++)
			{
				list.Add(new List<int>(handSizes[i]));
				for (int j = 0; j < handSizes[i]; j++)
				{
					list[i].Add((i + 1) * 10 + j);
				}
			}
			return list;
		}

		private void printLOL(string name, List<int> lol)
		{
			string res = name + " ";
			foreach (int num in lol)
			{
				res += num + ", ";
			}
			Console.WriteLine(res);
		}


		private List<Card> shuffle(List<Card> cards)
		{
			List<Card> shuffled = new List<Card>(cards.Count);
			for (int randomIndex = 0, j = 0; j < cards.Count; j++)
			{
				randomIndex = random.Next(0, cards.Count);
				Card randomCard = cards[randomIndex];
				shuffled.Add(randomCard);
			}
			return shuffled;
		}


		public List<List<Card>> SampleHands(Dictionary<int,List<int>> suitHasPlayer, int[] handSizes)
		{
			List<Card> shuffledDeck = shuffle(deck);
			var solver = SolverContext.GetContext();
			var model = solver.CreateModel();
			Decision[] decisions = new Decision[shuffledDeck.Count];
			List<List<int>> players = getDomains(handSizes);
			List<int> player1 = players[0];
			List<int> player1Copy = new List<int>(player1);
			List<int> player2 = players[1];
			List<int> player3 = players[2];
			List<int> player3Copy = new List<int>(player3);

			Domain domain1 = Domain.Set(player1.ToArray());
			Domain domain2 = Domain.Set(player2.ToArray());
			Domain domain3 = Domain.Set(player3.ToArray());
			player1.AddRange(player2);
			Domain domain12 = Domain.Set(player1.ToArray());
			player2.AddRange(player3);
			Domain domain23 = Domain.Set(player2.ToArray());
			player3.AddRange(player1Copy);
			Domain domain13 = Domain.Set(player3.ToArray());
			player1.AddRange(player3Copy);
			Domain domain123 = Domain.Set(player1.ToArray());


			for (int i = 0; i < shuffledDeck.Count; i++)
			{
				List<int> playersThatHaveSuit = suitHasPlayer[(int)shuffledDeck[i].Suit];

				if (playersThatHaveSuit.Count == 3)
				{
					decisions[i] = new Decision(domain123, "c" + i);
				}
				else if (playersThatHaveSuit.Count == 2 && playersThatHaveSuit[0] == 1 && playersThatHaveSuit[1] == 2)
				{
					decisions[i] = new Decision(domain12, "c" + i);
				}
				else if (playersThatHaveSuit.Count == 2 && playersThatHaveSuit[0] == 1 && playersThatHaveSuit[1] == 3)
				{
					decisions[i] = new Decision(domain13, "c" + i);
				}
				else if (playersThatHaveSuit.Count == 2 && playersThatHaveSuit[0] == 2 && playersThatHaveSuit[1] == 3)
				{
					decisions[i] = new Decision(domain23, "c" + i);
				}
				else if (playersThatHaveSuit[0] == 1)
				{
					decisions[i] = new Decision(domain1, "c" + i);
				}
				else if (playersThatHaveSuit[0] == 2)
				{
					decisions[i] = new Decision(domain2, "c" + i);
				}
				else
				{
					decisions[i] = new Decision(domain3, "c" + i);
				}

				model.AddDecision(decisions[i]);
			}
			model.AddConstraint("allDiff", Model.AllDifferent(decisions));
			var solution = solver.Solve();

			if (solution.Quality != SolverQuality.Feasible)
			{
				Console.Write("CSP Problem - solution {0}", solution.Quality);
			}

			List<List<Card>> cardsPerPlayer = new List<List<Card>>(3);
			cardsPerPlayer.Add(new List<Card>(handSizes[0]));
			cardsPerPlayer.Add(new List<Card>(handSizes[1]));
			cardsPerPlayer.Add(new List<Card>(handSizes[2]));

			for (int i = 0; i < shuffledDeck.Count; i++)
			{
				int lol = Convert.ToInt32(decisions[i].ToString());
				lol = lol / 10;
				cardsPerPlayer[lol - 1].Add(shuffledDeck[i]);
			}

			return cardsPerPlayer;
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
					players[i].Add(randomCard);
					deckCopy.RemoveAt(randomIndex);
				}
				players[i].Sort();
			}

			return players;
		}
	}
}