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
			if (tricks.Count == 0 || GetCurrentTrick().IsFull())
			{
				tricks.Add(new Trick(trump, debugFlag));
			}
			GetCurrentTrick().ApplyMove(move);
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