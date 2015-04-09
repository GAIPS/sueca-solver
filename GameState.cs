using System;

namespace SuecaSolver
{
	public class GameState
	{

		private Trick[] tricks;
		private int currentTrick;

		public GameState(int numTricks, Suit trumpSuit)
		{
			tricks = new Trick[numTricks];
			for (int i = 0; i < numTricks; i++)
			{
				tricks[i] = new Trick(trumpSuit);
			}
			currentTrick = 0;
		}

		public int GetCurrentTrick()
		{
			return currentTrick;
		}

		public void ApplyMove(Card move)
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
				Console.WriteLine("--- Trick " + i + ": ---");
				int trickResult = tricks[i].EvalTrick();
				if (trickResult > 0)
				{
					result += trickResult;
				}
				Console.WriteLine("Trickresult: " + trickResult + " Sum: " + result);
			}
			return result;
		}

		public int EvalTrick()
		{
			return tricks[currentTrick].EvalTrick();
		}

	}
}