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

		public int GetLastPlayerId()
		{
			if (currentMove - 1 < 0)
			{
				Console.WriteLine("Trouble1!!!");
			}
			return moves[currentMove - 1].PlayerId;
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

		public void PrintTrick()
		{
			foreach (Move m in moves)
			{
				if (m != null)
				{
					Console.WriteLine(m);
				}
			}
		}

		public int GetTrickWinnerId()
		{
			bool oldDebugFlag = debugFlag;
			debugFlag = false;
			int winnerId = evalTrick()[0];
			debugFlag = oldDebugFlag;
			return winnerId;
		}

		public int GetTrickPoints()
		{
			return evalTrick()[1];
		}

		private int[] evalTrick()
		{
			Suit winningSuit = moves[0].Card.Suit;
			int highestValueFromWinningSuit = moves[0].Card.Value;
			int highestRankFromWinningSuit = (int) moves[0].Card.Rank;
			int winningPlayerId = moves[0].PlayerId;
			int points = highestValueFromWinningSuit;

			if(debugFlag) Console.WriteLine("Card: " + moves[0].Card.ToString() + " pID: " + moves[0].PlayerId);

			for (int i = 1; i < 4; i++)
			{
				if(debugFlag) Console.WriteLine("Card: " + moves[i].Card.ToString() + " pID: " + moves[i].PlayerId);
				if (moves[i].Card.Suit == trump && winningSuit != trump)
				{
					winningSuit = trump;
					highestValueFromWinningSuit = moves[i].Card.Value;
					highestRankFromWinningSuit = (int) moves[i].Card.Rank;
					winningPlayerId = moves[i].PlayerId;
				}
				else if (moves[i].Card.Suit == winningSuit &&
					moves[i].Card.Value >= highestValueFromWinningSuit &&
					(int) moves[i].Card.Rank > highestRankFromWinningSuit)
				{
					highestValueFromWinningSuit = moves[i].Card.Value;
					highestRankFromWinningSuit = (int) moves[i].Card.Rank;
					winningPlayerId = moves[i].PlayerId;
				}

				points += moves[i].Card.Value;
			}

			if (winningPlayerId == 1 || winningPlayerId == 3)
			{
				points = -1 * points;
			}
			return new int[] {winningPlayerId, points};
		}
	}
}