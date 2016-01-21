using System.Collections.Generic;

namespace SuecaSolver
{
    public class RuleBasedNode : PlayerNode
    {

        private InformationSet infoSet;

        public RuleBasedNode(int id, List<int> hand, int trump)
            : base(id, hand, false)
        {
            infoSet = new InformationSet(hand, trump);
        }

        override public void ApplyMove(Move move)
        {
            if (move.PlayerId == Id)
            {
                infoSet.AddMyPlay(move.Card);
                Hand.Remove(move.Card);
                HasSuit[Card.GetSuit(move.Card)]--;
            }
            else
            {
                infoSet.AddPlay(move.PlayerId, move.Card);
            }
        }

        override public void UndoMove(Move move)
        {
            if (move.PlayerId == Id)
            {
                infoSet.RemoveMyPlay(move.Card);
                Hand.Add(move.Card);
                HasSuit[Card.GetSuit(move.Card)]++;
            }
            else
            {
                //infoSet.RemovePlay(move.Card);
            }
        }

        override public int PlayGame(GameState gameState, int alpha, int beta, int depthLimit, int card = -1)
        {
            if (gameState.reachedDepthLimit(depthLimit) || gameState.IsOtherTeamWinning() || gameState.IsEndGame())
            {
                return gameState.EvalGame();
            }

            int chosenCard = infoSet.RuleBasedDecision();
            gameState.ApplyMove(new Move(Id, chosenCard));
            // Hand.Remove(chosenCard);
            // HasSuit[Card.GetSuit(chosenCard)]--;

            int value = gameState.GetNextPlayer().PlayGame(gameState, alpha, beta, depthLimit);
            gameState.UndoMove();
            // Hand.Add(chosenCard);
            // HasSuit[Card.GetSuit(chosenCard)]++;

            return value;
        }
    }
}