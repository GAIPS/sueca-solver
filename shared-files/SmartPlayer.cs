using System.Collections.Generic;

namespace SuecaSolver
{
    public class SmartPlayer : ArtificialPlayer
    {
        //private int _idDiff;
        //private PIMC pimc;
        private InformationSet infoSet;
        public float TrickExpectedReward;


        public SmartPlayer(int id, List<int> initialHand, int trumpSuit)
            : base(id)
        {
            //_idDiff = 0 - id;
            //pimc = new PIMC();
            infoSet = new InformationSet(id, initialHand, trumpSuit);
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
                chosenCard = PIMC.Execute(infoSet, new List<int> { 50, 50, 50, 50, 50, 50, 50, 50, 50, 50 }, new List<int> { 1000, 1000, 1000, 1000, 1000, 4, 4, 3, 3, 3 });
            }

            //infoSet.AddMyPlay(chosenCard);
            TrickExpectedReward = infoSet.predictTrickPoints();
            return chosenCard;
        }

        public float PointsPercentage()
        {
            float alreadyMadePoints = infoSet.MyTeamPoints + infoSet.OtherTeamPoints;
            if (alreadyMadePoints == 0.0f)
            {
                return 0.5f;
            }
            return infoSet.MyTeamPoints / alreadyMadePoints;
        }

        public int GetHandSize()
        {
            return infoSet.GetHandSize();
        }
    }
}