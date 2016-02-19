using System.Collections.Generic;

namespace SuecaSolver
{
    public class RuleBasedPlayer : ArtificialPlayer
    {
        private InformationSet infoSet;

        public RuleBasedPlayer(int id, List<int> initialHand, int trumpCard, int trumpPlayerId)
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
            return infoSet.RuleBasedDecision();
        }
    }
}