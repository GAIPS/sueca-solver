using System;

namespace SuecaSolver
{
	public class MaxPlayer : Player
	{

		public MaxPlayer(int id, Card[] hand) : base(id, hand)
		{
		}

		override public int PlayGame(GameState gameState, int alfa, int beta, Card card = null)
		{
			int bestMove = Int32.MinValue;
			Card[] moves;

			if (gameState.IsEndGame())
			{
				return gameState.EvalGame();
			}

			if (card == null)
			{
				moves = SuecaGame.PossibleMoves(Hand, gameState.GetLeadSuit());
			} else {
				moves = new Card[1];
				moves[0] = card;
			}

			foreach (Card move in moves)
			{
				gameState.ApplyMove(new Move(Id, move));
				int moveValue = gameState.GetNextPlayer().PlayGame(gameState, alfa, beta);

				if (moveValue > bestMove)
				{
					bestMove = moveValue;
				}

				if (moveValue > alfa)
				{
					alfa = moveValue;
				}

				gameState.UndoMove();

				if (bestMove >= beta)
				{
					// Console.WriteLine("Beta prunning!");
					break;
				}
			}

			return bestMove;
		}


		override public int PlayTrick(GameState gameState, Card card = null)
		{
			if (gameState.IsEndFirstTrick())
			{
				return gameState.GetFirstTrickPoints();
			}

			int bestMove = Int32.MinValue;
			Card[] moves;

			if (card == null)
			{
				moves = SuecaGame.PossibleMoves(Hand, gameState.GetLeadSuit());
			} else {
				moves = new Card[1];
				moves[0] = card;
			}

			foreach (Card move in moves)
			{
				gameState.ApplyMove(new Move(Id, move));
				int moveValue = gameState.GetNextPlayer().PlayTrick(gameState);
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