using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public abstract class Player
	{

		public int Id;
		public Card[] Hand = new Card[10];
		public Player NextPlayer;

		public Player(int id, Card[] hand)
		{
			Id = id;
			for (int i = 0; i < 10; i++)
			{
				Hand[i] = hand[i];
			}
		}

		abstract public int Play(GameState gameState);

		public Card[] AllAvailableCards()
		{
			List<Card> allAvailableCards = new List<Card>();

			for (int i = 0; i < 10; i++)
			{
				if (!Hand[i].HasBeenPlayed)
				{
					allAvailableCards.Add(Hand[i]);
				}
			}
			return allAvailableCards.ToArray();
		}

		public Card[] AvailableCardsFromSuit(Suit leadSuit)
		{
			List<Card> allAvailableCards = new List<Card>();

			for (int i = 0; i < 10; i++)
			{
				if (!Hand[i].HasBeenPlayed && Hand[i].Suit == leadSuit)
				{
					allAvailableCards.Add(Hand[i]);
				}
			}
			return allAvailableCards.ToArray();
		}

		public Card[] PossibleMoves(GameState gameState)
		{
			if (gameState.IsNewTrick())
			{
				return AllAvailableCards();
			}

			Card[] possibleMoves = AvailableCardsFromSuit(gameState.GetLeadSuit());

			if (possibleMoves.Length == 0)
			{
				return AllAvailableCards();
			}

			return possibleMoves;
		}

		public void PrintCards(Card[] cards)
		{
			string str = "PlayerId: " + Id + " - ";
			foreach (Card c in cards)
			{
				str += c.ToString() + ", ";
			}
			Console.WriteLine(str);
		}

		public override string ToString()
		{
			return "PID: " + Id + " NextPlayer: " + NextPlayer.Id;
		}
	}
}