using System;

namespace SuecaSolver
{
	public class Trick
	{

		public Suit LeadSuit;
		private Move[] moves = new Move[4];
		private int currentMove;
		private Suit trump;
		private bool debugFlag;

		public Trick(Suit trumpSuit, bool debug)
		{
			LeadSuit = Suit.None;
			trump = trumpSuit;
			debugFlag = debug;
			currentMove = 0;
		}

		public void ApplyMove(Move move)
		{
			if (currentMove == 0)
			{
				LeadSuit = move.Card.Suit;
			}

			moves[currentMove] = move;
			move.Card.HasBeenPlayed = true;
			currentMove++;
		}

		public void UndoMove()
		{
			currentMove--;
			moves[currentMove].Card.HasBeenPlayed = false;
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
			Suit winningSuit = moves[0].Card.Suit;
			int highestValueFromWinningSuit = moves[0].Card.Value;
			int winningPlayerId = moves[0].PlayerId;
			int result = highestValueFromWinningSuit;

			if(debugFlag) Console.WriteLine("Card: " + moves[0].Card.ToString());

			for (int i = 1; i < 4; i++)
			{
				if(debugFlag) Console.WriteLine("Card: " + moves[i].Card.ToString());
				if (moves[i].Card.Suit == trump && winningSuit != trump)
				{
					winningSuit = trump;
					highestValueFromWinningSuit = moves[i].Card.Value;
					winningPlayerId = moves[i].PlayerId;
				}

				result += moves[i].Card.Value;

				if (moves[i].Card.Suit == winningSuit && moves[i].Card.Value > highestValueFromWinningSuit)
				{
					highestValueFromWinningSuit = moves[i].Card.Value;
					winningPlayerId = moves[i].PlayerId;
				}
			}

			if (winningPlayerId == 1 || winningPlayerId == 3)
			{
				return -1 * result;
			}
			return result;
		}
	}
}