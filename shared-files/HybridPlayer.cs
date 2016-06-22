using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class HybridPlayer : ArtificialPlayer
    {
        private InformationSet infoSet;

        public HybridPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
            : base(id)
        {
            infoSet = new InformationSet(id, initialHand, trumpCard, trumpPlayerId);
        }

        override public void AddPlay(int playerID, int card)
        {
            infoSet.AddPlay(playerID, card);
        }


        override public int Play()
        {
            int chosenCard;

            if (infoSet.GetHandSize() > 10)
            {
                chosenCard = infoSet.RuleBasedDecision();
            }
            else
            {
                chosenCard = PIMC.ExecuteWithHybridSearch(_id, infoSet, new List<int> { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 }, new List<int> { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 });
            }

            return chosenCard;
        }

        public int[] GetWinnerAndPointsAndTrickNumber()
        {
            return infoSet.GetWinnerAndPointsAndTrickNumber();
        }
    }
}