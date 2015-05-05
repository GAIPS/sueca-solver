using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public abstract class Player
	{

		public int Id;
		public List<Card> Hand;

		public Player(int id, List<Card> hand)
		{
			Id = id;
			Hand = new List<Card>(hand);
		}

		abstract public int PlayGame(GameState gameState, int alpha, int beta, Card move = null);
		abstract public int PlayTrick(GameState gameState, int alpha, int beta, Card move = null);

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