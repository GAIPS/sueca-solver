using System;

namespace SuecaSolver
{
	public class GameState
	{

		private Trick[] tricks;
		private int currentTrick;
		private bool debugFlag;
		private Player[] players;

		public GameState(int numTricks, Suit trumpSuit, Player[] playersList, bool debug)
		{
			players = new Player[4];
			tricks = new Trick[numTricks];
			debugFlag = debug;
			for (int i = 0; i < numTricks; i++)
			{
				tricks[i] = new Trick(trumpSuit, debug);
			}
			for (int i = 0; i < 4; i++)
			{
				players[i] = playersList[i];
			}
			currentTrick = 0;
		}

		public int GetCurrentTrick()
		{
			return currentTrick;
		}

		// This function is always called after applying a move
		public Player GetNextPlayer()
		{
			int nextPlayerId;
			if (tricks[currentTrick].IsNewTrick())
			{
				nextPlayerId = tricks[currentTrick - 1].GetTrickWinnerId();
			} else {
				int lastPlayerId = tricks[currentTrick].GetLastPlayerId();
				nextPlayerId = (lastPlayerId + 1) % 4;
			}
			return players[nextPlayerId];
		}

		public void ApplyMove(Move move)
		{
			tricks[currentTrick].ApplyMove(move);

			if (tricks[currentTrick].IsEndTrick() && (currentTrick + 1) < tricks.Length)
			{
				currentTrick++;
			}
		}

		public void UndoMove()
		{
			if (IsNewTrick())
			{
				currentTrick--;
			}
			tricks[currentTrick].UndoMove();
		}

		public Suit GetLeadSuit()
		{
			return tricks[currentTrick].LeadSuit;
		}

		public bool IsNewTrick()
		{
			if (currentTrick == tricks.Length || tricks[currentTrick].IsNewTrick())
			{
				return true;
			}
			return false;
		}

		public bool IsEndGame()
		{
			if (tricks[tricks.Length - 1].IsEndTrick())
			{
				return true;
			}
			return false;
		}


		public bool IsEndFirstTrick()
		{
			if (tricks[0].IsEndTrick())
			{
				return true;
			}
			return false;
		}

		public bool IsEndTrick()
		{
			if (tricks[currentTrick].IsEndTrick())
			{
				return true;
			}
			return false;
		}

		public int EvalGame()
		{
			int result = 0;
			for (int i = 0; i < tricks.Length; i++)
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
			return tricks[currentTrick].GetTrickWinnerId();
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
			int trickResult = tricks[currentTrick].GetTrickPoints();
			if(debugFlag) Console.WriteLine("Trickresult: " + trickResult + " Sum: " + trickResult);
			return trickResult;
		}


		public void PrintLastTrick()
		{
			if (currentTrick > 0)
			{
				Console.WriteLine("Last trick:");
				tricks[currentTrick - 1].PrintTrick();
				Console.WriteLine("");
			}
		}

		public void PrintCurrentTrick()
		{
			Console.WriteLine("Current trick:");
			tricks[currentTrick].PrintTrick();
			Console.WriteLine("");
		}

	}
}