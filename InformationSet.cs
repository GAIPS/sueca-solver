using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class InformationSet
	{

		public int[] Hand = new int[10];
		public List<int> Deck = new List<int>();
		public int[] player1 = new int[10];
		public int[] player2 = new int[10];
		public int[] player3 = new int[10];

		public InformationSet()
		{
			for (int i = 0; i < 40; i++)
			{
				Deck.Add(i);
			}
			distributeCardToPlayer(Deck, Hand);
		}

		public List<int> distributeCardToPlayer(List<int> deck, int[] player)
		{
			Random r = new Random();
			for (int randomIndex = 0, i = 0; i < 10; i++)
			{
				randomIndex = r.Next(0, deck.Count);
				player[i] = deck[randomIndex];
				deck.RemoveAt(randomIndex);
			}
			return deck;
		}

		public void sample()
		{
			List<int> deck = new List<int>(Deck);
			deck = distributeCardToPlayer(deck, player1);
			deck = distributeCardToPlayer(deck, player2);
			deck = distributeCardToPlayer(deck, player3);
		}

		public void PrintPlayerHand(int[] player, string name)
		{
			string playerHand = name + ": ";
			for (int i = 0; i < player.Length; i++)
			{
				playerHand += player[i].ToString() + " ";
			}
			Console.WriteLine(playerHand);
		}

		public void PrintDeck()
		{
			string deck = "Deck: ";
			for (int i = 0; i < Deck.Count; i++)
			{
				deck += Deck[i].ToString() + " ";
			}
			Console.WriteLine(deck);
		}
	}
}