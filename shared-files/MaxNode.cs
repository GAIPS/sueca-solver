using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class MaxNode : PlayerNode
    {

        public MaxNode(int id, List<int> hand, int trumpCard, int trumpPlayerId)
            : base(id, hand, trumpCard, trumpPlayerId)
        {
        }

        public MaxNode(int id, List<int> hand, int trump, InformationSet infoSet)
            : base(id, hand, infoSet)
        {
        }

        public override int PlayGame(PerfectInformationGame pig, int alpha, int beta, int depthLimit, int card)
        {
            int v = Int16.MinValue;
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
                cards = Sueca.PossibleMoves(Hand, pig.GetLeadSuit());
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

                if (moveValue > v)
                {
                    v = moveValue;
                }

                pig.UndoMove(move);

                if (v >= beta)
                {
                    return v;
                }

                if (v > alpha)
                {
                    alpha = v;
                }
            }

            return v;
        }
    }
}