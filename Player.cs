using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public abstract class Player
	{

		public int Id;
		public Card[] Hand;
		public Player NextPlayer;

		public Player(int id, Card[] hand)
		{
			int numCards = hand.Length;
			Id = id;
			Hand = new Card[numCards];
			for (int i = 0; i < numCards; i++)
			{
				Hand[i] = hand[i];
			}
		}

		abstract public int PlayGame(GameState gameState);
		abstract public int PlayTrick(GameState gameState);
		abstract public int PlayTrick(GameState gameState, Card move);

		private void printCards(Card[] cards)
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
			return "PID: " + Id + " NextPlayer: " + NextPlayer.Id;
		}
	}
}