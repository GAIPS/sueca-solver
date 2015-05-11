using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public abstract class Player
	{

		public int Id;
		public List<Card> Hand;
		public Dictionary<Suit, int> HasSuit;


		public Player(int id, List<Card> hand)
		{
			Id = id;
			Hand = new List<Card>(hand);
			HasSuit = new Dictionary<Suit, int>() {{Suit.Clubs, 0}, {Suit.Diamonds, 0}, {Suit.Hearts, 0}, {Suit.Spades, 0}};

			foreach (Card c in hand) 
			{
				HasSuit[c.Suit]++;
			}
		}


		abstract public int PlayGame(GameState gameState, int alpha, int beta, int lol, Card move = null);
		abstract public int PlayTrick(GameState gameState, int alpha, int beta, Card move = null);


		public int HighestRankForSuit(Suit leadSuit, Suit trump)
		{
			// Console.WriteLine("-- HighestRankForSuit --");
			// Console.WriteLine("leadSuit " + leadSuit);
			// Console.WriteLine("trump " + trump);
			// int highestFromLeadSuit = -1;
			// int highestTrump = -1;

			// foreach (Card card in Hand)
			// {
			// 	if (!card.HasBeenPlayed && card.Suit == leadSuit && ((int) card.Rank) + 1 > highestFromLeadSuit)
			// 	{
			// 		highestFromLeadSuit = ((int) card.Rank) + 1;
			// 	}
			// 	else if (!card.HasBeenPlayed && card.Suit == trump && ((int) card.Rank) + 1 > highestTrump)
			// 	{
			// 		highestTrump = ((int) card.Rank) + 1;
			// 	}
			// }

			// if (highestFromLeadSuit == -1 && highestTrump == -1)
			// {
			// 	return 0;
			// }
			// else if (highestFromLeadSuit == -1)
			// {
			// 	return -1 * highestTrump;
			// }

			// return highestFromLeadSuit;

			if (HasSuit[leadSuit] > 0) 
			{
				int highestFromLeadSuit = 0;

				foreach (Card card in Hand)
				{
					if (!card.HasBeenPlayed && card.Suit == leadSuit && ((int) card.Rank) + 1 > highestFromLeadSuit)
					{
						highestFromLeadSuit = ((int) card.Rank) + 1;
					}
				}

				// Console.WriteLine("Returning leadSuit rank " + highestFromLeadSuit);
				return highestFromLeadSuit;
			}
			else if (HasSuit[trump] > 0)
			{
				int highestTrump = 0;

				foreach (Card card in Hand)
				{
					if (!card.HasBeenPlayed && card.Suit == trump && ((int) card.Rank) + 1 > highestTrump)
					{
						highestTrump = ((int) card.Rank) + 1;
					}
				}

				// Console.WriteLine("Returning trump rank " + highestTrump);
				return highestTrump * -1;
			}
			else
			{
				// Console.WriteLine("Returning none rank ");
				return 0;
			}
		}


		private void printCards(List<Card> cards)
		{
			string str = "PlayerId: " + Id + " - ";
			foreach (Card c in cards)
			{
				str += c.ToString() + ", ";
			}
			Console.WriteLine(str);
		}


		public void PrintHand()
		{
			printCards(Hand);
		}


		public override string ToString()
		{
			return "PID: " + Id;
		}
	}
}