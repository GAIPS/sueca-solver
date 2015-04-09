using System;

namespace SuecaSolver
{
	public class Trick
	{

		public Suit LeadSuit;
		private Card[] moves = new Card[4];
		private int currentMove;
		private Suit trump;

		public Trick(Suit trumpSuit)
		{
			trump = trumpSuit;
			currentMove = 0;
		}

		public void ApplyMove(Card move)
		{
			if (currentMove == 0)
			{
				LeadSuit = move.Suit;
			}

			moves[currentMove] = move;
			move.HasBeenPlayed = true;
			currentMove++;
		}

		public void UndoMove()
		{
			currentMove--;
			moves[currentMove].HasBeenPlayed = false;
			moves[currentMove] = null;
		}

		public bool IsNewTrick()
		{
			if (moves[0] == null)
			{
				return true;
			}
			return false;
		}

		public bool IsEndTrick()
		{
			if (moves[3] != null)
			{
				return true;
			}
			return false;
		}

		public int EvalTrick()
		{
			Suit winningSuit = moves[0].Suit;
			int highestValueFromWinningSuit = moves[0].Value;
			int winningPlayer = 0;
			int result = highestValueFromWinningSuit;

			// Console.WriteLine("Card: " + moves[0].ToString());

			for (int i = 1; i < 4; i++)
			{
				// Console.WriteLine("Card: " + moves[i].ToString());
				if (moves[i].Suit == trump && winningSuit != trump)
				{
					winningSuit = trump;
					highestValueFromWinningSuit = moves[i].Value;
					winningPlayer = i;
				}

				result += moves[i].Value;

				if (moves[i].Suit == winningSuit && moves[i].Value > highestValueFromWinningSuit)
				{
					highestValueFromWinningSuit = moves[i].Value;
					winningPlayer = i;
				}
			}

			if (winningPlayer == 1 || winningPlayer == 3)
			{
				return -1 * result;
			}
			return result;
		}
	}
}