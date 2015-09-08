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
                bestCardAndValue = pimc.Execute(infoSet);
            }

            chosenCard = bestCardAndValue[0];
            infoSet.AddMyPlay(chosenCard);
            //infoSet.ExpectedGameValue = bestCardAndValue[1];
            _handSize--;
            TrickExpectedReward = infoSet.predictTrickPoints();
            return chosenCard;
        }

        //public int GetExpectedScore()
        //{
        //    return infoSet.ExpectedGameValue;
        //}

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