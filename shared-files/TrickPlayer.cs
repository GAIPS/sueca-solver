using System.Collections.Generic;

namespace SuecaSolver
{
    public class TrickPlayer : ArtificialPlayer
    {
        //private InformationSet InfoSet;


        public TrickPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
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
            int chosenCard = PIMC.Execute(_id, InfoSet, 1);

            return chosenCard;
        }

        public int[] GetTrickWinnerAndPoints()
        {
            return InfoSet.GetWinnerAndPointsAndTrickNumber();
        }

        public float PointsPercentage()
        {
            float alreadyMadePoints = InfoSet.MyTeamPoints + InfoSet.OtherTeamPoints;
            if (alreadyMadePoints == 0.0f)
            {
                return 0.5f;
            }
            return InfoSet.MyTeamPoints / alreadyMadePoints;
        }
    }
}