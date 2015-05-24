using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MinPlayer : Player
    {

        public MinPlayer(int id, List<int> hand)
            : base(id, hand)
        {
        }

        override public int PlayGame(GameState gameState, int alpha, int beta, int depthLimit, int card = -1)
        {
            int v = Int32.MaxValue;
            List<int> moves;

            if (gameState.reachedDepthLimit(depthLimit))
            {
                return gameState.EvalGame();// + gameState.Heuristic(depthLimit);
            }

            if (gameState.IsOtherTeamWinning() || gameState.IsEndGame())
            {
                return gameState.EvalGame();
            }

            if (card == -1)
            {
                moves = SuecaGame.PossibleMoves(Hand, gameState.GetLeadSuit());
                gameState.orderPossibleMoves(moves, Id);
            }
            else
            {
                moves = new List<int>();
                moves.Add(card);
            }


            foreach (int move in moves)
            {
                gameState.ApplyMove(new Move(Id, move));
                Hand.Remove(move);
                HasSuit[Card.GetSuit(move)]--;
                int moveValue = gameState.GetNextPlayer().PlayGame(gameState, alpha, beta, depthLimit);

                if (moveValue < v)
                {
                    v = moveValue;
                }

                gameState.UndoMove();
                Hand.Add(move);
                HasSuit[Card.GetSuit(move)]++;

                if (v <= alpha)
                {
                    NumCuts++;
                    return v;
                }

                if (moveValue < beta)
                {
                    beta = moveValue;
                }
            }

            return v;
        }
    }
}