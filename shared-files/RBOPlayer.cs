using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class RBOPlayer : ArtificialPlayer
    {
        //private InformationSet InfoSet;

        public RBOPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
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
            int chosenCard;

            if (InfoSet.GetHandSize() > 10)
            {
                chosenCard = InfoSet.RuleBasedDecision();
            }
            else
            {
                //chosenCard = PIMC.Execute(_id, InfoSet, 1, new List<int> { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 });
                chosenCard = PIMC.Execute(_id, InfoSet, 1, new List<int> { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 });
                //chosenCard = PIMC.Execute(_id, infoSet, 1, new List<int> { 200, 200, 200, 200, 200, 200, 200, 200, 200, 200 });
                //chosenCard = PIMC.Execute(_id, infoSet, 1, new List<int> { 500, 500, 500, 500, 500, 500, 500, 500, 500, 500 });
            }

            return chosenCard;
        }

        public int[] GetWinnerAndPointsAndTrickNumber()
        {
            return InfoSet.GetWinnerAndPointsAndTrickNumber();
        }

        public int GetCurrentTrickWinner()
        {
            return InfoSet.GetCurrentTrickWinner();
        }

        public int GetCurrentTrickPoints()
        {
            return InfoSet.GetCurrentTrickPoints();
        }

        public bool HasNewTrickWinner()
        {
            return InfoSet.HasNewTrickWinner();
        }

        public bool HasNewTrickTeamWinner()
        {
            return InfoSet.HasNewTrickTeamWinner();
        }

        public int GetTrickIncrease()
        {
            return InfoSet.GetTrickIncrease();
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

        public int GetHandSize()
        {
            return InfoSet.GetHandSize();
        }

        public string GetLastPlayInfo()
        {
            return InfoSet.GetLastPlayInfo();
        }

        public bool IsLastPlayOfTrick()
        {
            return InfoSet.IsLastPlayOfTrick();
        }
    }
}