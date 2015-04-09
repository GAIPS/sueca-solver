using System;

namespace SuecaSolver
{
	public class MaxPlayer : Player
	{
		private int possibleGamesCounter = 0;

		public MaxPlayer(int id, int numCards, Card[] hand) : base(id, numCards, hand)
		{
		}

		override public int PlayGame(GameState gameState)
		{
			if (gameState.IsEndGame())
			{
				possibleGamesCounter++;
				if (possibleGamesCounter < 2)
				{
					return gameState.EvalGame();
				}
				System.Environment.Exit(1);
			}

			int bestMove = 0;
			Card[] moves = PossibleMoves(gameState);
			foreach (Card move in moves)
			{
				gameState.ApplyMove(move);
				int moveValue = NextPlayer.PlayGame(gameState);
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