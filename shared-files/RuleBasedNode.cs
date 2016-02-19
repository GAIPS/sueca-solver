using System.Collections.Generic;

namespace SuecaSolver
{
    public class RuleBasedNode : PlayerNode
    {


        public RuleBasedNode(int id, List<int> hand, int trumpCard, int trumpPlayerId)
            : base(id, hand, trumpCard, trumpPlayerId)
        {
        }

        public override int PlayGame(PerfectInformationGame pig, int alpha, int beta, int depthLimit, int card = -1)
        {
            if (Sueca.UTILITY_FUNC == 2 && pig.IsAnyTeamWinning())
            {
                return pig.EvalGame2();
            }
            if (pig.reachedDepthLimit(depthLimit) || pig.IsEndGame())
            {
                return pig.EvalGame1();
            }

            int chosenCard;
            if (card != -1)
            {
                chosenCard = card;
            }
            else
            {
                chosenCard = InfoSet.RuleBasedDecision();
            }
            Move move = new Move(Id, chosenCard);
            pig.ApplyMove(move);

            int value = pig.GetNextPlayer().PlayGame(pig, alpha, beta, depthLimit);
            pig.UndoMove(move);

            return value;
        }
    }
}