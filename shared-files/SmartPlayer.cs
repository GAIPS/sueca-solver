using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class SmartPlayer : ArtificialPlayer
    {
        private int _idDiff;
        private int _handSize;
        private PIMC pimc;
        private InformationSet infoSet;
        public float TrickExpectedReward;


        public SmartPlayer(int id, List<int> initialHand, int trumpSuit, Random randomLOL, int seed)
            : base(id)
        {
            _idDiff = 0 - id;
            _handSize = initialHand.Count;
            pimc = new PIMC();
            infoSet = new InformationSet(initialHand, trumpSuit, randomLOL, seed);
            TrickExpectedReward = 0.0f;
        }

        override public void AddPlay(int playerID, int card)
        {
            int playerIdForMe = playerID + _idDiff;
            if (playerIdForMe < 0)
            {
                playerIdForMe += 4;
            }
            infoSet.AddPlay(playerIdForMe, card);
            TrickExpectedReward = infoSet.predictTrickPoints();
        }


        override public int Play()
        {
            int chosenCard;

            if (_handSize > 10)
            {
                chosenCard = infoSet.RuleBasedDecision();
            }
            else
            {
                chosenCard = pimc.Execute(infoSet);
            }

            infoSet.AddMyPlay(chosenCard);
            _handSize--;
            TrickExpectedReward = infoSet.predictTrickPoints();
            return chosenCard;
        }

        public float PointsPercentage()
        {
            float alreadyMadePoints = infoSet.BotTeamPoints + infoSet.OtherTeamPoints;
            if (alreadyMadePoints == 0.0f)
            {
                return 1.0f;
            }
            return infoSet.BotTeamPoints / alreadyMadePoints;
        }

        public float GetHandHope()
        {
            return infoSet.GetHandHope();
        }
    }
}