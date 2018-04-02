using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class RuleBasedPlayer : ArtificialPlayer
    {
        //private InformationSet InfoSet;

        public RuleBasedPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
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
            return InfoSet.RuleBasedDecision();
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


        //attribute the event to the winner when he is from my team and blame himself or the partner when winner is an opponent
        public int GetResposibleForLastTrick()
        {
            return InfoSet.GetCurrentTrickResponsible();
        }
    }
}