using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class BestPlayer : ArtificialPlayer
    {
        //private InformationSet InfoSet;

        public BestPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
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
            return PIMC.Execute(_id, InfoSet, 2, new List<int> { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 });
        }

        public int[] GetWinnerAndPointsAndTrickNumber()
        {
            return InfoSet.GetWinnerAndPointsAndTrickNumber();
        }
    }
}