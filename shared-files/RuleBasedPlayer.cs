using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class RuleBasedPlayer : ArtificialPlayer
    {
        private int _idDiff;
        private InformationSet infoSet;

        public RuleBasedPlayer(int id, List<int> initialHand, int trumpSuit)
            : base(id)
        {
            _idDiff = 0 - id;                                                                                                                                                                                                                                                                                                
            infoSet = new InformationSet(initialHand, trumpSuit);
        }

        override public void AddPlay(int playerID, int card)
        {
            int playerIdForMe = playerID + _idDiff;
            if (playerIdForMe < 0)
            {
                playerIdForMe += 4;
            }
            infoSet.AddPlay(playerIdForMe, card);
        }


        override public int Play()
        {
            int chosenCard = infoSet.RuleBasedDecision();
            infoSet.AddMyPlay(chosenCard);
            return chosenCard;
        }
    }
}