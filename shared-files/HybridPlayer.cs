using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class HybridPlayer : ArtificialPlayer
    {
        private InformationSet infoSet;
        public float TrickExpectedReward;

        public HybridPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
            : base(id)
        {
            infoSet = new InformationSet(id, initialHand, trumpCard, trumpPlayerId);
            TrickExpectedReward = 0.0f;
        }

        override public void AddPlay(int playerID, int card)
        {
            infoSet.AddPlay(playerID, card);
            TrickExpectedReward = infoSet.predictTrickPoints();
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

            TrickExpectedReward = infoSet.predictTrickPoints();
            return chosenCard;
        }
    }
}