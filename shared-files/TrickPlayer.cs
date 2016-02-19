using System.Collections.Generic;

namespace SuecaSolver
{
    public class TrickPlayer : ArtificialPlayer
    {
        //private int _idDiff;
        public int HandSize;
        private InformationSet infoSet;
        public float TrickExpectedReward;


        public TrickPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
            : base(id)
        {
            //_idDiff = 0 - id;
            HandSize = initialHand.Count;
            infoSet = new InformationSet(id, initialHand, trumpCard, trumpPlayerId);
            TrickExpectedReward = 0.0f;
        }

        override public void AddPlay(int playerID, int card)
        {
            //int playerIdForMe = playerID + _idDiff;
            //if (playerIdForMe < 0)
            //{
            //    playerIdForMe += 4;
            //}
            infoSet.AddPlay(playerID, card);
            TrickExpectedReward = infoSet.predictTrickPoints();
        }

        override public int Play()
        {
            int chosenCard = PIMC.Execute(_id, infoSet, 1);

            HandSize--;
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
    }
}