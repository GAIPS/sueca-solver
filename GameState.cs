using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class GameState
	{

		private List<Trick> tricks;
		// private int currentTrick;
		private bool debugFlag;
		private Player[] players;
		private Suit trump;

		public GameState(int numTricks, Suit trumpSuit, Player[] playersList, bool debug)
		{
			players = new Player[4];
			tricks = new List<Trick>(numTricks);
			trump = trumpSuit;
			debugFlag = debug;

			// for (int i = 0; i < numTricks; i++)
			// {
			// 	tricks[i] = new Trick(trumpSuit, debug);
			// }

			for (int i = 0; i < 4; i++)
			{
				players[i] = playersList[i];
			}
			// currentTrick = 0;
		}

		// public int GetCurrentTrickIndex()
		// {
		// 	return currentTrick;
		// }

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

		public void ApplyMove(Move move)
		{
			// tricks[currentTrick].ApplyMove(move);

			// if (tricks[currentTrick].IsEndTrick() && (currentTrick + 1) < tricks.Length)
			// {
			// 	currentTrick++;
			// }

			if (tricks.Count == 0 || GetCurrentTrick().IsFull()) 
			{
				tricks.Add(new Trick(trump, debugFlag));
			}
			GetCurrentTrick().ApplyMove(move);
		}

		public void UndoMove()
		{
			// if (IsNewTrick())
			// {
			// 	currentTrick--;
			// }
			// tricks[currentTrick].UndoMove();

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
			// if (currentTrick == tricks.Length || tricks[currentTrick].IsNewTrick())
			if (tricks.Count == 0 || GetCurrentTrick().IsEmpty())
			{
				return true;
			}
			return false;
		}

		public bool IsEndGame()
		{
			// if (tricks[tricks.Length - 1].IsEndTrick())
			if (tricks.Count == tricks.Capacity && GetCurrentTrick().IsFull())
			{
				return true;
			}
			return false;
		}


		public bool IsEndFirstTrick()
		{
			if (tricks[0].IsFull())
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

		public int EvalGame()
		{
			int result = 0;
			for (int i = 0; i < tricks.Count; i++)
			{
				if(debugFlag) Console.WriteLine("--- Trick " + i + ": ---");
				int trickResult = tricks[i].GetTrickPoints();
				result += trickResult;
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


		public void PrintLastTrick()
		{
			if (tricks.Count > 0)
			{
				Console.WriteLine("Last trick:");
				tricks[tricks.Count - 1].PrintTrick();
				Console.WriteLine("");
			}
		}

		public void PrintCurrentTrick()
		{
			Console.WriteLine("Current trick:");
			if (tricks.Count > 0)
			{
				GetCurrentTrick().PrintTrick();
			}
			Console.WriteLine("");
		}

	}
}