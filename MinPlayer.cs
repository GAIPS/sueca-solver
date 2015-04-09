using System;

namespace SuecaSolver
{
	public class MinPlayer : Player
	{

		public MinPlayer(int id, int numCards, Card[] hand) : base(id, numCards, hand)
		{
		}

		override public int PlayGame(GameState gameState)
		{
			int worstMove = Int32.MaxValue;
			Card[] moves = PossibleMoves(gameState);
			foreach (Card move in moves)
			{
				gameState.ApplyMove(move);
				int moveValue = NextPlayer.PlayGame(gameState);

				if (moveValue < worstMove)
				{
					worstMove = moveValue;
				}
				gameState.UndoMove();
			}

			return worstMove;
		}
	}
}