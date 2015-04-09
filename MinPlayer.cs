using System;

namespace SuecaSolver
{
	public class MinPlayer : Player
	{

		public MinPlayer(int id, Card[] hand) : base(id, hand)
		{
		}

		override public int Play(GameState gameState)
		{
			int bestMove = 10000;

			if (gameState.IsEndGame())
			{
				Console.WriteLine("Min is evaluating the game");
				Console.WriteLine("Min eval: {0}", gameState.EvalGame());
				System.Environment.Exit(1);
			}

			Card[] moves = PossibleMoves(gameState);
			// PrintCards(moves);
			foreach (Card move in moves)
			{
				gameState.ApplyMove(move);
				int moveValue = NextPlayer.Play(gameState);

				if (moveValue < bestMove)
				{
					bestMove = moveValue;
				}
				gameState.UndoMove();
			}

			return bestMove;
		}
	}
}