using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class ElephantPlayer : ArtificialPlayer
    {
        private int _idDiff;
        private int _handSize;
        private PIMC pimc;
        private InformationSet infoSet;


        public ElephantPlayer(int id, List<int> initialHand, int trumpSuit, Random randomLOL, int seed)
            : base(id)
        {
            _idDiff = 0 - id;
            _handSize = initialHand.Count;
            pimc = new PIMC();
            infoSet = new InformationSet(initialHand, trumpSuit, randomLOL, seed);
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
            int chosenCard;

            if (_handSize > 8)
            {
                chosenCard = infoSet.RuleBasedDecision();
            }
            else
            {
                chosenCard = pimc.ExecuteTestVersion(infoSet, new List<int> { 500, 500, 250, 100, 50, 15, 5, 3, 1, 1 }, true);
            }

            infoSet.AddMyPlay(chosenCard);
            _handSize--;
            return chosenCard;
        }
    }
}