using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class SmartestPlayer : ArtificialPlayer
    {
        private int _idDiff;
        private int _handSize;
        private PIMC pimc;
        private InformationSet infoSet;


        public SmartestPlayer(int id, List<int> initialHand, int trumpSuit, Random randomLOL, int seed)
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
            int[] bestCardAndValue;
            int chosenCard;

            if (_handSize > 10)
            {
                bestCardAndValue = new int[2];
                bestCardAndValue[0] = infoSet.RuleBasedDecision();
                bestCardAndValue[1] = 0; //TODO ver isto!
            }
            else
            {
                bestCardAndValue = pimc.ExecuteWithTimeLimit(infoSet);
            }

            chosenCard = bestCardAndValue[0];
            infoSet.AddMyPlay(chosenCard);
            infoSet.ExpectedGameValue = bestCardAndValue[1];
            _handSize--;
            return chosenCard;
        }
    }
}