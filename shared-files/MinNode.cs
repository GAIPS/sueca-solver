using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MinNode : PlayerNode
    {

        public MinNode(int id, List<int> hand, bool USE_CACHE)
            : base(id, hand, USE_CACHE)
        {
        }

        public override int PlayGame(PerfectInformationGame pig, int alpha, int beta, int depthLimit, int card = -1)
        {
            int v = Int32.MaxValue;
            List<int> cards;

            if (pig.reachedDepthLimit(depthLimit) || pig.IsOtherTeamWinning() || pig.IsEndGame())
            {
                return pig.EvalGame();
            }

            if (card == -1)
            {
                cards = Sueca.PossibleMoves(Hand, pig.GetLeadSuit());
                //TODO gameState.orderPossibleMoves(moves, Id);
            }
            else
            {
                cards = new List<int>();
                cards.Add(card);
            }

            foreach (int c in cards)
            {
                Move move = new Move(Id, c);
                pig.ApplyMove(move);
                int moveValue = pig.GetNextPlayer().PlayGame(pig, alpha, beta, depthLimit);

                if (moveValue < v)
                {
                    v = moveValue;
                }

                pig.UndoMove(move);
                
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

        override public int PlayGame(GameState gameState, int alpha, int beta, int depthLimit, int card = -1)
        {
            int v = Int32.MaxValue;
        //    List<int> moves;

        //    if (gameState.reachedDepthLimit(depthLimit) || gameState.IsOtherTeamWinning() || gameState.IsEndGame())
        //    {
        //        return gameState.EvalGame();
        //    }

        //    if (card == -1)
        //    {
        //        moves = Sueca.PossibleMoves(Hand, gameState.GetLeadSuit());
        //        gameState.orderPossibleMoves(moves, Id);
        //    }
        //    else
        //    {
        //        moves = new List<int>();
        //        moves.Add(card);
        //    }

        //    foreach (int move in moves)
        //    {
        //        gameState.ApplyMove(new Move(Id, move));
        //        // Hand.Remove(move);
        //        // HasSuit[Card.GetSuit(move)]--;
        //        int moveValue = gameState.GetNextPlayer().PlayGame(gameState, alpha, beta, depthLimit);

        //        if (moveValue < v)
        //        {
        //            v = moveValue;
        //        }

        //        gameState.UndoMove();
        //        // Hand.Add(move);
        //        // HasSuit[Card.GetSuit(move)]++;

        //        if (v <= alpha)
        //        {
        //            NumCuts++;
        //            return v;
        //        }

        //        if (moveValue < beta)
        //        {
        //            beta = moveValue;
        //        }
        //    }

            return v;
        }
    }
}