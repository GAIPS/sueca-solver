using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class HybridPlayer : ArtificialPlayer
    {
        //private InformationSet InfoSet;

        public HybridPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
            : base(id)
        {
            InfoSet = new InformationSet(id, initialHand, trumpCard, trumpPlayerId);
        }

        override public void AddPlay(int playerID, int card)
        {
            InfoSet.AddPlay(playerID, card);
        }


        override public int Play()
        {
            int chosenCard;

            if (InfoSet.GetHandSize() > 10)
            {
                chosenCard = InfoSet.RuleBasedDecision();
            }
            else
            {
                chosenCard = PIMC.ExecuteWithHybridSearch(_id, InfoSet, new List<int> { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }, new List<int> { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
            }

            return chosenCard;
        }

        public int[] GetWinnerAndPointsAndTrickNumber()
        {
            return InfoSet.GetWinnerAndPointsAndTrickNumber();
        }
    }
}