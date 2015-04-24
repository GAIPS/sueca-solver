using System;
using System.Collections.Generic;

namespace SuecaSolver
{
	public class MaxPlayer : Player
	{

		int lol = 0;

		public MaxPlayer(int id, List<Card> hand) : base(id, hand)
		{
		}

		override public int PlayGame(GameState gameState, int alpha, int beta, Card card = null)
		{
			// int bestMove = Int32.MinValue;
			List<Card> moves;

			if (gameState.IsEndGame())
			{
				// lol++;
				// if (lol == 2)
				// {
				// 	System.Environment.Exit(1);
				// }
				return gameState.EvalGame();
			}

			if (card == null)
			{
				moves = SuecaGame.PossibleMoves(Hand, gameState.GetLeadSuit());
			} else {
				moves = new List<Card>();
				moves.Add(card);
			}

			foreach (Card move in moves)
			{
				gameState.ApplyMove(new Move(Id, move));
				int moveValue = gameState.GetNextPlayer().PlayGame(gameState, alpha, beta);

				// if (moveValue > bestMove)
				// {
				// 	bestMove = moveValue;
				// }

				if (moveValue > alpha)
				{
					alpha = moveValue;
				}

				gameState.UndoMove();

				if (alpha >= beta)
				{
					// Console.WriteLine("Beta prunning!");
					// break;
					return alpha;
				}
			}

			// return bestMove;
			return alpha;
		}


		override public int PlayTrick(GameState gameState, Card card = null)
		{
			if (gameState.IsEndFirstTrick())
			{
				return gameState.GetFirstTrickPoints();
			}

			int bestMove = Int32.MinValue;
			List<Card> moves;

			if (card == null)
			{
				moves = SuecaGame.PossibleMoves(Hand, gameState.GetLeadSuit());
			} else {
				moves = new List<Card>();
				moves.Add(card);
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