using System;

namespace SuecaSolver
{
	public class MinPlayer : Player
	{

		public MinPlayer(int id, Card[] hand) : base(id, hand)
		{
		}

		override public int PlayGame(GameState gameState, int alfa, int beta, Card card = null)
		{
			int worstMove = Int32.MaxValue;
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

				if (moveValue < worstMove)
				{
					worstMove = moveValue;
				}

				if (moveValue < beta)
				{
					beta = moveValue;
				}

				gameState.UndoMove();

				if (worstMove <= alfa)
				{
					// Console.WriteLine("Alfa prunning!");
					break;
				}
			}

			return worstMove;
		}


		override public int PlayTrick(GameState gameState, Card card = null)
		{
			if (gameState.IsEndFirstTrick())
			{
				return gameState.GetFirstTrickPoints();
			}

			int worstMove = Int32.MaxValue;
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