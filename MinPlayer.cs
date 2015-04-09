using System;

namespace SuecaSolver
{
	public class MinPlayer : Player
	{

		public MinPlayer(int id, Card[] hand) : base(id, hand)
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

		override public int PlayTrick(GameState gameState)
		{
			if (gameState.IsEndTrick())
			{
				return gameState.EvalGame();
			}

			int bestMove = 100000;
			Card[] moves = PossibleMoves(gameState);
			foreach (Card move in moves)
			{
				gameState.ApplyMove(move);
				int moveValue = NextPlayer.PlayTrick(gameState);
				if (moveValue < bestMove)
				{
					bestMove = moveValue;
				}
				gameState.UndoMove();
			}
			return bestMove;
		}

		override public int PlayTrick(GameState gameState, Card move)
		{
			Console.WriteLine("FORBIDEN!");
			return 0;
		}
	}
}