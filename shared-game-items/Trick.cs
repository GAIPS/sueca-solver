using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class Trick
	{

		public Suit LeadSuit;
		private List<Move> moves;
		private Suit trump;

		public Trick (Suit trumpSuit)
		{
			moves = new List<Move> (4);
			LeadSuit = Suit.None;
			trump = trumpSuit;
		}

		public List<Move> GetMoves ()
		{
			return moves;
		}

		public int getPlayInTrick ()
		{
			return moves.Count;
		}

		public void ApplyMove (Move move)
		{
			if (moves.Count == 0) {
				LeadSuit = move.Card.Suit;
			}

			moves.Add (move);
		}

		public void UndoMove ()
		{
			int currentMove = moves.Count - 1;
			moves.RemoveAt (currentMove);
		}

		public int GetLastPlayerId ()
		{
			if (moves.Count == 0) {
				Console.WriteLine ("Trick trouble at GetLastPlayerId!!!");
			}
			return moves [moves.Count - 1].PlayerId;
		}


		public bool IsEmpty ()
		{
			if (moves.Count == 0) {
				return true;
			}
			return false;
		}

		public bool IsFull ()
		{
			if (moves.Count == moves.Capacity) {
				return true;
			}
			return false;
		}

		public void PrintTrick ()
		{
			foreach (Move m in moves) {
				Console.WriteLine (m);
			}
		}

		public int GetTrickWinnerId ()
		{
			return evalTrick () [0];
		}

		public int GetTrickPoints ()
		{
			return evalTrick () [1];
		}

		private int[] evalTrick ()
		{
			if (moves.Count == 0) {
				Console.WriteLine ("Trick trouble at evalTrick!!!");
			}

			Suit winningSuit = moves [0].Card.Suit;
			int highestValueFromWinningSuit = moves [0].Card.Value;
			int highestRankFromWinningSuit = (int)moves [0].Card.Rank;
			int winningPlayerId = moves [0].PlayerId;
			int points = highestValueFromWinningSuit;


			for (int i = 1; i < 4; i++) {
				
				if (moves [i].Card.Suit == trump && winningSuit != trump) {
					winningSuit = trump;
					highestValueFromWinningSuit = moves [i].Card.Value;
					highestRankFromWinningSuit = (int)moves [i].Card.Rank;
					winningPlayerId = moves [i].PlayerId;
				} else if (moves [i].Card.Suit == winningSuit &&
				           moves [i].Card.Value >= highestValueFromWinningSuit &&
				           (int)moves [i].Card.Rank > highestRankFromWinningSuit) {
					highestValueFromWinningSuit = moves [i].Card.Value;
					highestRankFromWinningSuit = (int)moves [i].Card.Rank;
					winningPlayerId = moves [i].PlayerId;
				}

				points += moves [i].Card.Value;
			}

			if (winningPlayerId == 1 || winningPlayerId == 3) {
				points = -1 * points;
			}
			return new int[] { winningPlayerId, points };
		}
	}
}