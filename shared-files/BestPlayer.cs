using System;
using System.Collections.Generic;

namespace SuecaSolver
{
    public class BestPlayer : ArtificialPlayer
    {
        private InformationSet infoSet;

        public BestPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
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
            return PIMC.Execute(_id, infoSet, 2, new List<int> { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000 });
        }

        public int[] GetWinnerAndPointsAndTrickNumber()
        {
            return infoSet.GetWinnerAndPointsAndTrickNumber();
        }
    }
}