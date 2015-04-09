using System;

namespace SuecaSolver
{
	public class GameState
	{

		private Trick[] tricks = new Trick[10];
		private int currentTrick;

		public GameState(Suit trumpSuit)
		{
			for (int i = 0; i < 10; i++)
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

			if (tricks[currentTrick].IsEndTrick())
			{
				currentTrick++;
			}
		}

		public void UndoMove()
		{
			if (currentTrick == 10 || IsNewTrick())
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
			if (tricks[currentTrick].IsNewTrick())
			{
				return true;
			}
			return false;
		}

		public bool IsEndGame()
		{
			if (tricks[9].IsEndTrick())
			{
				return true;
			}
			return false;
		}

		public int EvalGame()
		{
			int result = 0;
			for (int i = 0; i < 10; i++)
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

	}
}