using System;

namespace SuecaSolver
{
	public class MaxPlayer : Player
	{
		private int lol = 0;

		public MaxPlayer(int id, Card[] hand) : base(id, hand)
		{
		}

		override public int Play(GameState gameState)
		{
			int bestMove = 0;

			if (gameState.IsEndGame())
			{
				// Console.WriteLine("Max is evaluating the game and lol: " + lol);
				// Console.WriteLine("Max eval: {0}", gameState.EvalGame());
				int gameResult = gameState.EvalGame();

				lol++;
				// if (lol == 2)
				// {
				// 	System.Environment.Exit(1);
				// }
				return gameResult;
			}

			Card[] moves = PossibleMoves(gameState);
			// PrintCards(moves);
			foreach (Card move in moves)
			{
				if (Id == 0 && gameState.GetCurrentTrick() == 0)
				{
					Console.WriteLine("next move from player 0 at trick 0 and lol: " + lol);
				}
				gameState.ApplyMove(move);
				int moveValue = NextPlayer.Play(gameState);

				if (moveValue > bestMove)
				{
					bestMove = moveValue;
				}
				gameState.UndoMove();
			}

			return bestMove;
		}
	}
}