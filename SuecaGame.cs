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

		public SuecaGame(List<Card> p0, List<Card> p1, List<Card> p2, List<Card> p3, Suit trumpSuit, List<Move> alreadyPlayed, bool debug)
		{
			trump = trumpSuit;
			players[0] = new MaxPlayer(0, p0);
			players[1] = new MinPlayer(1, p1);
			players[2] = new MaxPlayer(2, p2);
			players[3] = new MinPlayer(3, p3);
			debugFlag = debug;

			gameState = new GameState(p0.Count, trump, players, debug);

			if (alreadyPlayed != null)
			{
				foreach (Move move in alreadyPlayed)
				{
					gameState.ApplyMove(move);
				}
			}
		}

		public Trick GetCurrentTrick()
		{
			return gameState.GetCurrentTrick();
		}

		public int GetNextPlayerId()
		{
			return gameState.GetNextPlayer().Id;
		}

		public void PlayCard(int playerID, Card card)
		{
			gameState.ApplyMove(new Move(playerID, card));
		}

		public int SampleGame(Card card = null)
		{
			Player myPlayer = players[0];
			if (debugFlag) PrintPlayersHands();
			int bestmove = myPlayer.PlayGame(gameState, Int32.MinValue, Int32.MaxValue, 0, card);
			return bestmove;
		}

		public int SampleTrick(Card card = null)
		{
			Player myPlayer = players[0];
			if (debugFlag) PrintPlayersHands();
			int bestmove = myPlayer.PlayTrick(gameState, Int32.MinValue, Int32.MaxValue, card);
			return bestmove;
		}


		public void PrintLastTrick()
		{
			gameState.PrintLastTrick();
		}


		public void PrintCurrentTrick()
		{
			gameState.PrintCurrentTrick();
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

		public static void PrintHand(List<Card> hand)
		{
			Console.WriteLine("Your hand:");
			for (int i = 0; i < hand.Count; i++)
			{
				Console.Write("-- ");
			}
			Console.WriteLine("");
			for (int i = 0; i < hand.Count; i++)
			{
				Console.Write(hand[i] + " ");
			}
			Console.WriteLine("");
			for (int i = 0; i < hand.Count; i++)
			{
				Console.Write("-- ");
			}
			Console.WriteLine("");
			for (int i = 0; i < hand.Count; i++)
			{
				Console.Write(" " + i + " ");
			}
			Console.WriteLine("");
			Console.WriteLine("");
		}

		public static List<Card> AllPossibleMoves(List<Card> hand)
		{
			List<Card> result = new List<Card>();
			foreach (Card card in hand)
			{
				if (!card.HasBeenPlayed)
				{
					result.Add(card);
				}
			}
			return removeEquivalentMoves(result);
		}


		public static List<Card> PossibleMoves(List<Card> hand, Suit leadSuit)
		{
			List<Card> result = new List<Card>();
			// Console.WriteLine("-------------------------------");
			// Console.WriteLine("leadSuit " + leadSuit);
			// SuecaGame.PrintCards("allcards", hand);

			if (leadSuit == Suit.None)
			{
				result = AllPossibleMoves(hand);
				// SuecaGame.PrintCards("result", result);
				return result;
			}

			foreach (Card card in hand)
			{
				if (card.Suit == leadSuit && !card.HasBeenPlayed)
				{
					result.Add(card);
				}
			}

			if (result.Count > 0)
			{
				removeEquivalentMoves(result);
				// SuecaGame.PrintCards("result", result);
				return result;
			}

			result = AllPossibleMoves(hand);
			// SuecaGame.PrintCards("result", result);
			return result;
		}

		private static List<Card> removeEquivalentMoves(List<Card> cards)
		{
			Suit lastSuit = Suit.None;
			int lastRank = (int) Rank.None;
			int lastValue = -1;

			cards.Sort();
			for (int i = 0; i < cards.Count; )
			{
				Card card = cards[i];
				if (lastSuit == card.Suit)
				{
					if (lastValue == card.Value && lastRank  == ((int) card.Rank - 1))
					{
						lastRank = (int) card.Rank;
						cards.RemoveAt(i);
						continue;
					}
					else
					{
						lastValue = card.Value;
						lastRank = (int) card.Rank;
					}
				}
				else
				{
					lastSuit = card.Suit;
					lastValue = card.Value;
					lastRank = (int) card.Rank;
				}
				i++;
			}
			return cards;
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

		public int[] GetGamePoints()
		{
			return gameState.GetGamePoints();
		}	}
}
















