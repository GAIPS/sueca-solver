using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class GameState
	{

		private List<Trick> tricks;
		private bool debugFlag;
		private Player[] players;
		private Suit trump;

		public GameState(int numTricks, Suit trumpSuit, Player[] playersList, bool debug)
		{
			players = new Player[4];
			tricks = new List<Trick>(numTricks);
			trump = trumpSuit;
			debugFlag = debug;

			for (int i = 0; i < 4; i++)
			{
				players[i] = playersList[i];
			}
		}

		private int getPlayInTrick()
		{
			return GetCurrentTrick().getPlayInTrick();
		}

		public Trick GetCurrentTrick()
		{
			if (tricks.Count == 0)
			{
				Console.WriteLine("Trouble at GameState in GetCurrentTrick");
			}
			return tricks[tricks.Count - 1];
		}

		// This function is always called after applying a move
		public Player GetNextPlayer()
		{
			int nextPlayerId;
			if (GetCurrentTrick().IsFull())
			{
				nextPlayerId = GetCurrentTrick().GetTrickWinnerId();
			} else {
				int lastPlayerId = GetCurrentTrick().GetLastPlayerId();
				nextPlayerId = (lastPlayerId + 1) % 4;
			}
			return players[nextPlayerId];
		}

		public List<Card> orderPossibleMoves(List<Card> moves) //, int playerID)
		{
			Suit leadSuit = GetLeadSuit();
			List<Card> trumps = new List<Card>();
			List<Card> nonTrumps = new List<Card>();
			int currentPlayInTrick = getPlayInTrick();

			check if current player has cards from the leadsuit
			if (moves[0].Suit == leadSuit)
			{
				int bestValueFromSuit = 0;
				int playerWinning;
				for (int i = 0; i < 4; i++)
				{
					if (i != playerID)
					{
						// int cardValue = players[i].GetHigherFromSuit(leadSuit).Value;

						// int cardValue = players[i].HasSuit(leadSuit);
						if (cardValue > bestValueFromSuit)
						{
							bestValueFromSuit = cardValue;
							playerWinning = i;
						}

						if (cardValue < 0)
						{

						}
					}
				}

			} else {

			}



			// if (moves[0].Suit == leadSuit)
			// {
			// 	return moves;
			// }
			// else
			// {
			// 	for (int i = 0; i < moves.Count; i++)
			// 	{
			// 		if (moves[i].Suit == trump)
			// 		{
			// 			trumps.Add(moves[i]);
			// 		} else {
			// 			nonTrumps.Add(moves[i]);
			// 		}
			// 	}
			// 	trumps.AddRange(nonTrumps);
			// 	return trumps;
			// }

		}

		private bool cardsHaveSuit(List<Card> cards, Suit leadSuit)
		{
			foreach (Card card in cards)
			{
				if (card.Suit == leadSuit)
				{
					return true;
				}
			}
			return false;
		}


		public void ApplyMove(Move move)
		{
			// printTricks();
			if (tricks.Count == 0 || GetCurrentTrick().IsFull())
			{
				tricks.Add(new Trick(trump, debugFlag));
			}
			// Console.WriteLine("ApplyMove!!!");
			GetCurrentTrick().ApplyMove(move);
			// printTricks();
			// Console.WriteLine("");
		}

		public void UndoMove()
		{
			GetCurrentTrick().UndoMove();
			if (GetCurrentTrick().IsEmpty())
			{
				tricks.RemoveAt(tricks.Count - 1);
			}
		}

		public Suit GetLeadSuit()
		{
			return GetCurrentTrick().LeadSuit;
		}

		public bool IsNewTrick()
		{
			if (tricks.Count == 0 || GetCurrentTrick().IsEmpty())
			{
				return true;
			}
			return false;
		}

		public bool IsEndGame()
		{
			if (tricks.Count == tricks.Capacity && GetCurrentTrick().IsFull())
			{
				// Console.WriteLine("ENG GAME!!!!");
				return true;
			}
			return false;
		}


		public bool IsEndFirstTrick()
		{
			if (tricks.Count > 0 && tricks[0].IsFull())
			{
				return true;
			}
			return false;
		}

		public bool IsEndTrick()
		{
			if (GetCurrentTrick().IsFull())
			{
				return true;
			}
			return false;
		}



		public int[] GetGamePoints()
		{
			int[] result = new int[2] {0, 0};
			for (int i = 0; i < tricks.Count; i++)
			{
				int trickResult = tricks[i].GetTrickPoints();
				if (trickResult > 0)
				{
					result[0] += trickResult;
				} else {
					result[1] += (-1 * trickResult);
				}
			}
			return result;
		}

		public int EvalGame()
		{
			// Console.WriteLine("EvalGame - tricks.Count " + tricks.Count);
			int result = 0;
			for (int i = 0; i < tricks.Count; i++)
			{
				if(debugFlag) Console.WriteLine("--- Trick " + i + ": ---");
				int trickResult = tricks[i].GetTrickPoints();
				result += trickResult;
				// Console.WriteLine("Trickresult: " + trickResult + " Sum: " + result);
				if(debugFlag) Console.WriteLine("Trickresult: " + trickResult + " Sum: " + result);
			}
			return result;
		}

		public int GetTrickWinnerId()
		{
			return GetCurrentTrick().GetTrickWinnerId();
		}

		public int GetFirstTrickPoints()
		{
			if(debugFlag) Console.WriteLine("--- Trick ---");
			int trickResult = tricks[0].GetTrickPoints();
			if(debugFlag) Console.WriteLine("Trickresult: " + trickResult + " Sum: " + trickResult);
			return trickResult;
		}

		public int GetTrickPoints()
		{
			if(debugFlag) Console.WriteLine("--- Trick ---");
			int trickResult = GetCurrentTrick().GetTrickPoints();
			if(debugFlag) Console.WriteLine("Trickresult: " + trickResult + " Sum: " + trickResult);
			return trickResult;
		}


		private void printTricks()
		{
			Console.WriteLine("printTricks - tricks.Count " + tricks.Count);
			for (int i = 0; i < tricks.Count; i++)
			{
				Console.WriteLine("--- Trick " + i + "---");
				tricks[i].PrintTrick();
			}
		}


		public void PrintLastTrick()
		{
			if (tricks.Count > 0 && tricks[0].IsFull())
			{
				Console.WriteLine("Last trick:");
				if (GetCurrentTrick().IsFull())
				{
					GetCurrentTrick().PrintTrick();
				}
				else
				{
					tricks[tricks.Count - 2].PrintTrick();
				}
				Console.WriteLine("");
			}
		}

		public void PrintCurrentTrick()
		{
			Console.WriteLine("Current trick:");
			if (tricks.Count > 0 && !GetCurrentTrick().IsFull())
			{
				GetCurrentTrick().PrintTrick();
			}
			Console.WriteLine("");
		}

	}
}