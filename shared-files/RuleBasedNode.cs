using System.Collections.Generic;

namespace SuecaSolver
{
    public class RuleBasedNode : PlayerNode
    {

        private InformationSet infoSet;

        public RuleBasedNode(int id, List<int> hand, int trump)
            : base(id, hand, false)
        {
            infoSet = new InformationSet(id, hand, trump);
        }

        override public void ApplyMove(Move move)
        {
            if (move.PlayerId == Id)
            {
                Hand.Remove(move.Card);
                HasSuit[Card.GetSuit(move.Card)]--;
            }
            else
            {
            }
            infoSet.AddPlay(move.PlayerId, move.Card);
        }

        override public void UndoMove(Move move)
        {
            if (move.PlayerId == Id)
            {
                infoSet.RemovePlay(move.PlayerId, move.Card);
                Hand.Add(move.Card);
                HasSuit[Card.GetSuit(move.Card)]++;
            }
            else
            {
                //infoSet.RemovePlay(move.Card);
            }
        }

        public override int PlayGame(PerfectInformationGame pig, int alpha, int beta, int depthLimit, int card = -1)
        {
            if (pig.reachedDepthLimit(depthLimit) || pig.IsOtherTeamWinning() || pig.IsEndGame())
            {
                return pig.EvalGame();
            }

            int chosenCard = infoSet.RuleBasedDecision();
            Move move = new Move(Id, chosenCard);
            pig.ApplyMove(move);

            int value = pig.GetNextPlayer().PlayGame(pig, alpha, beta, depthLimit);
            pig.UndoMove(move);

            return value;
        }
    }
}