using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class SmartPlayer : ArtificialPlayer
    {
        //private InformationSet InfoSet;


        public SmartPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
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
                chosenCard = PIMC.Execute(_id, InfoSet, 0, new List<int> { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }, new List<int> { 1000, 1000, 1000, 1000, 1000, 4, 4, 3, 3, 3 });
            }

            return chosenCard;
        }
    }
}