using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class WorstPlayer : TIPlayer
    {

        public WorstPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
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
                chosenCard = PIMC.ExecuteWithWorstSelection(_id, infoSet, 1, new List<int> { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000 }, new List<int> { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
            }

            return chosenCard;
        }
    }
}