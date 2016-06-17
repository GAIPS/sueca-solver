using System.Collections.Generic;

namespace SuecaSolver
{
    public class TrickPlayer : ArtificialPlayer
    {
        private InformationSet infoSet;


        public TrickPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
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
            int chosenCard = PIMC.Execute(_id, infoSet, 1);

            return chosenCard;
        }

        public int[] GetTrickWinnerAndPoints()
        {
            return infoSet.GetTrickWinnerAndPoints();
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