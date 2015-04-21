using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class SuecaGame
	{

		private Player[] players = new Player[4];
		private Suit trump;
		private GameState gameState;
		private bool debugFlag;

		public SuecaGame(Card[] hand, Suit trumpSuit, bool debug)
		{
			trump = trumpSuit;
			Deck deck = new Deck(hand);
			players[0] = new MaxPlayer(0, hand);
			players[1] = new MinPlayer(1, deck.GetHand(10).ToArray());
			players[2] = new MaxPlayer(2, deck.GetHand(10).ToArray());
			players[3] = new MinPlayer(3, deck.GetHand(10).ToArray());
			debugFlag = debug;

			gameState = new GameState(10, trump, players, debug);
		}

		public SuecaGame(Card[] p0, Card[] p1, Card[] p2, Card[] p3, Suit trumpSuit, Move[] alreadyPlayed, bool debug)
		{
			players[0] = new MaxPlayer(0, p0);
			players[1] = new MinPlayer(1, p1);
			players[2] = new MaxPlayer(2, p2);
			players[3] = new MinPlayer(3, p3);
			debugFlag = debug;

			gameState = new GameState(p0.Length, trump, players, debug);

			if (alreadyPlayed != null)
			{
				foreach (Move move in alreadyPlayed)
				{
					Console.WriteLine("Going to add the already played card: " + move);
					gameState.ApplyMove(move);
				}
			}
		}

		public int SampleGame(Card card = null)
		{
			// PrintPlayersHands();
			Player myPlayer = players[0];
			if (debugFlag) PrintPlayersHands();
			int bestmove = myPlayer.PlayGame(gameState, Int32.MinValue, Int32.MaxValue, card);
			return bestmove;
		}

		// public int SampleTrick()
		// {
		// 	Player myPlayer = players[0];
		// 	int bestmove = myPlayer.PlayTrick(gameState);
		// 	return bestmove;
		// }

		public int SampleTrick(Card card = null)
		{
			Player myPlayer = players[0];
			if (debugFlag) PrintPlayersHands();
			int bestmove = myPlayer.PlayTrick(gameState, card);
			return bestmove;
		}

		public void PrintPlayersHands()
		{
			Console.WriteLine("---------Cards---------");
			players[0].PrintHand();
			players[1].PrintHand();
			players[2].PrintHand();
			players[3].PrintHand();
			Console.WriteLine("-----------------------");
		}

		public static Card[] AllPossibleMoves(Card[] hand)
		{
			List<Card> result = new List<Card>();
			foreach (Card card in hand)
			{
				if (!card.HasBeenPlayed)
				{
					result.Add(card);
				}
			}
			return result.ToArray();
		}


		public static Card[] PossibleMoves(Card[] hand, Suit leadSuit)
		{
			if (leadSuit == Suit.None)
			{
				return AllPossibleMoves(hand);
			}

			List<Card> result = new List<Card>();

			foreach (Card card in hand)
			{
				if (card.Suit == leadSuit && !card.HasBeenPlayed)
				{
					result.Add(card);
				}
			}

			if (result.Count > 0)
			{
				return result.ToArray();
			}

			return AllPossibleMoves(hand);
		}

		public static void PrintCards(string name, List<Card> cards)
		{
			string str = name + " - ";
			for (int i = 0; i < cards.Count; i++)
			{
				str += cards[i].ToString() + ", ";
			}
			Console.WriteLine(str);
		}
	}
}
















