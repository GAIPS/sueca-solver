using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MinNode : PlayerNode
    {

        public MinNode(int id, List<int> hand, int trumpCard, int trumpPlayerId)
            : base(id, hand, trumpCard, trumpPlayerId)
        {
        }

        public MinNode(int id, List<int> hand, int trump, InformationSet infoSet)
            : base(id, hand, infoSet)
        {
        }

        public override int PlayGame(PerfectInformationGame pig, int alpha, int beta, int depthLimit, int card = -1)
        {
            int v = Int16.MaxValue;
            List<int> cards;

            if (Sueca.UTILITY_FUNC == 2 && pig.IsAnyTeamWinning())
            {
                return pig.EvalGame2();
            }
            if (pig.reachedDepthLimit(depthLimit) || pig.IsEndGame())
            {
                return pig.EvalGame1();
            }

            if (card == -1)
            {
                cards = Sueca.PossibleMovesReduced(Hand, pig.GetLeadSuit());
                pig.orderPossibleMoves(cards, Id);
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