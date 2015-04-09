using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public abstract class Player
	{

		public int Id;
		public Card[] Hand;
		public Player NextPlayer;
		private int handSize;

		public Player(int id, Card[] hand)
		{
			int numCards = hand.Length;
			Id = id;
			Hand = new Card[numCards];
			handSize = numCards;
			for (int i = 0; i < numCards; i++)
			{
				Hand[i] = hand[i];
			}
		}

		abstract public int PlayGame(GameState gameState);
		abstract public int PlayTrick(GameState gameState);
		abstract public int PlayTrick(GameState gameState, Card move);

		public Card[] AllAvailableCards()
		{
			List<Card> allAvailableCards = new List<Card>();

			for (int i = 0; i < handSize; i++)
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

			for (int i = 0; i < handSize; i++)
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